// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentSetupSyntaxReceiver.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using System.Collections.Generic;

   using Microsoft.CodeAnalysis;
   using Microsoft.CodeAnalysis.CSharp.Syntax;

   /// <summary>Created on demand before each generation pass</summary>
   internal class FluentSetupSyntaxReceiver : ISyntaxReceiver
   {
      #region ISyntaxReceiver Members

      /// <summary>Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation</summary>
      public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
      {
         // any class with at least one attribute is a candidate for source generation
         if (!(syntaxNode is ClassDeclarationSyntax classDeclaration))
            return;

         if (IsSetupClass(classDeclaration))
            SetupCandidates.Add(classDeclaration);
      }

      #endregion

      #region Public Properties

      public List<ClassDeclarationSyntax> SetupCandidates { get; } = new List<ClassDeclarationSyntax>();

      #endregion

      #region Methods

      private static bool HasAttributes(ClassDeclarationSyntax classDeclaration)
      {
         return classDeclaration.AttributeLists.Count > 0;
      }

      private static bool IsSetupClass(ClassDeclarationSyntax classDeclaration)
      {
         return HasAttributes(classDeclaration);
      }

      #endregion
   }
}