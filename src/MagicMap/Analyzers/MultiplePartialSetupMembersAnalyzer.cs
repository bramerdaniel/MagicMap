// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiplePartialSetupMembersAnalyzer.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Analyzers
{
   using System;
   using System.Collections.Immutable;
   using System.Linq;

   using Microsoft.CodeAnalysis;
   using Microsoft.CodeAnalysis.Diagnostics;

   [DiagnosticAnalyzer(LanguageNames.CSharp)]
   public class MultiplePartialSetupMembersAnalyzer : DiagnosticAnalyzer
   {
      #region Public Properties

      /// <summary>Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.</summary>
      public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
         ImmutableArray.Create(MagicMapDiagnostics.MultiplePartialParts);

      #endregion

      #region Public Methods and Operators

      public override void Initialize(AnalysisContext context)
      {
         context.EnableConcurrentExecution();

         context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
         context.RegisterCompilationStartAction(compilationContext =>
         {
            // We only care about compilations where attribute type "FluentSetup" is available.
            var fluentSetupAttribute = compilationContext.Compilation.GetTypeByMetadataName(MagicGeneratorManager.TypeMapperAttributeName);
            if (fluentSetupAttribute == null)
            {
               return;
            }

            // Register an action that accesses the immutable state and reports diagnostics.
            compilationContext.RegisterSymbolAction(symbolContext => AnalyzeSymbol(symbolContext, fluentSetupAttribute), SymbolKind.NamedType);
         });
      }

      #endregion

      #region Methods

      private static bool IsGeneratedPartialPart(INamedTypeSymbol namedTypeSymbol)
      {
         foreach (var reference in namedTypeSymbol.DeclaringSyntaxReferences)
         {
            if (reference.SyntaxTree.FilePath.EndsWith(".generated.cs", StringComparison.InvariantCultureIgnoreCase))
               return true;
         }

         return false;
      }

      private void AnalyzeSymbol(SymbolAnalysisContext context, INamedTypeSymbol fluentSetupAttribute)
      {
         var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

         if (namedTypeSymbol.DeclaringSyntaxReferences.Length > 1)
         {
            var attributeData = namedTypeSymbol.GetAttributes().FirstOrDefault(IsFluentSetupAttribute);
            if (attributeData == null || attributeData.AttributeClass == null)
               return;

            if (IsGeneratedPartialPart(namedTypeSymbol))
               return;

            var reference = attributeData.ApplicationSyntaxReference;
            if (reference == null)
               return;

            var diagnostic = Diagnostic.Create(MagicMapDiagnostics.MultiplePartialParts, Location.Create(reference.SyntaxTree, reference.Span),
               namedTypeSymbol.Name);
            context.ReportDiagnostic(diagnostic);
         }

         bool IsFluentSetupAttribute(AttributeData attributeData)
         {
            if (fluentSetupAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
               return true;
            return false;
         }
      }

      #endregion
   }
}