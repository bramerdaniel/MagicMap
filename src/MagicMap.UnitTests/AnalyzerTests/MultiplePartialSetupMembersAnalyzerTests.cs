// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiplePartialSetupMembersAnalyzerTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.AnalyzerTests;

using System.Diagnostics.CodeAnalysis;

using MagicMap.Analyzers;

using Microsoft.CodeAnalysis;

[TestClass]
[SuppressMessage("Blocker Code Smell", "S2699:Tests should include assertions")]
public class MultiplePartialSetupMembersAnalyzerTests : FluentSetupAnalyzerTest<MultiplePartialSetupMembersAnalyzer>
{
   #region Public Methods and Operators

   [TestMethod]
   public async Task EnsureCorrectResultForIgnoredFluentSetupClasses()
   {
      string code = @"
                         using FluentSetups;

                         [{|#0:FluentSetup|}]
                         public partial class PersonSetup
                         {
                         }

                         public partial class PersonSetup
                         {
                         }
                      ";

      var descriptor = new MultiplePartialSetupMembersAnalyzer().SupportedDiagnostics.First();

      ExpectDiagnostic(descriptor, d => d.WithLocation(0).WithArguments("PersonSetup").WithSeverity(DiagnosticSeverity.Info));
      await RunAsync(code);
   }

   [TestMethod]
   public async Task EnsureNoResultsForEmptyCode()
   {
      await RunAsync("");
   }

   [TestMethod]
   public async Task EnsureNoResultsForGeneratedFluentSetupClasses()
   {
      string code = @"
                         using FluentSetups;

                         [FluentSetup]
                         public partial class PersonSetup
                         {
                         }
                      ";

      await RunAsync(code);
   }

   [TestMethod]
   public async Task EnsureNoResultsForNotFluentSetupClasses()
   {
      string code = @"
                         using FluentSetups;

                         public partial class PersonSetup
                         {
                         }

                         public partial class PersonSetup
                         {
                         }
                      ";

      await RunAsync(code);
   }

   #endregion
}