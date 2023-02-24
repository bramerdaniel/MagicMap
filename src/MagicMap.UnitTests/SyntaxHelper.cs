// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyntaxHelper.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests;

using System.Collections.Generic;

using MagicMap;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Obsolete("Check if still required")]
public class SyntaxHelper : CSharpSyntaxWalker
{
   #region Constants and Fields

   private readonly FluentSetupSyntaxReceiver receiver;

   #endregion

   #region Constructors and Destructors

   public SyntaxHelper()
   {
      receiver = new FluentSetupSyntaxReceiver();
   }

   #endregion

   #region Public Methods and Operators

   public override void VisitClassDeclaration(ClassDeclarationSyntax node)
   {
      receiver.OnVisitSyntaxNode(node);
      base.VisitClassDeclaration(node);
   }

   #endregion

   #region Methods

   internal static IEnumerable<ClassDeclarationSyntax> FindSetupClasses(SyntaxTree syntaxTree)
   {
      var syntaxWalker = new SyntaxHelper();
      syntaxWalker.Visit(syntaxTree.GetRoot());
      return syntaxWalker.GetSetupClasses();
   }

   internal IList<ClassDeclarationSyntax> GetSetupClasses()
   {
      return receiver.SetupCandidates;
   }

   #endregion
}