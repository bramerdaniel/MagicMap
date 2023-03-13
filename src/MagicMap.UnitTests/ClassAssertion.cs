// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassAssertion.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests;

using System.Linq;
using System.Reflection;
using System.Text;

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

using MagicMap.UnitTests.Setups;

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

internal class ClassAssertion : ReferenceTypeAssertions<INamedTypeSymbol, ClassAssertion>
{
   #region Constants and Fields

   private readonly GenerationResult generationResult;

   #endregion

   #region Constructors and Destructors

   public ClassAssertion(GenerationResult generationResult, INamedTypeSymbol subject)
      : base(subject)
   {
      this.generationResult = generationResult;
   }

   #endregion

   #region Properties

   protected override string Identifier => nameof(INamedTypeSymbol);

   #endregion

   #region Public Methods and Operators

   public MethodAssertion WhereMethod(string methodName, string signature = null)
   {
      IMethodSymbol methodSymbol;
      var methodSymbols = Subject.GetMembers(methodName).OfType<IMethodSymbol>().ToArray();
      if (methodSymbols.Length == 1)
      {
         methodSymbol = methodSymbols[0];
      }
      else
      {
         methodSymbol = MatchMethodBySignature(methodSymbols, signature);
         Assert.IsNotNull(methodSymbol, $"A method {methodName} with the specified signature could not be found.{GetGeneratedCode()}");
      }

      Assert.IsNotNull(methodSymbol, $"The method {methodName} could not be found.{GetGeneratedCode()}");
      return new MethodAssertion(generationResult, methodSymbol);
   }

   public PropertyAssertion WhereProperty(string propertyName)
   {
      var property = Subject.GetMembers(propertyName).OfType<IPropertySymbol>().FirstOrDefault();
      if (property == null)
         throw new AssertionFailedException($"The expected property {propertyName} could not be found.{GetGeneratedCode()}");

      return new PropertyAssertion(generationResult, property);
   }

   private IMethodSymbol MatchMethodBySignature(IMethodSymbol[] methodSymbols, string signature = null)
   {
      IMethodSymbol candidate = null;
      foreach (var methodSymbol in methodSymbols)
      {
         var methodSignature = ToSignatureString(methodSymbol);
         if (methodSignature.StartsWith(signature ?? string.Empty))
         {
            if (candidate == null)
            {
               candidate = methodSymbol;
            }
            else
            {
               candidate = null;
               break;
            }
         }
      }

      return candidate ?? methodSymbols.FirstOrDefault(ms => ToSignatureString(ms) == signature);
   }

   private string ToSignatureString(IMethodSymbol methodSymbol)
   {
      var signatureString = string.Join(", ", methodSymbol.Parameters.Select(x => $"{x.Type.ToDisplayString()} {x.Name}"));
      return signatureString;
   }

   public ClassAssertion WithField(string expectedFieldName)
   {
      var fieldSymbol = Subject.GetMembers(expectedFieldName).OfType<IFieldSymbol>().Single();
      Assert.IsNotNull(fieldSymbol, $"The field {expectedFieldName} could not be found.{GetGeneratedCode()}");
      return this;
   }

   public ClassAssertion WithInternalMethod(string methodName)
   {
      var methodSymbol = Subject.GetMembers(methodName).OfType<IMethodSymbol>().Single();
      Assert.IsNotNull(methodSymbol, $"The method {methodName} could not be found");

      methodSymbol.DeclaredAccessibility.Should().Be(Accessibility.Internal);
      return this;
   }

   public ClassAssertion WithInternalModifier()
   {
      Subject.DeclaredAccessibility.Should().Be(Accessibility.Internal);
      return this;
   }

   public ClassAssertion WithMethod(string methodName)
   {
      var methodSymbol = Subject.GetMembers(methodName).OfType<IMethodSymbol>().FirstOrDefault();
      Assert.IsNotNull(methodSymbol, $"The method {methodName} could not be found.{GetGeneratedCode()}");
      return this;
   }

   public ClassAssertion WithMethod(string methodName, params string[] parameterTypes)
   {
      var methods = Subject.GetMembers(methodName).OfType<IMethodSymbol>()
         .Where(x => x.Parameters.Length == parameterTypes.Length).ToArray();

      Assert.IsTrue(methods.Length > 0, $"The method {methodName} with {parameterTypes.Length} parameters could not be found.{GetGeneratedCode()}");

      if (methods.Any(x => HasSignature(x, parameterTypes)))
         return this;

      Assert.Fail($"Could not find a method with matching signature{GetGeneratedCode()}");
      return null;
   }

   public ClassAssertion WithoutMethod(string methodName)
   {
      var methodSymbol = Subject.GetMembers(methodName).OfType<IMethodSymbol>().FirstOrDefault();
      Assert.IsNull(methodSymbol, $"The method {methodName} was found but it should not.");
      return this;
   }

   public ClassAssertion WithoutMethod(string methodName, string signature)
   {
      IMethodSymbol methodSymbol;
      var methodSymbols = Subject.GetMembers(methodName).OfType<IMethodSymbol>().ToArray();
      if (methodSymbols.Length == 1 && signature == null)
      {
         methodSymbol = methodSymbols[0];
      }
      else
      {
         methodSymbol = methodSymbols.FirstOrDefault(m => m.Parameters.FirstOrDefault()?.ToString() == signature);
      }

      Assert.IsNull(methodSymbol, $"The method {methodName}({methodSymbol?.Parameters.FirstOrDefault()?.ToString()}) was found but it should not.");
      return this;
   }

   public ClassAssertion WithProtectedMethod(string methodName)
   {
      var methodSymbol = Subject.GetMembers(methodName).OfType<IMethodSymbol>().FirstOrDefault();
      Assert.IsNotNull(methodSymbol, $"The method {methodName} could not be found.{GetGeneratedCode()}");
      methodSymbol.DeclaredAccessibility.Should().Be(Accessibility.Protected);
      return this;
   }

   public ClassAssertion WithPublicModifier()
   {
      Subject.DeclaredAccessibility.Should().Be(Accessibility.Public);
      return this;
   }

   public ClassAssertion WithStaticMethod(string methodName)
   {
      var methodSymbol = Subject.GetMembers(methodName).OfType<IMethodSymbol>().FirstOrDefault();
      Assert.IsNotNull(methodSymbol, $"The method {methodName} could not be found. {GetGeneratedCode()}");
      Assert.IsTrue(methodSymbol.IsStatic, $"The found method {methodName} is not static. {GetGeneratedCode()}");
      return this;
   }

   #endregion

   #region Methods

   string GetGeneratedCode()
   {
      var builder = new StringBuilder();
      builder.AppendLine();
      builder.AppendLine("### INPUT ###");
      builder.AppendLine(generationResult.OutputSyntaxTrees.First().GetRoot().NormalizeWhitespace().ToString());
      builder.AppendLine();
      builder.AppendLine("### OUTPUT ###");

      foreach (var resultSyntaxTree in generationResult.OutputSyntaxTrees.Skip(1))
      {
         builder.AppendLine(resultSyntaxTree.ToString());
         builder.AppendLine();
         builder.AppendLine("".PadRight(70, '-'));
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

   public ClassAssertion WithoutCode(string expectedSubstring)
   {
      var syntaxReference = Subject.DeclaringSyntaxReferences.LastOrDefault();
      if (syntaxReference == null)
         throw new AssertFailedException("Syntax reference could not be found");

      var code = syntaxReference.GetSyntax().ToString();
      Assert.IsFalse(code.Contains(expectedSubstring), CreateMessage());

      return this;

      string CreateMessage()
      {
         var builder = new StringBuilder();
         builder.AppendLine($"The generated code contained '{expectedSubstring}' but is should not.");
         builder.AppendLine();
         builder.AppendLine("CLASS CODE");
         builder.AppendLine();
         builder.AppendLine(code);

         return builder.ToString();
      }

   }
}