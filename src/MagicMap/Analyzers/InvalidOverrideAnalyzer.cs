// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidOverrideAnalyzer.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Analyzers;

using System.Collections.Immutable;

using MagicMap.Extensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class InvalidOverrideAnalyzer : TypeMapperAnalyzer
{
   #region Public Properties

   /// <summary>Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.</summary>
   public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
      ImmutableArray.Create(MagicMapDiagnostics.DefaultMapperNotStatic);

   #endregion


   protected override void AnalyzeSymbol(IMapperContext context)
   {
      var defaultMapperProperty = context.TypeMapperClass.GetProperty("Default");
      if (defaultMapperProperty == null)
         return;

      if (defaultMapperProperty.IsStatic)
         return;

      context.ReportDiagnostic(MagicMapDiagnostics.DefaultMapperNotStatic, FindLocation(defaultMapperProperty));
   }
}