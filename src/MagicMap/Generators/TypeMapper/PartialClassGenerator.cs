// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartialClassGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators.TypeMapper;

using Microsoft.CodeAnalysis;

internal abstract class PartialClassGenerator
{
   protected INamedTypeSymbol ClassType { get; }

   public PartialClassGenerator(INamedTypeSymbol classType)
   {
      ClassType = classType;
   }




}