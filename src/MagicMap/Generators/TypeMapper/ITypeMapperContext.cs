// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeMapperContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators.TypeMapper;

using System.Collections.Generic;

using Microsoft.CodeAnalysis;

interface ITypeMapperContext : IGeneratorContext
{
   #region Public Properties
   
   INamedTypeSymbol MapperExtensionsType { get; }

   INamedTypeSymbol MapperType { get; }

   IDictionary<string, string> MappingSpecifications { get; }

   bool SourceEqualsTargetType { get; }

   INamedTypeSymbol SourceType { get; }

   INamedTypeSymbol TargetType { get; }

   INamedTypeSymbol PropertyMappingAttribute { get; }

   INamedTypeSymbol PropertyMapperAttribute { get; }

   #endregion
}

internal static class TypeMapperContextExtensions
{
   #region Public Methods and Operators

   public static string FullMapperName(this ITypeMapperContext context)
   {
      return context.MapperType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
   }

   public static string MapperName(this ITypeMapperContext context)
   {
      return context.MapperType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
   }

   #endregion
}