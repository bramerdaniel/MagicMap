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

using Microsoft.CodeAnalysis;

internal class TypeMapperGenerator : IGenerator
{
   #region Constants and Fields

   private readonly ITypeMapperContext context;

   private string mapperTypeName => context.MapperType.Name;

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

      var generatedSource = new GeneratedSource { Name = mapperTypeName + ".generated.cs", Code = builder.ToString() };

      return generatedSource;
   }

   #endregion

   #region Properties

   private string SourceTypeName => context.SourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

   private string TargetTypeName => context.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

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

   private void AppendMapperSignature(StringBuilder builder)
   {
      builder.Append("public void Map(");
      builder.Append(context.SourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
      builder.Append(" source");
      builder.Append(", ");
      builder.Append(context.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
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
      builder.AppendLine($"{ComputeModifier()} static partial class {mapperTypeName}Extensions");
      builder.AppendLine("{");
      builder.AppendLine($"private static {mapperTypeName} mapper = new {mapperTypeName}();");
      GenerateToTargetMethod(builder);
      GenerateToSourceMethod(builder);

      builder.AppendLine("}");
   }

   private void GenerateToSourceMethod(StringBuilder builder)
   {
      builder.AppendLine($"{ComputeModifier()} static {SourceTypeName} To{context.SourceType.Name}(this {TargetTypeName} value)");
      builder.AppendLine("{");
      builder.AppendLine("throw new global::System.NotImplementedException();");
      builder.AppendLine("}");

   }

   private void GenerateMapperClass(StringBuilder builder)
   {
      builder.AppendLine($"partial class {mapperTypeName}");
      builder.AppendLine("{");
      var unmapped = GenerateMappingMethod(builder).ToArray();
      GeneratePartialMappers(builder, unmapped);
      builder.AppendLine("}");
   }

   private IEnumerable<(IPropertySymbol source, IPropertySymbol target)> GenerateMappingMethod(StringBuilder builder)
   {
      var targetProperties = context.TargetType.GetMembers()
         .OfType<IPropertySymbol>()
         .ToDictionary(p => p.Name, StringComparer.InvariantCultureIgnoreCase);

      AppendMapperSignature(builder);
      builder.AppendLine("{");

      if (targetProperties.Count == 0)
      {
         builder.AppendLine("// target type does not contain any properties.");
         builder.AppendLine("// No mappings were generated");
      }
      else
      {
         foreach (var source in context.SourceType.GetMembers().OfType<IPropertySymbol>())
         {
            if (targetProperties.TryGetValue(source.Name, out var target))
            {
               if (!target.Type.Equals(source.Type, SymbolEqualityComparer.Default))
               {
                  builder.AppendLine("// types do not match");
                  builder.Append($"Map{source.Name}(target, source.{source.Name});");
                  yield return (source, target);
               }
               else
               {
                  builder.AppendLine($"target.{source.Name} = source.{target.Name};");
               }
            }
         }
      }

      builder.AppendLine("}");
   }

   private void GeneratePartialMappers(StringBuilder builder, (IPropertySymbol source, IPropertySymbol target)[] unmapped)
   {
      foreach (var tuple in unmapped)
      {
         builder.Append(
            $"partial void Map{tuple.target.Name}({context.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} target, {tuple.source.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} value);");
      }
   }

   private void GenerateToTargetMethod(StringBuilder builder)
   {
      builder.AppendLine($"{ComputeModifier()} static {TargetTypeName} To{context.TargetType.Name}(this {SourceTypeName} value)");
      builder.AppendLine("{");
      builder.AppendLine($"var result = new {TargetTypeName}();");
      builder.AppendLine("mapper.Map(value, result);");
      builder.AppendLine("return result;");
      builder.AppendLine("}");
   }

   #endregion
}