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
using MagicMap.Generators;
using Microsoft.CodeAnalysis;

/// <summary>Generator for type mapping</summary>
/// <seealso cref="MagicMap.Generators.IGenerator" />
internal class TypeMapperGenerator : PartialClassGenerator, IGenerator
{
   #region Constants and Fields

   private readonly ITypeMapperContext context;

   private readonly string fileName;

   #endregion

   #region Constructors and Destructors

   public TypeMapperGenerator(ITypeMapperContext context, string fileName)
      : base(context.MapperType)
   {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
      this.fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
   }

   #endregion

   #region IGenerator Members

   public GeneratedSource Generate()
   {
      var builder = new StringBuilder();
      if (!context.MapperType.ContainingNamespace.IsGlobalNamespace)
      {
         builder.AppendLine($"namespace {context.MapperType.ContainingNamespace.ToDisplayString()}");
         builder.AppendLine("{");
      }

#if DEBUG
      if (MapperTypeName.StartsWith("Throw", StringComparison.InvariantCultureIgnoreCase))
         throw new InvalidOperationException("Throw is not supported here as mapper type name.");
#endif

      GenerateMapperClass(builder);
      GenerateExtensionsClass(builder);

      if (!context.MapperType.ContainingNamespace.IsGlobalNamespace)
         builder.AppendLine("}");

      var generatedSource = new GeneratedSource
      {
         Name = fileName,
         Code = builder.ToString()
      };

      return generatedSource;
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

   private void AppendMapperSignature(StringBuilder builder, INamedTypeSymbol fromType, INamedTypeSymbol toType)
   {
      builder.AppendLine("/// <summary>");
      builder.AppendLine("/// Maps all properties of the <see cref=\"source\"/> to the properties of the <see cref=\"target\"/>");
      builder.AppendLine("/// </summary>");
      builder.Append("public void Map(");
      builder.Append(fromType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
      builder.Append(" source");
      builder.Append(", ");
      builder.Append(toType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
      builder.Append(" target");
      builder.AppendLine(")");
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

   private void GenerateExtensionsClass(StringBuilder builder)
   {
      builder.AppendLine($"{ComputeModifier()} static partial class {MapperTypeName}Extensions");
      builder.AppendLine("{");
      GenerateMapperProperty(builder);
      GenerateExtensionMethod(builder, context.TargetType, context.SourceType);

      if (!context.SourceEqualsTargetType)
         GenerateExtensionMethod(builder, context.SourceType, context.TargetType);

      builder.AppendLine("}");
   }

   private void GenerateMapperProperty(StringBuilder builder)
   {
      var property = context.MapperExtensionsType?.GetProperty(p => p.Name == "Mapper");
      if (property != null)
         return;

      builder.AppendLine("/// <summary>");
      builder.AppendLine($"/// The instance of the <see cref=\"{MapperTypeName}\"/> all extension methods use.");
      builder.AppendLine($"/// You can customize this by implementing this property in your own partial implementation of the extensions class.");
      builder.AppendLine("/// </summary>");
      builder.AppendLine($"private static {context.MapperType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} Mapper => {context.MapperType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.Default;");
   }

   private void GenerateMapperClass(StringBuilder builder)
   {
      builder.AppendLine($"partial class {MapperTypeName}");
      builder.AppendLine("{");

      GeneratedSingletonInstance(builder);

      var propertyContext = new PropertyMappingContext(context.SourceType, context.TargetType, InvertMappings(context.MappingSpecifications));
      GenerateMappingMethod(builder, propertyContext);

      if (!context.SourceEqualsTargetType)
      {
         propertyContext = new PropertyMappingContext(context.TargetType, context.SourceType, context.MappingSpecifications);
         GenerateMappingMethod(builder, propertyContext);
      }

      GenerateOverrides(builder);
      builder.AppendLine("}");
   }

   private void GenerateOverrides(StringBuilder builder)
   {
      builder.AppendLine("/// <summary>Implement this method to map properties the mapper could not handle for any reason</summary>");
      builder.AppendLine($"partial void MapOverride({context.SourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} source, {context.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} target);");

      if (!context.SourceEqualsTargetType)
      {
         builder.AppendLine("/// <summary>Implement this method to map properties the mapper could not handle for any reason</summary>");
         builder.AppendLine($"partial void MapOverride({context.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} source, {context.SourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} target);");
      }
   }

   private void GeneratedSingletonInstance(StringBuilder builder)
   {
      var method = context.MapperType.GetMethod(IsDefaultMapperFactory);
      if (method != null)
      {
         builder.AppendLine("/// <summary>The default singleton instance of the generated type mapper.</summary>");
         builder.AppendLine($"public static {context.MapperType.Name} Default {{ get; private set; }} = {method.Name}();");
      }
      else
      {
         var defaultConstructor = context.MapperType.Constructors.FirstOrDefault(c => c.Parameters.Length == 0);
         if (defaultConstructor != null)
         {
            builder.AppendLine("/// <summary>");
            builder.AppendLine("/// The default singleton instance of the generated type mapper.");
            builder.AppendLine("/// To customize the creation of your default mapper, just create a static method, and mark it with the <see cref=\"MagicMap.MapperFactoryAttribute\"/>");
            builder.AppendLine("/// </summary>");
            builder.AppendLine("/// <code>");
            builder.AppendLine("/// [MapperFactory]");
            builder.AppendLine("/// static Mapper CreateMapper() => new Mapper();");
            builder.AppendLine("/// </code>");
            builder.AppendLine($"public static {context.MapperName()} Default {{ get; private set; }} = new {context.MapperName()}();");
            builder.AppendLine();
         }
      }
   }

   private bool IsDefaultMapperFactory(IMethodSymbol m)
   {
      if (!m.IsStatic || m.Parameters.Length > 0)
         return false;

      if (!m.ReturnType.Equals(context.MapperType, SymbolEqualityComparer.Default))
         return false;

      return HasFactoryAttribute(m);
   }

   private bool HasFactoryAttribute(IMethodSymbol methodSymbol)
   {
      return methodSymbol.GetAttributes()
         .Any(a => context.FactoryAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default));
   }

   private void GenerateMappingMethod(StringBuilder builder, PropertyMappingContext propertyContext)
   {
      var targetProperties = propertyContext.TargetType.GetMembers()
         .OfType<IPropertySymbol>()
         .ToDictionary(p => p.Name, StringComparer.InvariantCultureIgnoreCase);

      var sourceProperties = propertyContext.SourceType.GetMembers()
         .OfType<IPropertySymbol>()
         .ToDictionary(p => p.Name, StringComparer.InvariantCultureIgnoreCase);

      AppendMapperSignature(builder, propertyContext.SourceType, propertyContext.TargetType);
      builder.AppendLine("{");

      if (targetProperties.Count == 0)
      {
         builder.AppendLine("// target type does not contain any properties.");
         builder.AppendLine("// No mappings were generated");
      }
      else
      {
         foreach (var targetProperty in targetProperties.Values.Where(MappingPossible))
         {
            var sourcePropertyName = GetSourcePropertyName(propertyContext.PropertyMappings, targetProperty.Name);
            if (TryFindSourceProperty(sourceProperties, sourcePropertyName, out var sourceProperty))
            {
               var mapping = CreatePropertyMapping(propertyContext, sourceProperty, targetProperty);
               builder.AppendLine(mapping);
            }
         }
      }

      builder.AppendLine($"MapOverride(source, target);");
      builder.AppendLine("}");

      foreach (var declaration in propertyContext.MemberDeclarations)
         builder.AppendLine(declaration());
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

   private void GenerateExtensionMethod(StringBuilder builder, INamedTypeSymbol sourceType, INamedTypeSymbol targetType)
   {
      var targetName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      var sourceName = sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      var valueName = sourceType.Name.ToFirstLower();

      builder.AppendLine($"{ComputeModifier()} static {targetName} To{targetType.Name}(this {sourceName} {valueName})");
      builder.AppendLine("{");

      builder.AppendLine($"if ({valueName} == null)");
      builder.AppendLine($"throw new global::System.ArgumentNullException(nameof({valueName}));");
      builder.AppendLine($"var result = new {targetName}();");
      builder.AppendLine($"Mapper.Map({valueName}, result);");
      builder.AppendLine("return result;");
      builder.AppendLine("}");
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

   private bool TryFindSourceProperty(Dictionary<string, IPropertySymbol> sourceProperties, string sourceName, out IPropertySymbol source)
   {
      return sourceProperties.TryGetValue(sourceName, out source);
   }

   #endregion
}