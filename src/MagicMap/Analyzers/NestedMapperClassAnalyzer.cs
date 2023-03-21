// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedMapperClassAnalyzer.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Analyzers
{
   using System.Collections.Immutable;

   using Microsoft.CodeAnalysis;
   using Microsoft.CodeAnalysis.Diagnostics;

   [DiagnosticAnalyzer(LanguageNames.CSharp)]
   public class NestedMapperClassAnalyzer : TypeMapperAnalyzer
   {
      #region Public Properties

      /// <summary>Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.</summary>
      public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
         ImmutableArray.Create(MagicMapDiagnostics.NestedMapperNotSupported);

      #endregion

      #region Methods

      protected override void AnalyzeSymbol(IMapperContext context)
      {
         if (context.TypeMapperClass.ContainingType == null)
         {
            // we only care about nested classes here
            return;
         }

         context.ReportDiagnostic(MagicMapDiagnostics.NestedMapperNotSupported, FindLocation(context.TypeMapperAttribute));
      }

      #endregion
   }
}