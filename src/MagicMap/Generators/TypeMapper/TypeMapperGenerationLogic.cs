// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperGenerationLogic.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators.TypeMapper;

using System;
using System.Collections.Generic;
using System.Text;

using MagicMap.Extensions;
using MagicMap.Utils;

using Microsoft.CodeAnalysis;

internal class TypeMapperGenerationLogic
{
   private ITypeMapperContext Context { get; }

   public TypeMapperGenerationLogic(ITypeMapperContext context)
   {
      Context = context ?? throw new ArgumentNullException(nameof(context));
   }

   #region Public Methods and Operators

   public void Generate(IClassBuilder mapperGenerator)
   {
      GeneratedSingletonInstance(mapperGenerator, Context);
      GenerateLeftToRight(mapperGenerator);
      GenerateRightToLeft(mapperGenerator);
      GenerateOverrides(mapperGenerator, Context);
   }

   private void GenerateRightToLeft(IClassBuilder mapperGenerator)
   {
      if (Context.Mode == GeneratorMode.LeftToRight)
         return;

      if (Context.SourceEqualsTargetType || mapperGenerator.ContainsMethod("Map", Context.TargetType, Context.SourceType))
         return;

      var rightToLeft = new PropertyMappingContext(Context, Context.TargetType, Context.SourceType, Context.MappingSpecifications);
      mapperGenerator.AddMethod("Map", (Context.TargetType, "source"), (Context.SourceType, "target"))
         .WithModifier("public")
         .WithSummary(x => x.AppendLine("Maps all properties of the <see cref=\"source\"/> to the properties of the <see cref=\"target\"/>"))
         .WithBody(_ => GenerateMapBody(mapperGenerator, rightToLeft))
         .Build();

      foreach (var declaration in rightToLeft.MemberDeclarations)
         mapperGenerator.AppendLine(declaration());

      mapperGenerator.AddMethod("MapFrom", (Context.TargetType, "source"))
         .WithReturnType(Context.SourceType)
         .WithModifier(ComputeModifier(Context))
         .WithBody(() => GenerateMapFromBody(Context.TargetType, Context.SourceType))
         .Build();
   }

   private void GenerateLeftToRight(IClassBuilder mapperGenerator)
   {
      if (Context.Mode == GeneratorMode.RightToLeft)
         return;

      var leftToRight = new PropertyMappingContext(Context, Context.SourceType, Context.TargetType, InvertMappings(Context.MappingSpecifications));
      mapperGenerator.AddMethod("Map", (Context.SourceType, "source"), (Context.TargetType, "target"))
         .WithModifier("public")
         .WithSummary(x => x.AppendLine("Maps all properties of the <see cref=\"source\"/> to the properties of the <see cref=\"target\"/>"))
         .WithBody(_ => GenerateMapBody(mapperGenerator, leftToRight))
         .Build();

      foreach (var declaration in leftToRight.MemberDeclarations)
         mapperGenerator.AppendLine(declaration());

      mapperGenerator.AddMethod("MapFrom", (Context.SourceType, "source"))
         .WithReturnType(Context.TargetType)
         .WithModifier(ComputeModifier(Context))
         .WithBody(() => GenerateMapFromBody(Context.SourceType, Context.TargetType))
         .Build();
   }

   #endregion

   #region Methods

   private static string ComputeModifier(ITypeMapperContext context)
   {
      if (context.MapperType == null)
         return "public";

      if (IsInternal(context.MapperType))
         return "internal";
      if (IsInternal(context.TargetType))
         return "internal";
      if (IsInternal(context.SourceType))
         return "internal";

      return "public";
   }

   private string CreatePartialMethod(IPropertySymbol targetProperty, INamedTypeSymbol sourceType)
   {
      var builder = new StringBuilder();
      builder.AppendLine($"/// <summary>Must be implemented (because ForceMappings was set to true) to support the mapping of the {targetProperty.Name} property</summary>");
      builder.AppendLine($"private partial {targetProperty.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} Map{targetProperty.Name}({sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} value);/*NEWLINE*/");
      return builder.ToString();
   }

   private string CreatePartialMethod(IPropertySymbol sourceProperty, IPropertySymbol targetProperty)
   {
      var builder = new StringBuilder();
      if (Context.ForceMappings)
      {
         builder.AppendLine($"/// <summary>Can must be implemented to support the mapping of the {targetProperty.Name} property</summary>");
         builder.AppendLine($"private partial {targetProperty.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} Map{targetProperty.Name}({sourceProperty.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} value);/*NEWLINE*/");
      }
      else
      {
         builder.AppendLine($"/// <summary>Can be implemented to support the mapping of the {targetProperty.Name} property</summary>");
         builder.AppendLine($"partial void Map{targetProperty.Name}({targetProperty.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} target, {sourceProperty.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} value);/*NEWLINE*/");
      }
      return builder.ToString();
   }

   private string CreatePropertyMapping(IClassBuilder mapperClassGenerator, PropertyMappingContext propertyContext,
      IPropertySymbol sourceProperty, IPropertySymbol targetProperty)
   {
      if (targetProperty.Type.Equals(sourceProperty.Type, SymbolEqualityComparer.Default))
         return $"target.{targetProperty.Name} = source.{sourceProperty.Name};";

      if (TryCreateRecursiveMapping(propertyContext, sourceProperty, targetProperty, out var recursiveMapping))
         return recursiveMapping;

      if (TryCreateNestedMapping(mapperClassGenerator, sourceProperty, targetProperty, out var nestedMapping))
         return nestedMapping;

      if (TryCreateEnumMapping(propertyContext, sourceProperty, targetProperty, out var enumMapping))
         return enumMapping;

      if (Context.ForceMappings)
      {
         propertyContext.AddMemberDeclaration(() => CreatePartialMethod(sourceProperty, targetProperty));
         return $"target.{targetProperty.Name} = Map{targetProperty.Name}(source.{sourceProperty.Name});";
      }

      propertyContext.AddMemberDeclaration(() => CreatePartialMethod(sourceProperty, targetProperty));
      return $"Map{targetProperty.Name}(target, source.{sourceProperty.Name});";
   }

   private static void GeneratedSingletonInstance(IClassBuilder mapperGenerator, ITypeMapperContext context)
   {
      if (mapperGenerator.ContainsProperty("Default"))
         return;

      mapperGenerator.AppendLine("/// <summary>");
      mapperGenerator.AppendLine("/// The default singleton instance of the generated type mapper.");
      mapperGenerator.AppendLine("/// To customize the creation of the default mapper, just implement this property in the defining partial part.");
      mapperGenerator.AppendLine("/// </summary>");

      if (mapperGenerator.UserDefinedPart == null || mapperGenerator.UserDefinedPart.GetDefaultConstructor() != null)
      {
         mapperGenerator.AppendLine($"public static {context.MapperName} Default {{ get; }} = new {context.MapperName}();/*NEWLINE*/");
      }
      else
      {
         mapperGenerator.AppendLine(
            $"public static {context.MapperName} Default => throw new global::System.NotSupportedException(\"The type {context.MapperName} does not define a default constructor.\");");
      }
   }

   private string GenerateMapBody(IClassBuilder mapperClassGenerator, PropertyMappingContext propertyContext)
   {
      var bodyCode = new StringBuilder();

      if (propertyContext.TargetProperties.Count == 0)
      {
         bodyCode.AppendLine("// target type does not contain any properties.");
         bodyCode.AppendLine("// No mappings were generated");
         return bodyCode.ToString();
      }

      foreach (var targetProperty in propertyContext.TargetProperties.Values)
      {
         if (propertyContext.TryCreateCustomMappings(targetProperty, out var customMapping))
         {
            bodyCode.AppendLine(customMapping);
         }
         else
         {
            MapProperty(mapperClassGenerator, propertyContext, targetProperty, bodyCode);
         }
      }

      bodyCode.AppendLine("MapOverride(source, target);");
      return bodyCode.ToString();
   }

   private void MapProperty(IClassBuilder mapperClassGenerator, PropertyMappingContext propertyContext,
      IPropertySymbol targetProperty, StringBuilder bodyCode)
   {
      if (!MappingPossible(targetProperty))
         return;

      var sourcePropertyName = GetSourcePropertyName(propertyContext.PropertyMappings, targetProperty.Name);
      if (TryFindSourceProperty(propertyContext.SourceProperties, sourcePropertyName, out var sourceProperty))
      {
         var mapping = CreatePropertyMapping(mapperClassGenerator, propertyContext, sourceProperty, targetProperty);
         bodyCode.AppendLine(mapping);
      }
      else if(Context.ForceMappings)
      {
         propertyContext.AddMemberDeclaration(() => CreatePartialMethod(targetProperty, propertyContext.SourceType));
         bodyCode.AppendLine($"target.{targetProperty.Name} = Map{targetProperty.Name}(source);");
      }
      else
      {
         // We ignore the property because we could not find a source
      }
   }

   private static string GenerateMapFromBody(INamedTypeSymbol sourceType, INamedTypeSymbol targetType)
   {
      var fullSourceName = sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      var fullTargetName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

      var builder = new StringBuilder();
      builder.AppendLine($"var target = Default is MagicMap.ITypeFactory<{fullTargetName}, {fullSourceName}> factory");
      builder.AppendLine("? factory.Create(source)");
      var constructor = targetType.GetDefaultConstructor();
      if (constructor == null || constructor.IsPrivate())
      {
         builder.AppendLine(
            $": throw new global::System.NotSupportedException(\"The target type {targetType.Name} can not be created. Provide a accessible constructor without parameters or implement the ITypeFactory interface in the responsible mapper.\");");
      }
      else
      {
         builder.AppendLine($": new {fullTargetName}();");
      }

      builder.AppendLine("Default.Map(source, target);");
      builder.AppendLine("return target;");
      return builder.ToString();
   }

   private void GenerateOverrides(IClassBuilder mapperGenerator, ITypeMapperContext context)
   {
      if (context.Mode is GeneratorMode.TwoWay or GeneratorMode.LeftToRight)
      {
         mapperGenerator.AddPartialMethod("MapOverride", (context.SourceType, "source"), (context.TargetType, "target"))
            .WithSummary("Implement this method, to map the properties the mapper could not handle for any reason.");
      }

      if (context.Mode is GeneratorMode.TwoWay or GeneratorMode.RightToLeft)
      {
         mapperGenerator.AddPartialMethod("MapOverride", (context.TargetType, "source"), (context.SourceType, "target"))
            .WithCondition(_ => !context.SourceEqualsTargetType)
            .WithSummary("Implement this method, to map the properties the mapper could not handle for any reason.");
      }
   }

   private static string GetSourcePropertyName(IDictionary<string, MappingDescription> nameMappings, string targetName)
   {
      return nameMappings.TryGetValue(targetName, out var sourceDescription) ? sourceDescription.Name : targetName;
   }

   private static IDictionary<string, MappingDescription> InvertMappings(IDictionary<string, MappingDescription> mappingSpecifications)
   {
      var inverted = new Dictionary<string, MappingDescription>();
      foreach (var specification in mappingSpecifications)
         inverted[specification.Value.Name] = new MappingDescription { Name = specification.Key, Ignored = specification.Value.Ignored };
      return inverted;
   }

   private static bool IsInternal(INamedTypeSymbol typeSymbol)
   {
      switch (typeSymbol.DeclaredAccessibility)
      {
         case Accessibility.Public:
            return false;
      }

      return true;
   }

   private static bool MappingPossible(IPropertySymbol target)
   {
      if (target.IsReadOnly)
         return false;

      if (target.DeclaredAccessibility == Accessibility.Private)
         return false;

      // TODO this is at the moment the solution for properties of type List<T>
      // because this leads to a stack overflow...did not check why til now
      if (target.Type is INamedTypeSymbol { IsGenericType: true })
         return false;

      return true;
   }

   private static bool TryCreateEnumMapping(PropertyMappingContext propertyContext, IPropertySymbol sourceProperty, IPropertySymbol targetProperty,
      out string enumMapping)
   {
      enumMapping = null;
      if (EnumConverterGenerator.IsEnum(sourceProperty.Type) && EnumConverterGenerator.IsEnum(targetProperty.Type))
      {
         enumMapping = $"target.{targetProperty.Name} = ConvertEnum(source.{sourceProperty.Name});";

         var existingMethod = propertyContext.MapperType.FindMethod(targetProperty.Type, "ConvertEnum", sourceProperty.Type);
         if (existingMethod == null)
         {
            // We could not find an user defined implementation of the enum conversion method => we have to generate one
            propertyContext.AddMemberDeclaration(() => EnumConverterGenerator.GenerateEnum(sourceProperty.Type, targetProperty.Type, "ConvertEnum"));
         }

         return true;
      }

      return false;
   }

   private bool TryCreateNestedMapping(IClassBuilder mapperClassGenerator, IPropertySymbol sourceProperty, IPropertySymbol targetProperty, out string mappingCode)
   {
      mappingCode = null;
      if (targetProperty.Type.IsValueType || targetProperty.Name == "String")
         return false;
      if (sourceProperty.Type.IsValueType || sourceProperty.Name == "String")
         return false;

      var nestedMapperName = ComputeNestedClassName(sourceProperty, targetProperty);

      if (targetProperty.Type is INamedTypeSymbol targetType && sourceProperty.Type is INamedTypeSymbol sourceType)
      {
         if (sourceProperty.ContainingType == Context.SourceType)
            mapperClassGenerator.AddTypeMapperBuilder(nestedMapperName, sourceType, targetType);
      }

      mappingCode = $"target.{targetProperty.Name} = {nestedMapperName}.Default.MapFrom(source.{sourceProperty.Name});";

      return true;
   }

   private string ComputeNestedClassName(IPropertySymbol sourceProperty, IPropertySymbol targetProperty)
   {
      if (sourceProperty.ContainingType.Equals(Context.SourceType, SymbolEqualityComparer.Default))
         return $"{sourceProperty.Type.Name}Mapper";
      return $"{targetProperty.Type.Name}Mapper";
   }

   private static bool TryCreateRecursiveMapping(PropertyMappingContext propertyContext, IPropertySymbol sourceProperty,
      IPropertySymbol targetProperty, out string mappingCode)
   {
      if (propertyContext.TargetType.Equals(targetProperty.Type, SymbolEqualityComparer.Default)
          && propertyContext.SourceType.Equals(sourceProperty.Type, SymbolEqualityComparer.Default))
      {
         mappingCode = $"target.{targetProperty.Name} = source.{sourceProperty.Name}?.To{targetProperty.Type.Name}();";
         return true;
      }

      mappingCode = null;
      return false;
   }

   private static bool TryFindSourceProperty(IDictionary<string, IPropertySymbol> sourceProperties, string sourceName, out IPropertySymbol source)
   {
      return sourceProperties.TryGetValue(sourceName, out source);
   }

   #endregion
}