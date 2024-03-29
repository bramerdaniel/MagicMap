﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators.TypeMapper;

using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

internal class TypeMapperContext : ITypeMapperContext
{
   #region Constants and Fields

   private readonly string mapperClassName;

   #endregion

   #region Constructors and Destructors

   public TypeMapperContext(INamedTypeSymbol mapperType)
   {
      MapperType = mapperType ?? throw new ArgumentNullException(nameof(mapperType));
   }

   public TypeMapperContext(string mapperClassName)
   {
      this.mapperClassName = mapperClassName ?? throw new ArgumentNullException(nameof(mapperClassName));
   }

   #endregion

   #region ITypeMapperContext Members

   public INamedTypeSymbol MapperType { get; }

   public string MapperName => mapperClassName ?? MapperType.Name;

   public INamedTypeSymbol SourceType { get; set; }

   public string FullSourceName => SourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

   public string FullTargetName => TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

   public INamedTypeSymbol TargetType { get; set; }

   public bool SourceEqualsTargetType => SourceType.Equals(TargetType, SymbolEqualityComparer.Default);

   public IDictionary<string, MappingDescription> MappingSpecifications { get; set; }

   public string TargetName => MapperType?.Name ?? "TypeMapper";

   public INamedTypeSymbol MapperExtensionsType { get; set; }

   public INamedTypeSymbol PropertyMappingAttribute { get; set; }

   public INamedTypeSymbol PropertyMapperAttribute { get; set; }

   public GeneratorMode Mode { get; set; }

   public bool ForceMappings { get; set; }

   public bool IsEnabled()
   {
      return MapperType != null;
   }

   #endregion
}