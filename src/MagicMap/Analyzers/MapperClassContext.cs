// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapperClassContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Analyzers;

using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

public struct MapperClassContext : IMapperContext
{
   private readonly SymbolAnalysisContext context;

   public INamedTypeSymbol TypeMapperClass { get; }

   public INamedTypeSymbol TypeMapperAttribute { get; }

   public AttributeData AttributeData { get; }

   public IList<Diagnostic> Diagnostics { get; } = new List<Diagnostic>();

   public MapperClassContext(INamedTypeSymbol typeMapperClass, INamedTypeSymbol typeMapperAttribute, AttributeData attributeData,
      SymbolAnalysisContext context)
   {
      TypeMapperClass = typeMapperClass ?? throw new ArgumentNullException(nameof(typeMapperClass));
      TypeMapperAttribute = typeMapperAttribute ?? throw new ArgumentNullException(nameof(typeMapperAttribute));
      AttributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));
      this.context = context;
   }

   public void ReportDiagnostic(DiagnosticDescriptor diagnosticDescriptor, Location location)
   {
      Diagnostics.Add(Diagnostic.Create(diagnosticDescriptor, location));
   }
}