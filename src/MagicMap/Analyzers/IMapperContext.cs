// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMapperContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Analyzers;

using Microsoft.CodeAnalysis;

public interface IMapperContext
{
   INamedTypeSymbol TypeMapperClass { get; }

   INamedTypeSymbol TypeMapperAttribute { get; }
   
   AttributeData AttributeData { get; }

   void ReportDiagnostic(DiagnosticDescriptor diagnosticDescriptor, Location location);
}