// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceGeneratorTestSetup.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.Setups;

using MagicMap;

using Microsoft.CodeAnalysis.CSharp;

internal class SourceGeneratorTestSetup : SetupBase
{
   #region Public Methods and Operators

   public GenerationResult Done()
   {
      var compilation = CreateCompilation();

      var generator = new MagicMapSourceGenerator();
      var driver = CSharpGeneratorDriver.Create(generator);
      driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatedDiagnostics);

      return new GenerationResult
      {
         InputCompilation = compilation,
         OutputCompilation = outputCompilation,
         GeneratedDiagnostics = generatedDiagnostics,
      };
   }

   public SourceGeneratorTestSetup WithRootNamespace(string value)
   {
      RootNamespace = value;
      return this;
   }

   public SourceGeneratorTestSetup WithSource(string code)
   {
      AddSource(code);
      return this;
   }

   #endregion
}