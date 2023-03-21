// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedSetupClassAnalyzer.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Analyzers
{
   using System.Collections.Immutable;
   using System.Linq;

   using Microsoft.CodeAnalysis;
   using Microsoft.CodeAnalysis.Diagnostics;

   [DiagnosticAnalyzer(LanguageNames.CSharp)]
   public class NestedMapperClassAnalyzer : DiagnosticAnalyzer
   {
      #region Public Properties
      
      /// <summary>Returns a set of descriptors for the diagnostics that this analyzer is capable of producing.</summary>
      public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(MagicMapDiagnostics.NotSupportedNestedSetup);

      #endregion

      #region Public Methods and Operators

      public override void Initialize(AnalysisContext context)
      {
         context.EnableConcurrentExecution();
         context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
         context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
      }

      #endregion

      #region Methods

      private static Location FindLocation(AttributeData attributeData, INamedTypeSymbol ownerClass)
      {
         if (attributeData.ApplicationSyntaxReference != null)
            return Location.Create(attributeData.ApplicationSyntaxReference.SyntaxTree, attributeData.ApplicationSyntaxReference.Span);

         return ownerClass.Locations.FirstOrDefault() ?? Location.None;
      }

      private void AnalyzeSymbol(SymbolAnalysisContext context)
      {
         // We only care about compilations where attribute type "TypeMapper" is available.
         var typeMapperAttribute = context.Compilation.GetTypeByMetadataName(MagicGeneratorManager.TypeMapperAttributeName);
         if (typeMapperAttribute == null)
            return;

         var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
         if (namedTypeSymbol.ContainingType == null)
         {
            // we only care about nested classes here
            return;
         }

         var attributeData = namedTypeSymbol.GetAttributes().FirstOrDefault(IsFluentSetupAttribute);
         if (attributeData?.AttributeClass == null)
            return;

         var location = FindLocation(attributeData, namedTypeSymbol);

         var diagnostic = Diagnostic.Create(MagicMapDiagnostics.NotSupportedNestedSetup, location);
         context.ReportDiagnostic(diagnostic);

         bool IsFluentSetupAttribute(AttributeData candidate)
         {
            return typeMapperAttribute.Equals(candidate.AttributeClass, SymbolEqualityComparer.Default);
         }
      }

      private void AnalyzeSymbol(SymbolAnalysisContext context, INamedTypeSymbol fluentSetupAttribute)
      {
         var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;
         if (namedTypeSymbol.ContainingType == null)
         {
            // we only care about nested classes here
            return;
         }

         var attributeData = namedTypeSymbol.GetAttributes().FirstOrDefault(IsFluentSetupAttribute);
         if (attributeData?.AttributeClass == null)
            return;

         var location = FindLocation(attributeData, namedTypeSymbol);

         var diagnostic = Diagnostic.Create(MagicMapDiagnostics.NotSupportedNestedSetup, location);
         context.ReportDiagnostic(diagnostic);

         bool IsFluentSetupAttribute(AttributeData candidate)
         {
            return fluentSetupAttribute.Equals(candidate.AttributeClass, SymbolEqualityComparer.Default);
         }
      }

      #endregion
   }
}