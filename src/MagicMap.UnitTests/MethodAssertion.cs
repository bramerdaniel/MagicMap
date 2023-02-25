// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodAssertion.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests;

using System.Linq;
using System.Text;

using FluentAssertions;
using FluentAssertions.Primitives;

using MagicMap.UnitTests.Setups;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

internal class MethodAssertion : ReferenceTypeAssertions<IMethodSymbol, MethodAssertion>
{
   #region Constants and Fields

   private readonly GenerationResult generationResult;

   #endregion

   #region Constructors and Destructors

   public MethodAssertion(GenerationResult generationResult, IMethodSymbol subject)
      : base(subject)
   {
      this.generationResult = generationResult;
   }

   #endregion

   #region Properties

   protected override string Identifier => nameof(INamedTypeSymbol);

   #endregion

   #region Public Methods and Operators

   public MethodAssertion HasParameter(string expectedParameter)
   {
      var syntaxReference = Subject.DeclaringSyntaxReferences.FirstOrDefault();
      var syntax = syntaxReference?.GetSyntax() as MethodDeclarationSyntax;

      if (syntax == null)
         throw new AssertFailedException("Method syntax could not be found");

      foreach (var parameter in syntax.ParameterList.Parameters)
      {
         if (parameter.ToString().Contains(expectedParameter))
            return this;
      }

      Assert.Fail(CreateMessage());

      return this;
      string CreateMessage()
      {
         var builder = new StringBuilder();
         builder.AppendLine($"The expected parameter '{expectedParameter}' could not be found.");
         builder.AppendLine();
         builder.AppendLine("METHOD CODE");
         builder.AppendLine();
         builder.AppendLine(syntax.ToString());

         return builder.ToString();
      }
   }
   public MethodAssertion Contains(string expectedSubstring)
   {
      var syntaxReference = Subject.DeclaringSyntaxReferences.FirstOrDefault();
      if (syntaxReference == null)
         throw new AssertFailedException("Syntax reference could not be found");

      var code = syntaxReference.GetSyntax().ToString();
      Assert.IsTrue(code.Contains(expectedSubstring), CreateMessage());
      code.Should().Contain(expectedSubstring);

      return this;
      string CreateMessage()
      {
         var builder = new StringBuilder();
         builder.AppendLine($"The expected code '{expectedSubstring}' could not be found.");
         builder.AppendLine();
         builder.AppendLine("METHOD CODE");
         builder.AppendLine();
         builder.AppendLine(code);

         return builder.ToString();
      }
   }

   public MethodAssertion NotContains(string expectedSubstring)
   {
      var syntaxReference = Subject.DeclaringSyntaxReferences.FirstOrDefault();
      if (syntaxReference == null)
         throw new AssertFailedException("Syntax reference could not be found");

      var code = syntaxReference.GetSyntax().ToString();
      Assert.IsFalse(code.Contains(expectedSubstring), CreateMessage());

      return this;
      string CreateMessage()
      {
         var builder = new StringBuilder();
         builder.AppendLine($"The expected code contained '{expectedSubstring}' but is should not.");
         builder.AppendLine();
         builder.AppendLine("METHOD CODE");
         builder.AppendLine();
         builder.AppendLine(code);

         return builder.ToString();
      }
   }



   public MethodAssertion IsInternal()
   {
      Subject.DeclaredAccessibility.Should().Be(Accessibility.Internal);
      return this;
   }

   public MethodAssertion IsPartialDefinition()
   {
      Subject.IsPartialDefinition.Should().BeTrue();
      return this;
   }

   public async Task<string> GetCodeAsync()
   {
      var syntaxReference = Subject.DeclaringSyntaxReferences.First();
      if (syntaxReference == null)
         throw new AssertFailedException("The syntax reference for the method could not be found");

      return (await syntaxReference.GetSyntaxAsync()).NormalizeWhitespace().ToString();
   }

   public MethodAssertion IsPublic()
   {
      Subject.DeclaredAccessibility.Should().Be(Accessibility.Public);
      return this;
   }

   public MethodAssertion IsProtected()
   {
      Subject.DeclaredAccessibility.Should().Be(Accessibility.Protected);
      return this;
   }
   public MethodAssertion IsPrivate()
   {
      Subject.DeclaredAccessibility.Should().Be(Accessibility.Private);
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
}