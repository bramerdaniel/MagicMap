// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedSetupClassAnalyzerTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.AnalyzerTests;

using System.Diagnostics.CodeAnalysis;

using MagicMap;
using MagicMap.Analyzers;

using Microsoft.CodeAnalysis;

[TestClass]
[SuppressMessage("Blocker Code Smell", "S2699:Tests should include assertions")]
public class NestedSetupClassAnalyzerTests : FluentSetupAnalyzerTest<NestedSetupClassAnalyzer>
{
   #region Public Methods and Operators

   [TestMethod]
   public async Task EnsureCorrectResultForIgnoredFluentSetupClasses()
   {
      string code = @"using FluentSetups;

                      public class OuterType
                      {
                         [{|#0:FluentSetup|}]
                         public partial class PersonSetup
                         {
                         }
                      }
                   ";

      var descriptor = MagicMapDiagnostics.NotSupportedNestedSetup;
      ExpectDiagnostic(descriptor, d => d.WithLocation(0).WithSeverity(DiagnosticSeverity.Warning));
      await RunAsync(code);
   }

   [TestMethod]
   public async Task EnsureNoResultsForEmptyCode()
   {
      await RunAsync("");
   }

   [TestMethod]
   public async Task EnsureNoResultsForNotFluentSetupClasses()
   {
      string code = @"using FluentSetups;

                      public class OuterType
                      {
                         public partial class PersonSetup
                         {
                         }
                      }
                   ";

      await RunAsync(code);
   }

   #endregion
}