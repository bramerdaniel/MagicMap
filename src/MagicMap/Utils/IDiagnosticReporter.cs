// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiagnosticReporter.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace MagicMap.Utils;

internal interface IDiagnosticReporter
{
   void AddDiagnostic(DiagnosticDescriptor diagnosticDescriptor);
}