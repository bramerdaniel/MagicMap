// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentSetupSourceGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using System.Collections.Immutable;
   using System.Text;
   using System.Threading;

   using Microsoft.CodeAnalysis;
   using Microsoft.CodeAnalysis.CSharp;
   using Microsoft.CodeAnalysis.CSharp.Syntax;
   using Microsoft.CodeAnalysis.Text;

   [Generator]
   public class MagicMapSourceGenerator : IIncrementalGenerator
   {
      // https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md
      // https://notanaverageman.github.io/2020/12/07/cs-source-generators-cheatsheet.html
      public void Initialize(IncrementalGeneratorInitializationContext context)
      {
         // Here we generate the required attributes
         context.RegisterPostInitializationOutput(GeneratePostInitializationOutput);

         var provider = context.SyntaxProvider
            .CreateSyntaxProvider(IsTypeDeclarationWithAttributes, GetSemanticTargetForGeneration)
            .Where(x => x != null)
            .Collect()
            .Combine(context.CompilationProvider);

         context.RegisterImplementationSourceOutput(provider, GenerateMappers);
      }

      private static void AddSourceOrReportError(SourceProductionContext context, GeneratedSource generatedSource)
      {
         if (generatedSource.Enabled)
            context.AddSource(generatedSource.Name, SourceText.From(generatedSource.Code, Encoding.UTF8));

         foreach (var diagnostic in generatedSource.Diagnostics)
            context.ReportDiagnostic(diagnostic);
      }

      private void GenerateMappers(SourceProductionContext context, (ImmutableArray<ClassDeclarationSyntax> ClassDeclarations, Compilation Compilation) data)
      {
         var generatorManager = MagicGeneratorManager.FromCompilation(data.Compilation);

         foreach (var classDeclaration in data.ClassDeclarations)
         {
            if (generatorManager.TryCreateMagicGenerator(classDeclaration, out var generator))
            {
               var source = generator.Generate();
               AddSourceOrReportError(context, source);
            }
         }
      }

      private static ClassDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
      {
         var classSymbol  = context.SemanticModel.GetNameType(context.Node);
         var calssType = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
         var displayString = classSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
         

         // TODO return all and report diagnostics later
         var syntax = (ClassDeclarationSyntax)context.Node;
         
         if (syntax.Modifiers.Any(SyntaxKind.PartialKeyword) is false ||
             syntax.Modifiers.Any(SyntaxKind.InterfaceDeclaration))
         {
            return null;
         }

         return syntax;
      }


      private static bool IsTypeDeclarationWithAttributes(SyntaxNode node, CancellationToken cancellationToken)
      {
         return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
      }

      private void GeneratePostInitializationOutput(IncrementalGeneratorPostInitializationContext context)
      {
         context.AddSource("TypeMapperAttribute.generated.cs", TypeMapperAttribute.Code);

      }
   }
}