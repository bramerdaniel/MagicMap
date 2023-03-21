// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperAnalyzer.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Analyzers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

public abstract class TypeMapperAnalyzer : DiagnosticAnalyzer
{
   public override void Initialize(AnalysisContext context)
   {
      context.EnableConcurrentExecution();
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.RegisterSymbolAction(AnalyzeClass, SymbolKind.NamedType);
   }

   private void AnalyzeClass(SymbolAnalysisContext context)
   {
      var typeMapperAttribute = context.Compilation.GetTypeByMetadataName(MagicGeneratorManager.TypeMapperAttributeName);
      if (typeMapperAttribute == null)
         return;

      var mapperClassSymbol = (INamedTypeSymbol)context.Symbol;

      // We only care about compilations where attribute type "TypeMapper" is available.
      var attributeData = mapperClassSymbol.GetAttributes().FirstOrDefault(IsFluentSetupAttribute);
      if (attributeData?.AttributeClass == null)
         return;

      var classContext = new MapperClassContext(mapperClassSymbol, typeMapperAttribute, context);
      AnalyzeSymbol(classContext);
      foreach (var diagnostic in classContext.Dignostics)
         context.ReportDiagnostic(diagnostic);

      bool IsFluentSetupAttribute(AttributeData candidate)
      {
         return typeMapperAttribute.Equals(candidate.AttributeClass, SymbolEqualityComparer.Default);
      }
   }

   protected abstract void AnalyzeSymbol(IMapperContext context);

   protected Location FindLocation(AttributeData attributeData, INamedTypeSymbol ownerClass)
   {
      if (attributeData.ApplicationSyntaxReference != null)
         return Location.Create(attributeData.ApplicationSyntaxReference.SyntaxTree, attributeData.ApplicationSyntaxReference.Span);

      return ownerClass.Locations.FirstOrDefault() ?? Location.None;
   }

   protected Location FindLocation(IPropertySymbol propertySymbol)
   {
      var findLocation = propertySymbol.Locations.FirstOrDefault();
      return findLocation ?? Location.None;
   }
}