// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators.TypeMapper;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using MagicMap.Generators;
using MagicMap.Utils;
using Microsoft.CodeAnalysis;

/// <summary>Generator for type mapping</summary>
/// <seealso cref="MagicMap.Generators.IGenerator" />
internal class TypeMapperGenerator : IGenerator
{
   #region Constants and Fields

   private readonly ITypeMapperContext context;

   private readonly IUniqueNameProvider uniqueNameProvider;

   #endregion

   #region Constructors and Destructors

   public TypeMapperGenerator(ITypeMapperContext context, IUniqueNameProvider uniqueNameProvider)
   {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
      this.uniqueNameProvider = uniqueNameProvider ?? throw new ArgumentNullException(nameof(uniqueNameProvider));
   }

   #endregion

   #region IGenerator Members

   public IEnumerable<GeneratedSource> Generate()
   {
      var mapperClass = GenerateMapperClass();
      yield return new GeneratedSource
      {
         Name = uniqueNameProvider.GetFileNameForClass(context.MapperType.Name),
         Code = mapperClass.GenerateCode()
      };

      var extensionsClass = GenerateExtensionsClass();
      var extensionFileName = uniqueNameProvider.GetFileNameForClass(extensionsClass.ClassName);
      yield return new GeneratedSource
      {
         Name = extensionFileName,
         Code = extensionsClass.GenerateCode()
      };
   }

   #endregion

   #region Properties

   private string MapperTypeName => context.MapperType.Name;

   #endregion

   #region Methods

   private static bool IsInternal(INamedTypeSymbol typeSymbol)
   {
      switch (typeSymbol.DeclaredAccessibility)
      {
         case Accessibility.Public:
            return false;
      }

      return true;
   }

   private void AppendMapperSignature(PartialClassGenerator mapperGenerator, INamedTypeSymbol fromType,
      INamedTypeSymbol toType)
   {
      mapperGenerator.AppendLine("/// <summary>");
      mapperGenerator.AppendLine("/// Maps all properties of the <see cref=\"source\"/> to the properties of the <see cref=\"target\"/>");
      mapperGenerator.AppendLine("/// </summary>");
      var stringBuilder = new StringBuilder();
      stringBuilder.Append("public void Map(");
      stringBuilder.Append(fromType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
      stringBuilder.Append(" source");
      stringBuilder.Append(", ");
      stringBuilder.Append(toType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
      stringBuilder.Append(" target");
      stringBuilder.Append(")");

      mapperGenerator.AppendLine(stringBuilder.ToString());
   }

   private string ComputeModifier()
   {
      if (IsInternal(context.MapperType))
         return "internal";
      if (IsInternal(context.TargetType))
         return "internal";
      if (IsInternal(context.SourceType))
         return "internal";

      return "public";
   }

   private ClassGenerationContext GenerateExtensionsClass()
   {
      var mapperExtensions = CreateGenerationContext();
      GenerateMapperProperty(mapperExtensions);
      GenerateExtensionMethod(mapperExtensions, context.TargetType, context.SourceType);

      if (!context.SourceEqualsTargetType)
         GenerateExtensionMethod(mapperExtensions, context.SourceType, context.TargetType);

      return mapperExtensions;
   }

   private ClassGenerationContext CreateGenerationContext()
   {
      if (context.MapperExtensionsType == null)
      {
         return new ClassGenerationContext($"{MapperTypeName}Extensions")
         {
            IsStatic = true,
            Partial = true,
            Modifier = ComputeModifier(),
            Namespace = context.MapperType.ContainingNamespace

         };
      }

      return new ClassGenerationContext(context.MapperExtensionsType)
      {
         IsStatic = true,
      };
   }

   private void GenerateMapperProperty(ClassGenerationContext generationContext)
   {
      if (generationContext.ContainsProperty("Mapper"))
         return;

      var builder = new StringBuilder();
      builder.AppendLine("/// <summary>");
      builder.AppendLine($"/// The instance of the <see cref=\"{MapperTypeName}\"/> all extension methods use.");
      builder.AppendLine($"/// You can customize this by implementing this property in your own partial implementation of the extensions class.");
      builder.AppendLine("/// </summary>");
      builder.AppendLine($"private static {context.MapperType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} Mapper => {context.MapperType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.Default;");
      generationContext.AddCode(builder.ToString());
   }

   private PartialClassGenerator GenerateMapperClass()
   {
      var mapperGenerator = new PartialClassGenerator(context.MapperType);

      GeneratedSingletonInstance(mapperGenerator);

      var leftToRight = new PropertyMappingContext(context, context.SourceType, context.TargetType, InvertMappings(context.MappingSpecifications));
      mapperGenerator.AddMethod("Map", (context.SourceType, "source"), (context.TargetType, "target"))
         .WithModifier("public")
         .WithSummary(x => x.AppendLine("Maps all properties of the <see cref=\"source\"/> to the properties of the <see cref=\"target\"/>"))
         .WithBody(() => GenerateMapBody(leftToRight))
         .Build();

      foreach (var declaration in leftToRight.MemberDeclarations)
         mapperGenerator.AppendLine(declaration());

      if (!context.SourceEqualsTargetType && !mapperGenerator.ContainsMethod("Map", context.TargetType, context.SourceType))
      {
         var rightToLeft = new PropertyMappingContext(context, context.TargetType, context.SourceType, context.MappingSpecifications);
         mapperGenerator.AddMethod("Map", (context.TargetType, "source"), (context.SourceType, "target"))
            .WithModifier("public")
            .WithSummary(x => x.AppendLine("Maps all properties of the <see cref=\"source\"/> to the properties of the <see cref=\"target\"/>"))
            .WithBody(() => GenerateMapBody(rightToLeft))
            .Build();

         foreach (var declaration in rightToLeft.MemberDeclarations)
            mapperGenerator.AppendLine(declaration());
      }

      GenerateOverrides(mapperGenerator);
      return mapperGenerator;
   }

   private void GenerateOverrides(PartialClassGenerator mapperGenerator)
   {
      mapperGenerator.AddPartialMethod("MapOverride", (context.SourceType, "source"), (context.TargetType, "target"))
         .WithSummary("Implement this method, to map the properties the mapper could not handle for any reason.");

      mapperGenerator.AddPartialMethod("MapOverride", (context.TargetType, "source"), (context.SourceType, "target"))
         .WithCondition(_ => !context.SourceEqualsTargetType)
         .WithSummary("Implement this method, to map the properties the mapper could not handle for any reason.");
   }

   private void GeneratedSingletonInstance(PartialClassGenerator mapperGenerator)
   {
      if (mapperGenerator.ContainsProperty("Default"))
         return;

      mapperGenerator.AppendLine("/// <summary>");
      mapperGenerator.AppendLine("/// The default singleton instance of the generated type mapper.");
      mapperGenerator.AppendLine("/// To customize the creation of the default mapper, just implement this property in the defining partial part.");
      mapperGenerator.AppendLine("/// </summary>");

      if (mapperGenerator.UserDefinedPart.GetDefaultConstructor() != null)
      {
         mapperGenerator.AppendLine($"public static {context.MapperName()} Default {{ get; }} = new {context.MapperName()}();/*NEWLINE*/");
      }
      else
      {
         mapperGenerator.AppendLine($"public static {context.MapperName()} Default => throw new global::System.NotSupportedException(\"The type {context.MapperName()} does not define a default constructor.\");");
      }
   }


   private string GenerateMapBody(PropertyMappingContext propertyContext)
   {
      var bodyCode = new StringBuilder();

      if (propertyContext.TargetProperties.Count == 0)
      {
         bodyCode.AppendLine("// target type does not contain any properties.");
         bodyCode.AppendLine("// No mappings were generated");
      }
      else
      {
         // CreateCustomMappings(bodyCode, sourceProperties, targetProperties);

         foreach (var targetProperty in propertyContext.TargetProperties.Values)
         {
            if (propertyContext.TryCreateCustomMappings(targetProperty, out var customMapping))
            {
               bodyCode.AppendLine(customMapping);
            }
            else
            {
               if (!MappingPossible(targetProperty))
                  continue;

               var sourcePropertyName = GetSourcePropertyName(propertyContext.PropertyMappings, targetProperty.Name);
               if (TryFindSourceProperty(propertyContext.SourceProperties, sourcePropertyName, out var sourceProperty))
               {
                  var mapping = CreatePropertyMapping(propertyContext, sourceProperty, targetProperty);
                  bodyCode.AppendLine(mapping);
               }

            }
         }
      }

      bodyCode.AppendLine("MapOverride(source, target);");
      return bodyCode.ToString();
   }

   private void CreateCustomMappings(StringBuilder sourceBuilder, Dictionary<string, IPropertySymbol> sourceProperties, Dictionary<string, IPropertySymbol> targetProperties)
   {
      foreach (var (method, attributeData) in context.MapperType.GetMethodsWithAttribute(context.PropertyMapperAttribute))
      {
         if (attributeData.ConstructorArguments.Length == 3)
         {
            var sourcePropertyName = attributeData.ConstructorArguments[0].Value as string;
            var targetPropertyName = attributeData.ConstructorArguments[1].Value as string;

            if (sourceProperties.TryGetValue(sourcePropertyName, out var sourceProperty) && targetProperties.TryGetValue(targetPropertyName, out var targetProperty))
            {
               if (TryCreateInvocation(method, sourceProperty, targetProperty, out var invocation))
               {
                  sourceProperties.Remove(sourcePropertyName);
                  targetProperties.Remove(targetPropertyName);
                  sourceBuilder.AppendLine(invocation);
               }
            }
         }
      }
   }

   private bool TryCreateInvocation(IMethodSymbol method, IPropertySymbol sourceProperty, IPropertySymbol targetProperty, out string invocation)
   {
      var parameterCount = method.Parameters.Length;
      if (method.ReturnsVoid && parameterCount == 2)
      {
         if (method.IsCallableWith(sourceProperty.ContainingType, targetProperty.ContainingType))
         {
            invocation = $"{method.Name}(source, target);";
            return true;
         }

         if (method.IsCallableWith(sourceProperty.Type, targetProperty.ContainingType))
         {
            invocation = $"{method.Name}(source.{sourceProperty.Name}, target);";
            return true;
         }

         invocation = null;
         return false;
      }

      if (method.ReturnType.Equals(targetProperty.Type) && parameterCount == 1)
      {
         var parameter = method.Parameters[0].Type;
         if (parameter.Equals(sourceProperty.Type))
         {
            invocation = $"target.{targetProperty.Name} = {method.Name}(source.{sourceProperty.Name});";
            return true;
         }

         if (parameter.Equals(sourceProperty.ContainingType))
         {
            invocation = $"target.{targetProperty.Name} = {method.Name}(source);";
            return true;
         }
      }

      invocation = null;
      return false;
   }

   private string CreatePropertyMapping(PropertyMappingContext propertyContext, IPropertySymbol sourceProperty, IPropertySymbol targetProperty)
   {
      if (targetProperty.Type.Equals(sourceProperty.Type, SymbolEqualityComparer.Default))
         return $"target.{targetProperty.Name} = source.{sourceProperty.Name};";

      if (TryCreateEnumMapping(propertyContext, sourceProperty, targetProperty, out var enumMapping))
         return enumMapping;

      propertyContext.AddMemberDeclaration(() => CreatePartialMethod(sourceProperty, targetProperty));
      return $"Map{targetProperty.Name}(target, source.{sourceProperty.Name});";
   }

   private bool TryCreateEnumMapping(PropertyMappingContext propertyContext, IPropertySymbol sourceProperty, IPropertySymbol targetProperty, out string enumMapping)
   {
      enumMapping = null;
      if (EnumConverterGenerator.IsEnum(sourceProperty.Type) && EnumConverterGenerator.IsEnum(targetProperty.Type))
      {
         enumMapping = $"target.{targetProperty.Name} = ConvertEnum(source.{sourceProperty.Name});";

         var existingMethod = context.MapperType.FindMethod("ConvertEnum", targetProperty.Type, sourceProperty.Type);
         if (existingMethod == null)
         {
            // We could not find an user defined implementation of the enum conversion method => we have to generate one
            propertyContext.AddMemberDeclaration(() => EnumConverterGenerator.GenerateEnum(sourceProperty.Type, targetProperty.Type, "ConvertEnum"));
         }

         return true;
      }

      return false;
   }

   private string CreatePartialMethod(IPropertySymbol sourceProperty, IPropertySymbol targetProperty)
   {
      var builder = new StringBuilder();
      builder.AppendLine($"/// <summary>Can be implemented to support the mapping of the {targetProperty.Name} property</summary>");
      builder.AppendLine($"partial void Map{targetProperty.Name}({targetProperty.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} target, {sourceProperty.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} value);");
      return builder.ToString();
   }

   private void GenerateExtensionMethod(ClassGenerationContext extensionsClass, INamedTypeSymbol sourceType, INamedTypeSymbol targetType)
   {
      var targetName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      var sourceName = sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      var valueName = sourceType.Name.ToFirstLower();

      var builder = new StringBuilder();
      builder.AppendLine($"{ComputeModifier()} static {targetName} To{targetType.Name}(this {sourceName} {valueName})");
      builder.AppendLine("{");

      builder.AppendLine($"if ({valueName} == null)");
      builder.AppendLine($"throw new global::System.ArgumentNullException(nameof({valueName}));");

      builder.AppendLine($"var result = Mapper is MagicMap.ITypeFactory<{targetName}, {sourceName}> factory");
      builder.AppendLine($"? factory.Create({valueName})");

      var constructor = targetType.GetDefaultConstructor();
      if (constructor == null || constructor.IsPrivate())
      {
         builder.AppendLine($": throw new global::System.NotSupportedException(\"The target type {targetType.Name} can not be created. Provide a accessible parameterless constructor or implement the ITypeFactory interface in the responsible mapper.\");");
      }
      else
      {
         builder.AppendLine($": new {targetName}();");
      }
      builder.AppendLine($"Mapper.Map({valueName}, result);");
      builder.AppendLine("return result;");
      builder.AppendLine("}");

      extensionsClass.AddCode(builder.ToString());
   }

   private string GetSourcePropertyName(IDictionary<string, string> nameMappings, string targetName)
   {
      return nameMappings.TryGetValue(targetName, out var sourceName) ? sourceName : targetName;
   }

   private IDictionary<string, string> InvertMappings(IDictionary<string, string> mappingSpecifications)
   {
      var inverted = new Dictionary<string, string>();
      foreach (var specification in mappingSpecifications)
         inverted[specification.Value] = specification.Key;
      return inverted;
   }

   private bool MappingPossible(IPropertySymbol target)
   {
      if (target.IsReadOnly)
         return false;

      if (target.DeclaredAccessibility == Accessibility.Private)
         return false;

      return true;
   }

   private bool TryFindSourceProperty(IDictionary<string, IPropertySymbol> sourceProperties, string sourceName, out IPropertySymbol source)
   {
      return sourceProperties.TryGetValue(sourceName, out source);
   }

   #endregion


}