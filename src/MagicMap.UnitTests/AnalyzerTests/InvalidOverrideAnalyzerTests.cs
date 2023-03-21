// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidOverrideAnalyzerTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.AnalyzerTests;

using System.Diagnostics.CodeAnalysis;

using MagicMap.Analyzers;

using Microsoft.CodeAnalysis;

[TestClass]
[SuppressMessage("Blocker Code Smell", "S2699:Tests should include assertions")]
public class InvalidOverrideAnalyzerTests : FluentSetupAnalyzerTest<InvalidOverrideAnalyzer>
{
   #region Public Methods and Operators

   [TestMethod]
   public async Task EnsureCorrectDiagnosticForNonStaticDefaultProperty()
   {
      string code = @"using MagicMap;

                      class A { }
                      class B { }

                      [TypeMapper(typeof(A), typeof(B))]
                      public partial class Mapper
                      {
                          public Mapper {|#0:Default|} => new Mapper();
                      }
                   ";

      ExpectDiagnostic(MagicMapDiagnostics.DefaultMapperNotStatic, d => d.WithLocation(0).WithSeverity(DiagnosticSeverity.Warning));
      await RunAsync(code);
   }

   [TestMethod]
   public async Task EnsureNoResultsForEmptyCode()
   {
      await RunAsync("");
   }
   
   #endregion
}