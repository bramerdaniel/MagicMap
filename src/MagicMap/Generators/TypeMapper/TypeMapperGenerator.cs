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
internal class TypeMapperGenerator : IGenerator
{
   #region Constants and Fields

   private readonly ITypeMapperContext context;

   private readonly string fileName;

   #endregion

   #region Constructors and Destructors

   public TypeMapperGenerator(ITypeMapperContext context, string fileName)
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
      builder.AppendLine($"private static {MapperTypeName} mapper = new {MapperTypeName}();");

      GenerateExtensionMethod(builder, context.TargetType, context.SourceType);

      if (!context.SourceEqualsTargetType)
         GenerateExtensionMethod(builder, context.SourceType, context.TargetType);

      builder.AppendLine("}");
   }

   private void GenerateMapperClass(StringBuilder builder)
   {
      builder.AppendLine($"partial class {MapperTypeName}");
      builder.AppendLine("{");

      var propertyContext = new PropertyMappingContext(context.SourceType, context.TargetType, InvertMappings(context.MappingSpecifications));
      GenerateMappingMethod(builder, propertyContext);
      
      if (!context.SourceEqualsTargetType)
      {
         propertyContext = new PropertyMappingContext(context.TargetType, context.SourceType, context.MappingSpecifications);
         GenerateMappingMethod(builder, propertyContext);
      }

      builder.AppendLine("}");
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
               if (!targetProperty.Type.Equals(sourceProperty.Type, SymbolEqualityComparer.Default))
               {
                  builder.Append($"Map{targetProperty.Name}(target, source.{sourceProperty.Name});");
                  propertyContext.AddPartialDeclaration($"/// <summary>Can be implemented to support the mapping of the {targetProperty.Name} property</summary>");
                  propertyContext.AddPartialDeclaration($"partial void Map{targetProperty.Name}({targetProperty.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} target, {sourceProperty.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} value);");
               }
               else
               {
                  builder.AppendLine($"target.{targetProperty.Name} = source.{sourceProperty.Name};");
               }
            }
         }
      }

      builder.AppendLine($"MapOverride(source, target);");
      builder.AppendLine("}");

      propertyContext.AddPartialDeclaration("/// <summary>Implement this method to map properties the mapper could not handle for any reason</summary>");
      builder.AppendLine($"partial void MapOverride({propertyContext.SourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} source, {propertyContext.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} target);");

      foreach (var declaration in propertyContext.PartialDeclarations)
         builder.AppendLine(declaration);
   }

   private void GenerateExtensionMethod(StringBuilder builder, INamedTypeSymbol sourceType,  INamedTypeSymbol targetType)
   {
      var targetName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      var sourceName = sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      var valueName = sourceType.Name.ToFirstLower();

      builder.AppendLine($"{ComputeModifier()} static {targetName} To{targetType.Name}(this {sourceName} {valueName})");
      builder.AppendLine("{");

      builder.AppendLine($"if ({valueName} == null)");
      builder.AppendLine($"throw new global::System.ArgumentNullException(nameof({valueName}));");
      builder.AppendLine($"var result = new {targetName}();");
      builder.AppendLine($"mapper.Map({valueName}, result);");
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