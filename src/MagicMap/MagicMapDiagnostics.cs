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

      internal static readonly DiagnosticDescriptor NestedMapperNotSupported = new DiagnosticDescriptor(id: "MMW0001",
         title: "MagicMap source generator",
         messageFormat: "Mapper generation for nested classes is not supported",
         category: "MagicMap",
         defaultSeverity: DiagnosticSeverity.Warning,
         isEnabledByDefault: true);


      internal static readonly DiagnosticDescriptor NothingToMap = new DiagnosticDescriptor(id: "MMW0003",
         title: "MagicMap source generator",
         messageFormat: "The generated mapper would not map any properties",
         category: "MagicMap",
         defaultSeverity: DiagnosticSeverity.Warning,
         isEnabledByDefault: true);

      internal static readonly DiagnosticDescriptor DefaultMapperNotStatic = new DiagnosticDescriptor(id: "MMW0004",
         title: "MagicMap source generator",
         messageFormat: "The default mapper should be static and accessible from the generated extension methods.",
         category: "MagicMap",
         defaultSeverity: DiagnosticSeverity.Warning,
         isEnabledByDefault: true);

      #endregion
   }
}