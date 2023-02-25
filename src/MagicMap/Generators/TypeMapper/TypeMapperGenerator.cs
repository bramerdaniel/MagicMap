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

   #endregion

   #region Constructors and Destructors

   public TypeMapperGenerator(ITypeMapperContext context)
   {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
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

      var generatedSource = new GeneratedSource { Name = $"{MapperTypeName}.generated.cs", Code = builder.ToString() };

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

      var unmappedToRight = GenerateMappingMethod(builder, context.SourceType, context.TargetType, InvertMappings(context.MappingSpecifications)).ToArray();

      (IPropertySymbol source, IPropertySymbol target)[] unmappedToLeft = null;
      if (!context.SourceEqualsTargetType)
         unmappedToLeft = GenerateMappingMethod(builder, context.TargetType, context.SourceType, context.MappingSpecifications).ToArray();

      GeneratePartialMappers(builder, unmappedToRight);
      if (unmappedToLeft != null)
         GeneratePartialMappers(builder, unmappedToLeft);

      builder.AppendLine("}");
   }

   private IEnumerable<(IPropertySymbol source, IPropertySymbol target)> GenerateMappingMethod(StringBuilder builder, INamedTypeSymbol fromType,
      INamedTypeSymbol toType, IDictionary<string, string> nameMappings)
   {
      var targetProperties = toType.GetMembers()
         .OfType<IPropertySymbol>()
         .ToDictionary(p => p.Name, StringComparer.InvariantCultureIgnoreCase);

      var sourceProperties = fromType.GetMembers()
         .OfType<IPropertySymbol>()
         .ToDictionary(p => p.Name, StringComparer.InvariantCultureIgnoreCase);

      AppendMapperSignature(builder, fromType, toType);
      builder.AppendLine("{");

      if (targetProperties.Count == 0)
      {
         builder.AppendLine("// target type does not contain any properties.");
         builder.AppendLine("// No mappings were generated");
      }
      else
      {
         foreach (var target in targetProperties.Values.Where(MappingPossible))
         {
            var sourcePropertyName = GetSourcePropertyName(nameMappings, target.Name);
            if (TryFindSourceProperty(sourceProperties, sourcePropertyName, out var source))
            {
               if (!target.Type.Equals(source.Type, SymbolEqualityComparer.Default))
               {
                  builder.Append($"Map{source.Name}(target, source.{source.Name});");
                  yield return (source, target);
               }
               else
               {
                  builder.AppendLine($"target.{target.Name} = source.{source.Name};");
               }
            }
         }
      }

      builder.AppendLine("}");
   }

   /// <summary>Generates the partial mappers.</summary>
   /// <param name="builder">The builder.</param>
   /// <param name="unmapped">The unmapped.</param>
   private void GeneratePartialMappers(StringBuilder builder, (IPropertySymbol source, IPropertySymbol target)[] unmapped)
   {
      foreach (var tuple in unmapped)
      {
         var targetType = tuple.target.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
         var sourceValueType = tuple.source.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

         builder.AppendLine($"/// <summary>Can be implemented to support the mapping of the {tuple.target.Name} property</summary>");
         builder.AppendLine($"partial void Map{tuple.target.Name}({targetType} target, {sourceValueType} value);");
         builder.AppendLine();
      }
   }

   private void GenerateExtensionMethod(StringBuilder builder, INamedTypeSymbol sourceType,  INamedTypeSymbol targetType)
   {
      var targetName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      var sourceName = sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

      builder.AppendLine($"{ComputeModifier()} static {targetName} To{targetType.Name}(this {sourceName} value)");
      builder.AppendLine("{");
      builder.AppendLine($"var result = new {targetName}();");
      builder.AppendLine("mapper.Map(value, result);");
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