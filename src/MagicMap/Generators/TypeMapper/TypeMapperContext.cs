// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators.TypeMapper;

using Microsoft.CodeAnalysis;

internal struct TypeMapperContext : ITypeMapperContext
{
   #region ITypeMapperContext Members

   public INamedTypeSymbol MapperType { get; set; }

   public INamedTypeSymbol SourceType { get; set; }

   public INamedTypeSymbol TargetType { get; set; }

   public bool IsEnabled()
   {
      return MapperType != null;
   }

   #endregion
}