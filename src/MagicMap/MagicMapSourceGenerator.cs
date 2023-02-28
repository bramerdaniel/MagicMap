﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentSetupSourceGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using MagicMap.Generators;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    [Generator]
   public class MagicMapSourceGenerator : IIncrementalGenerator
   {
      // https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md
      // https://notanaverageman.github.io/2020/12/07/cs-source-generators-cheatsheet.html
      // https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md
      public void Initialize(IncrementalGeneratorInitializationContext context)
      {
         // Here we generate the required attributes
         context.RegisterPostInitializationOutput(GeneratePostInitializationOutput);

         var provider = context.SyntaxProvider
            .CreateSyntaxProvider(IsClassDeclarationWithAttributes, CreateSemanticGenerationContext)
            .Where(c => c.IsEnabled())
            .Collect()
            .Combine(context.CompilationProvider);

         context.RegisterSourceOutput(provider, GenerateSourceFromContext);
      }

      private void GenerateSourceFromContext(SourceProductionContext productionContext, (ImmutableArray<IGeneratorContext> GeneratorContext, Compilation Compilation) data)
      {
         var generatorManager = MagicGeneratorManager.FromCompilation(data.Compilation);

         foreach (var generatorContext in data.GeneratorContext)
         {
            try
            {
               RunGenerator(productionContext, generatorManager, generatorContext);
            }
            catch (Exception e)
            {
               Trace.WriteLine(e.Message);
               // TODO report a diagnostic here and do not rethrow ?
               // This could be more pleasant for the user I assume
               throw;
            }
         }
      }

      private static void RunGenerator(SourceProductionContext productionContext, MagicGeneratorManager generatorManager, IGeneratorContext generatorContext)
      {
         if (generatorManager.TryFindGenerator(generatorContext, out var generator))
         {
            var source = generator.Generate();
            AddSourceOrReportError(productionContext, source);
         }
      }

      private static void AddSourceOrReportError(SourceProductionContext context, GeneratedSource generatedSource)
      {
         if (generatedSource.Enabled)
         {
            var formattedCode = FormatCode(generatedSource);
            context.AddSource(generatedSource.Name, SourceText.From(formattedCode, Encoding.UTF8));
         }

         foreach (var diagnostic in generatedSource.Diagnostics)
            context.ReportDiagnostic(diagnostic);
      }

      private static string FormatCode(GeneratedSource generatedSource)
      {
         var syntaxTree = CSharpSyntaxTree.ParseText(generatedSource.Code)
            .GetRoot()
            .NormalizeWhitespace();

         var builder = new StringBuilder();
         builder.AppendLine("//------------------------------------------------");
         builder.AppendLine("// <auto-generated>");
         builder.AppendLine("//     Generated by the MagicMap source generator");
         builder.AppendLine("// </auto-generated>");
         builder.AppendLine("//------------------------------------------------");
         builder.AppendLine(syntaxTree.ToString());
         
         return builder.ToString();
      }

      private static IGeneratorContext CreateSemanticGenerationContext(GeneratorSyntaxContext context, CancellationToken cancellationToken)
      {
         var classDeclarationNode = context.GetNamedType(cancellationToken);
         if (classDeclarationNode == null)
            return GeneratorContext.Empty;

         var typeMapperGenerator = TypeMapperAttributeGenerator.FromCompilation(context.SemanticModel.Compilation);
         if (typeMapperGenerator.TryExtractData(classDeclarationNode, out var typeMapperData))
            return typeMapperData;

         return GeneratorContext.Empty;
      }


      private static bool IsClassDeclarationWithAttributes(SyntaxNode node, CancellationToken cancellationToken)
      {
         return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
      }

      private void GeneratePostInitializationOutput(IncrementalGeneratorPostInitializationContext context)
      {
         context.AddSource("TypeMapperAttribute.generated.cs", TypeMapperAttributeGenerator.Code);
         context.AddSource("PropertyMappingAttribute.generated.cs", PropertyMappingAttributeGenerator.Code);
         context.AddSource("MapperFactoryAttribute.generated.cs", MapperFactoryAttributeGenerator.Code);
      }
   }
}