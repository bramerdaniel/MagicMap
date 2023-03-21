// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MagicMapDiagnostics.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using System.Diagnostics.CodeAnalysis;

   using Microsoft.CodeAnalysis;

   [SuppressMessage("MicrosoftCodeAnalysisReleaseTracking", "RS2008:Enable analyzer release tracking")]
   public static class MagicMapDiagnostics
   {
      #region Constants and Fields
      
      internal static readonly DiagnosticDescriptor NotSupported = new DiagnosticDescriptor(id: "MMW0002",
         title: "MagicMap source generator",
         messageFormat: "This is not supported yet",
         category: "MagicMap",
         defaultSeverity: DiagnosticSeverity.Warning,
         isEnabledByDefault: true);

      internal static readonly DiagnosticDescriptor NotSupportedNestedSetup = new DiagnosticDescriptor(id: "MMW0001",
         title: "MagicMap source generator",
         messageFormat: "Mapper generation for nested classes is not supported",
         category: "MagicMap",
         defaultSeverity: DiagnosticSeverity.Warning,
         isEnabledByDefault: true);

      #endregion
   }
}