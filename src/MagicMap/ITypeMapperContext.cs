// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeMapperData.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

using Microsoft.CodeAnalysis;

interface ITypeMapperContext : IGeneratorContext
{
   INamedTypeSymbol MapperType { get; }

   INamedTypeSymbol LeftType { get; }

   INamedTypeSymbol RightType { get; }
}