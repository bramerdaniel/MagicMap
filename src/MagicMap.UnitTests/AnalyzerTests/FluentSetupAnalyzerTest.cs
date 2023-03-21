// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentSetupAnalyzerTest.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.AnalyzerTests;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

   protected virtual CSharpCompilation CreateCompilation(string code)
   {
      var syntaxTree = CSharpSyntaxTree.ParseText(code);
      return CSharpCompilation.Create("UnitTest", new List<SyntaxTree>{ syntaxTree },
         null, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
   }

   protected async Task RunAsync(string code)
   {

      var analyzerTest = new CSharpAnalyzerTest<T, MSTestVerifier>();
      analyzerTest.TestState.Sources.Add(code);

      var compilation = CreateCompilation(code);
      var generator = new MagicMapSourceGenerator();
      var driver = CSharpGeneratorDriver.Create(generator);
      // driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatedDiagnostics);
      var generatorDriver = driver.RunGenerators(compilation);
      var generatorDriverRunResult = generatorDriver.GetRunResult();
      foreach (var syntaxTree in generatorDriverRunResult.GeneratedTrees)
      {
         string syntaxTreeFilePath = syntaxTree.FilePath;
         analyzerTest.TestState.Sources.Add((syntaxTreeFilePath, syntaxTree.ToString()));
         //analyzerTest.TestState.GeneratedSources.Add((syntaxTreeFilePath, syntaxTree.ToString()));
      }

#if NET6_0
      // I did not know how to get rid of the compiler error CS1705 
      analyzerTest.CompilerDiagnostics = CompilerDiagnostics.None;
#endif

      analyzerTest.ExpectedDiagnostics.AddRange(ExpectedDiagnostics);

      await analyzerTest.RunAsync();
   }

   #endregion
}