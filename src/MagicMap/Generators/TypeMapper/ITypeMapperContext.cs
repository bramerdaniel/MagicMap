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

   string MapperName { get; }

   IDictionary<string, MappingDescription> MappingSpecifications { get; }

   bool SourceEqualsTargetType { get; }

   INamedTypeSymbol SourceType { get; }
   
   string FullSourceName{ get; }

   string FullTargetName{ get; }

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


   #endregion
}