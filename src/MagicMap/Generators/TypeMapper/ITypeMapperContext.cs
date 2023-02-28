// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeMapperData.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators.TypeMapper;

using System.Collections.Concurrent;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

interface ITypeMapperContext : IGeneratorContext
{
    INamedTypeSymbol MapperType { get; }

    INamedTypeSymbol SourceType { get; }

    INamedTypeSymbol TargetType { get; }

    bool SourceEqualsTargetType { get; }

   IDictionary<string, string> MappingSpecifications { get; }

   INamedTypeSymbol FactoryAttribute { get; }
}

internal static class TypeMapperContextExtensions
{
   public static string FullMapperName(this ITypeMapperContext context)
   {
      return context.MapperType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
   }

   public static string MapperName(this ITypeMapperContext context)
   {
      return context.MapperType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
   }
}