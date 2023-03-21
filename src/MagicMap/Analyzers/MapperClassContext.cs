// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapperClassContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Analyzers;

using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

public struct MapperClassContext : IMapperContext
{
   private readonly SymbolAnalysisContext context;

   public INamedTypeSymbol TypeMapperClass { get; }

   public INamedTypeSymbol TypeMapperAttribute { get; }

   public IList<Diagnostic> Dignostics { get; } = new List<Diagnostic>();

   public MapperClassContext(INamedTypeSymbol typeMapperClass, INamedTypeSymbol typeMapperAttribute, SymbolAnalysisContext context)
   {
      TypeMapperClass = typeMapperClass ?? throw new ArgumentNullException(nameof(typeMapperClass));
      TypeMapperAttribute = typeMapperAttribute ?? throw new ArgumentNullException(nameof(typeMapperAttribute));
      this.context = context;
   }

   public void ReportDiagnostic(DiagnosticDescriptor diagnosticDescriptor, Location location)
   {
      Dignostics.Add(Diagnostic.Create(diagnosticDescriptor, location));
   }
}

public interface IMapperContext
{
   INamedTypeSymbol TypeMapperClass { get; }

   INamedTypeSymbol TypeMapperAttribute { get; }

   void ReportDiagnostic(DiagnosticDescriptor diagnosticDescriptor, Location location);
}