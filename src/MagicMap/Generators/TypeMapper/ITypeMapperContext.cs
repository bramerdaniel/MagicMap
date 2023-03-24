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

   string FullSourceName { get; }

   string FullTargetName { get; }

   INamedTypeSymbol MapperExtensionsType { get; }

   string MapperName { get; }

   INamedTypeSymbol MapperType { get; }

   IDictionary<string, MappingDescription> MappingSpecifications { get; }

   GeneratorMode Mode { get; set; }

   INamedTypeSymbol PropertyMapperAttribute { get; }

   INamedTypeSymbol PropertyMappingAttribute { get; }

   bool SourceEqualsTargetType { get; }

   INamedTypeSymbol SourceType { get; }

   INamedTypeSymbol TargetType { get; }

   #endregion
}

internal static class TypeMapperContextExtensions
{
   #region Public Methods and Operators

   public static string FullMapperName(this ITypeMapperContext context)
   {
      return context.MapperType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
   }

   #endregion
}