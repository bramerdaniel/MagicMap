// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceGeneratorTestSetup.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.Setups;

using Microsoft.CodeAnalysis.CSharp;

internal class SourceGeneratorTestSetup : SetupBase
{
   #region Constants and Fields

   private LanguageVersion languageVersion = LanguageVersion.LatestMajor;

   #endregion

   #region Public Methods and Operators

   public GenerationResult Done()
   {
      var compilation = CreateCompilation();

      var generator = new MagicMapSourceGenerator();
      var driver = CSharpGeneratorDriver.Create(generator)
         .WithUpdatedParseOptions(new CSharpParseOptions(languageVersion));

      driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatedDiagnostics);

      return new GenerationResult
      {
         InputCompilation = compilation, OutputCompilation = outputCompilation, GeneratedDiagnostics = generatedDiagnostics,
      };
   }

   public SourceGeneratorTestSetup WithLanguageLevel(LanguageVersion version)
   {
      languageVersion = version;
      return this;
   }

   public SourceGeneratorTestSetup WithRootNamespace(string value)
   {
      RootNamespace = value;
      return this;
   }

   public SourceGeneratorTestSetup WithSource(string code)
   {
      AddSource(code, languageVersion);
      return this;
   }

   #endregion
}