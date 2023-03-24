// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerationResultAssertion.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.Assertions
{
   using System.Collections.Immutable;
   using System.Text;

   using FluentAssertions;
   using FluentAssertions.Primitives;

   using MagicMap.UnitTests.Setups;

   using Microsoft.CodeAnalysis;

   internal class GenerationResultAssertion : ReferenceTypeAssertions<GenerationResult, GenerationResultAssertion>
   {
      #region Constructors and Destructors

      public GenerationResultAssertion(GenerationResult subject)
         : base(subject)
      {
      }

      #endregion

      #region Properties

      protected override string Identifier => nameof(GenerationResultAssertion);

      #endregion

      #region Public Methods and Operators

      public ClassAssertion HaveClass(string className)
      {
         var classType = Subject.OutputCompilation.GetTypeByMetadataName(className);

         Assert.IsNotNull(classType, $"The class {className} could not be found. {Environment.NewLine}{Subject.OutputSyntaxTrees.Last()}");
         return new ClassAssertion(Subject, classType);
      }

      public AndConstraint<GenerationResultAssertion> HaveDiagnostic(string diagnosticId, DiagnosticSeverity severity)
      {
         var diagnostic = FindDiagnostic(diagnosticId);

         Assert.IsNotNull(diagnostic, $"Diagnostic {diagnostic} could not be found.{CreateAllFound(Subject.GeneratedDiagnostics)}");

         if (diagnostic.Severity != severity)
            Assert.Fail($"Diagnostic {diagnostic} did not have the expected severity {severity}.");

         return new AndConstraint<GenerationResultAssertion>(this);
      }

      public AndConstraint<GenerationResultAssertion> HaveDiagnostic(string diagnosticId)
      {
         var diagnostic = Subject.GeneratedDiagnostics.FirstOrDefault(d => d.Id == diagnosticId);
         if (diagnostic == null)
         {
            diagnostic = Subject.OutputCompilation.GetDiagnostics()
               .FirstOrDefault(d => d.Id == diagnosticId);
         }

         Assert.IsNotNull(diagnostic, $"Diagnostic {diagnostic} could not be found.{CreateAllFound(Subject.GeneratedDiagnostics)}");

         return new AndConstraint<GenerationResultAssertion>(this);
      }

      public AndConstraint<GenerationResultAssertion> HaveError(string diagnosticId)
      {
         return HaveDiagnostic(diagnosticId, DiagnosticSeverity.Error);
      }

      public ClassAssertion HaveInterface(string interfaceName)
      {
         var classType = Subject.OutputCompilation.GetTypeByMetadataName(interfaceName);

         Assert.IsNotNull(classType, $"The class {interfaceName} could not be found. {Environment.NewLine}{Subject.OutputSyntaxTrees.Last()}");
         return new ClassAssertion(Subject, classType);
      }

      public AndConstraint<GenerationResultAssertion> NotHaveClass(string className)
      {
         var classType = Subject.OutputCompilation.GetTypeByMetadataName(className);
         if (classType != null)
            throw new AssertFailedException(
               $"The class {className} was found but it should not exist. {Environment.NewLine}{Subject.OutputSyntaxTrees.Last()}");

         return new AndConstraint<GenerationResultAssertion>(this);
      }

      public AndConstraint<GenerationResultAssertion> NotHaveErrors()
      {
         ThrowOnErrors(Subject.GeneratedDiagnostics);
         ThrowOnErrors(Subject.OutputCompilation.GetDiagnostics());

         return new AndConstraint<GenerationResultAssertion>(this);
      }

      public AndConstraint<GenerationResultAssertion> NotHaveWarnings()
      {
         ThrowOnWarnings(Subject.GeneratedDiagnostics);
         ThrowOnWarnings(Subject.OutputCompilation.GetDiagnostics());

         return new AndConstraint<GenerationResultAssertion>(this);
      }

      #endregion

      #region Methods

      private static string CreateAllFound(ImmutableArray<Diagnostic> generatedDiagnostics)
      {
         return $"{Environment.NewLine}{string.Join(Environment.NewLine, generatedDiagnostics)}";
      }

      private static string CreateMessage(Diagnostic errorDiagnostic)
      {
         var builder = new StringBuilder();
         builder.AppendLine("MESSAGE");
         builder.AppendLine($"{errorDiagnostic.Id}: {errorDiagnostic.GetMessage()}");
         builder.AppendLine($"ERROR AT : {GetError(errorDiagnostic)}");
         builder.AppendLine("SOURCE");

         var codeWithMarkedLine = CreateSource(errorDiagnostic);
         builder.AppendLine(codeWithMarkedLine);

         return builder.ToString();
      }

      private static string CreateSource(Diagnostic errorDiagnostic)
      {
         var lineNumber = errorDiagnostic.Location.GetLineSpan().StartLinePosition.Line;
         var lines = errorDiagnostic.Location.SourceTree?.ToString()
            .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

         var stringBuilder = new StringBuilder();

         for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
         {
            var line = lines[lineIndex];
            if (lineIndex == lineNumber)
            {
               stringBuilder.Append(">>");
               if (line.StartsWith("  "))
                  line = line.Substring(2);
            }

            stringBuilder.AppendLine(line);
         }

         return stringBuilder.ToString();
      }

      private static string GetError(Diagnostic diagnostic)
      {
         var sourceTree = diagnostic.Location.SourceTree?.ToString();
         if (sourceTree == null)
            return string.Empty;

         var span = diagnostic.Location.SourceSpan;
         var builder = new StringBuilder();
         var expansion = 5;
         builder.AppendLine($"{sourceTree.Substring(span.Start - expansion, span.Length + expansion * 2)}");
         builder.Append($"           {string.Empty.PadRight(expansion)}{"".PadRight(span.Length, '^')}");
         return builder.ToString();
      }

      private static void ThrowOnErrors(IEnumerable<Diagnostic> diagnostics)
      {
         var errorDiagnostic = diagnostics.FirstOrDefault(x => x.Severity == DiagnosticSeverity.Error);
         if (errorDiagnostic != null)
            throw new AssertFailedException(CreateMessage(errorDiagnostic));
      }

      private static void ThrowOnWarnings(IEnumerable<Diagnostic> diagnostics)
      {
         var errorDiagnostic = diagnostics.FirstOrDefault(x => x.Severity >= DiagnosticSeverity.Warning);
         if (errorDiagnostic != null)
            throw new AssertFailedException(CreateMessage(errorDiagnostic));
      }

      private Diagnostic FindDiagnostic(string diagnosticId)
      {
         var diagnostic = Subject.GeneratedDiagnostics.FirstOrDefault(d => d.Id == diagnosticId);
         if (diagnostic == null)
         {
            diagnostic = Subject.OutputCompilation.GetDiagnostics()
               .FirstOrDefault(d => d.Id == diagnosticId);
         }

         return diagnostic;
      }

      #endregion
   }
}