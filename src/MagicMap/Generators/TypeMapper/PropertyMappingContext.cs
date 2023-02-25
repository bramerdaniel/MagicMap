// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyMappingContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators.TypeMapper;

using System.Collections.Generic;

using Microsoft.CodeAnalysis;

internal class PropertyMappingContext
{
   public INamedTypeSymbol SourceType { get; }

   public INamedTypeSymbol TargetType { get; }

   public IDictionary<string, string> PropertyMappings { get; }

   public PropertyMappingContext(INamedTypeSymbol sourceType, INamedTypeSymbol targetType, IDictionary<string, string> propertyMappings)
   {
      SourceType = sourceType;
      TargetType = targetType;
      PropertyMappings = propertyMappings;
   }
}