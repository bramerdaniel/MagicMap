// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

using Microsoft.CodeAnalysis;

internal struct TypeMapperContext : ITypeMapperContext
{
   public bool IsEnabled => true;

   public INamedTypeSymbol MapperType { get; set; }

   public INamedTypeSymbol LeftType { get; set; }

   public INamedTypeSymbol RightType { get; set; }

   bool IGeneratorContext.IsEnabled()
   {
      return MapperType != null;
   }
}