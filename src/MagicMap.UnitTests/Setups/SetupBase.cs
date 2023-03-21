// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupBase.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.Setups;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal class SetupBase
{
   #region Constants and Fields

   private readonly List<SyntaxTree> syntaxTrees;

   #endregion

   #region Constructors and Destructors

   public SetupBase()
   {
      syntaxTrees = new List<SyntaxTree>(5);
      SyntaxTrees = syntaxTrees;
   }

   #endregion

   #region Properties

   protected string RootNamespace { get; set; } = "RootNamespace";

   protected IReadOnlyList<SyntaxTree> SyntaxTrees { get; }

   #endregion

   #region Methods

   protected CSharpSyntaxTree AddSource(string code)
   {
      var syntaxTree = CSharpSyntaxTree.ParseText(code);
      ThrowOnErrors(syntaxTree.GetDiagnostics());
      syntaxTrees.Add(syntaxTree);

      return (CSharpSyntaxTree)syntaxTree;
   }

   protected virtual List<MetadataReference> ComputeReferences()
   {
      var references = new List<MetadataReference>();
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (var assembly in assemblies)
         if (!assembly.IsDynamic)
            references.Add(MetadataReference.CreateFromFile(assembly.Location));

      return references;
   }

   protected virtual CSharpCompilation CreateCompilation()
   {
      return CSharpCompilation.Create(RootNamespace, SyntaxTrees, ComputeReferences(),
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
   }

   private void ThrowOnErrors(IEnumerable<Diagnostic> diagnostics)
   {
      var errorDiagnostic = diagnostics.FirstOrDefault(x => x.Severity == DiagnosticSeverity.Error);
      if (errorDiagnostic != null)
         throw new InvalidOperationException(errorDiagnostic.GetMessage());
   }

   #endregion
}