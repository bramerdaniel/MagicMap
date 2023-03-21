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
      
      public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
         ImmutableArray.Create(MagicMapDiagnostics.NestedMapperNotSupported);

      #endregion

      #region Methods

      protected override void AnalyzeSymbol(IMapperContext context)
      {
         if (context.TypeMapperClass.ContainingType != null)
            context.ReportDiagnostic(MagicMapDiagnostics.NestedMapperNotSupported, FindLocation(context.AttributeData));
      }

      #endregion
   }
}