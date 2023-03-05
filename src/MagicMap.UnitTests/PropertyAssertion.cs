// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyAssertion.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests;

using System.Text;

using FluentAssertions;
using FluentAssertions.Primitives;

using MagicMap.UnitTests.Setups;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal class PropertyAssertion : ReferenceTypeAssertions<IPropertySymbol, PropertyAssertion>
{
   #region Constants and Fields

   private readonly GenerationResult generationResult;

   #endregion

   #region Constructors and Destructors

   public PropertyAssertion(GenerationResult generationResult, IPropertySymbol subject)
      : base(subject)
   {
      this.generationResult = generationResult;
   }

   #endregion

   #region Properties

   protected override string Identifier => nameof(PropertyAssertion);

   #endregion

   #region Public Methods and Operators

   public async Task<string> GetCodeAsync()
   {
      var syntaxReference = Subject.DeclaringSyntaxReferences.First();
      if (syntaxReference == null)
         throw new AssertFailedException("The syntax reference for the method could not be found");

      return (await syntaxReference.GetSyntaxAsync()).NormalizeWhitespace().ToString();
   }

   public PropertyAssertion IsInternal()
   {
      Subject.DeclaredAccessibility.Should().Be(Accessibility.Internal);
      return this;
   }
   public PropertyAssertion IsStatic()
   {
      if (Subject.IsStatic)
         return this;

      throw new AssertFailedException($"The property {Subject.Name} should be static but it is not.");
   }

   public PropertyAssertion IsPrivate()
   {
      Subject.DeclaredAccessibility.Should().Be(Accessibility.Private);
      return this;
   }

   public PropertyAssertion IsProtected()
   {
      Subject.DeclaredAccessibility.Should().Be(Accessibility.Protected);
      return this;
   }

   public PropertyAssertion IsPublic()
   {
      Subject.DeclaredAccessibility.Should().Be(Accessibility.Public);
      return this;
   }

   #endregion

   #region Methods

   string GetGeneratedCode()
   {
      var builder = new StringBuilder();
      builder.AppendLine();
      builder.AppendLine("### INPUT ###");
      builder.AppendLine(generationResult.OutputSyntaxTrees.First().ToString());
      builder.AppendLine();
      builder.AppendLine("### OUTPUT ###");

      foreach (var resultSyntaxTree in generationResult.OutputSyntaxTrees.Skip(1))
      {
         builder.AppendLine(resultSyntaxTree.ToString());
         builder.AppendLine();
         builder.AppendLine("".PadRight(50, '-'));
      }

      return builder.ToString();
   }

   private bool HasSignature(IMethodSymbol methodSymbol, string[] parameterTypes)
   {
      var typeNames = methodSymbol.Parameters.Select(x => x.Type.ToString()).ToArray();
      for (var i = 0; i < parameterTypes.Length; i++)
      {
         var expectedType = parameterTypes[i];
         if (!string.Equals(expectedType, typeNames[i]))
            return false;
      }

      return true;
   }

   #endregion

   public PropertyAssertion HasInitializationExpression(string expectedInitializationExpression)
   {
      var initializationExpression = GetInitializationString();
      if (initializationExpression != expectedInitializationExpression)
         throw new AssertFailedException($"The expected initialization expression {expectedInitializationExpression} did not match the found expression {initializationExpression}");

      return this;
   }

   private string GetInitializationString()
   {
      var syntaxNode = Subject.DeclaringSyntaxReferences
         .FirstOrDefault()
         .GetSyntax();

      var syntaxNodes = syntaxNode.DescendantNodes().ToArray();
      var arrowExpression = syntaxNodes.OfType<ArrowExpressionClauseSyntax>().FirstOrDefault();
      if (arrowExpression != null)
         return arrowExpression.Expression.ToFullString();

      var equalsValueClauseSyntax = syntaxNodes.OfType<EqualsValueClauseSyntax>().FirstOrDefault();
      return equalsValueClauseSyntax?.Value.ToString() ?? string.Empty;
   }
}