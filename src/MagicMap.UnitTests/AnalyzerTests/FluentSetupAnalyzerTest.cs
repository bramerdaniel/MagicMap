// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentSetupAnalyzerTest.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.AnalyzerTests;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp.Testing.MSTest;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

public class FluentSetupAnalyzerTest<T>
   where T : DiagnosticAnalyzer, new()
{
   #region Properties

   protected List<DiagnosticResult> ExpectedDiagnostics { get; set; } = new();

   #endregion

   #region Methods

   protected void ExpectDiagnostic(DiagnosticDescriptor descriptor)
   {
      ExpectedDiagnostics.Add(AnalyzerVerifier<T>.Diagnostic(descriptor));
   }

   protected void ExpectDiagnostic(DiagnosticDescriptor descriptor, Func<DiagnosticResult, DiagnosticResult> setup)
   {
      var diagnostic = setup(AnalyzerVerifier<T>.Diagnostic(descriptor));
      ExpectedDiagnostics.Add(diagnostic);
   }

   protected async Task RunAsync(string code)
   {
      var analyzerTest = new CSharpAnalyzerTest<T, MSTestVerifier>
      {
         TestState =
         {
            Sources = { code }, 
            // AdditionalReferences = { MetadataReference.CreateFromFile(typeof(FluentSetupAttribute).Assembly.Location) },
         }
      };

#if NET6_0
      // I did not know how to ger rid of the compiler error CS1705 
      analyzerTest.CompilerDiagnostics = CompilerDiagnostics.None;
#endif

      analyzerTest.ExpectedDiagnostics.AddRange(ExpectedDiagnostics);

      await analyzerTest.RunAsync();
   }

   #endregion
}