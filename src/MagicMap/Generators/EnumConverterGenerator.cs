// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumConverterGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators;

using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;

public class EnumConverterGenerator
{
   public static bool IsEnum(ITypeSymbol typeSymbol)
   {
      if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
         return namedTypeSymbol.EnumUnderlyingType != null;
      return false;
   }

   public static string GenerateEnum(ITypeSymbol fromType, ITypeSymbol toType, string methodName)
   {
      var builder = new StringBuilder();
      AppendSignature(methodName, fromType, toType, builder);
      builder.AppendLine("{");
      AppendBody(fromType, toType, builder);
      builder.AppendLine("}");
      return builder.ToString();
   }

   private static void AppendBody(ITypeSymbol fromType, ITypeSymbol toType, StringBuilder builder)
   {
      var sourceMembers = fromType.GetMembers().OfType<IFieldSymbol>().Select(x => x.Name);
      var targetMembers = toType.GetMembers().OfType<IFieldSymbol>().ToDictionary(x => x.Name);

      builder.AppendLine("switch (value)");
      builder.AppendLine("{");
      foreach (var name in sourceMembers)
      {
         if (targetMembers.TryGetValue(name, out var field))
         {
            builder.AppendLine($"case {fromType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{name}:");
            builder.AppendLine($"return {toType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{field.Name};");
         }
      }

      builder.Append("default:");
      builder.Append("throw new global::System.ArgumentException(nameof(value));");
      builder.AppendLine("}");
   }


   private static void AppendSignature(string methodName, ITypeSymbol fromType, ITypeSymbol toType, StringBuilder builder)
   {
      builder.Append("private");
      builder.Append(" ");
      builder.Append(toType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
      builder.Append(" ");
      builder.Append(methodName);
      builder.Append("(");
      builder.Append(fromType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
      builder.Append(" ");
      builder.Append(" value");
      builder.Append(")");
   }
}