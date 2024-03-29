﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperAnalyzer.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Analyzers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

using MagicMap.Extensions;

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
      
      var attributeData = mapperClassSymbol.GetAttribute(typeMapperAttribute);
      if (attributeData?.AttributeClass == null)
         return;

      var classContext = new MapperClassContext(mapperClassSymbol, typeMapperAttribute, attributeData,  context);

      AnalyzeSymbol(classContext);
      
      foreach (var diagnostic in classContext.Diagnostics)
         context.ReportDiagnostic(diagnostic);
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
      return propertySymbol.Locations.FirstOrDefault() ?? Location.None;
   }

   protected Location FindLocation(INamedTypeSymbol typeSymbol)
   {
      return typeSymbol.Locations.FirstOrDefault() ?? Location.None;
   }

   protected Location FindLocation(AttributeData attributeData)
   {
      return attributeData.ApplicationSyntaxReference != null
         ? Location.Create(attributeData.ApplicationSyntaxReference.SyntaxTree, attributeData.ApplicationSyntaxReference.Span)
         : Location.None;
   }
}