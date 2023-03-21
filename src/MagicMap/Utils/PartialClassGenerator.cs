// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartialClassGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using MagicMap.Extensions;
using Microsoft.CodeAnalysis;

internal sealed class PartialClassGenerator : ICodeBuilder
{
   #region Constants and Fields

   private IMemberBuilder activeBuilder;

   private StringBuilder sourceBuilder;

   #endregion

   #region Constructors and Destructors

   public PartialClassGenerator(INamedTypeSymbol userDefinedPart)
   {
      UserDefinedPart = userDefinedPart ?? throw new ArgumentNullException(nameof(userDefinedPart));
      ClassName = userDefinedPart.Name;
      IsStatic = userDefinedPart.IsStatic;
      Diagnostics = new List<Diagnostic>();
   }

   internal IList<Diagnostic> Diagnostics { get; }

   #endregion

   #region ICodeBuilder Members

   public ICodeBuilder Append(string code)
   {
      CloseActiveBuilder();

      if (!string.IsNullOrWhiteSpace(code))
         SourceBuilder.Append(code);

      return this;
   }

   public ICodeBuilder AppendLine(string code)
   {
      CloseActiveBuilder();

      if (!string.IsNullOrWhiteSpace(code))
         SourceBuilder.AppendLine(code);

      return this;
   }

   private void CloseActiveBuilder()
   {
      UpdateActiveBuilder((IMemberBuilder)null);
   }

   #endregion

   #region Public Properties

   private string ClassName { get; }

   private bool IsStatic { get; set; }

   private string Modifier { get; set; }

   private INamespaceSymbol Namespace => UserDefinedPart.ContainingNamespace;

   public INamedTypeSymbol UserDefinedPart { get; }

   #endregion

   #region Properties

   private StringBuilder SourceBuilder => sourceBuilder ??= InitializeSourceBuilder();

   #endregion

   #region Public Methods and Operators

   public MethodBuilder<PartialClassGenerator> AddMethod()
   {
      return UpdateActiveBuilder(new MethodBuilder<PartialClassGenerator>(this));
   }

   public void AddDiagnostic(Diagnostic diagnostic)
   {
      if (diagnostic == null)
         throw new ArgumentNullException(nameof(diagnostic));

      Diagnostics.Add(diagnostic);
   }


   public PartialMethodBuilder<PartialClassGenerator> AddPartialMethod()
   {
      return UpdateActiveBuilder(new PartialMethodBuilder<PartialClassGenerator>(this));
   }

   public bool ContainsMethod(INamedTypeSymbol returnType, string name, params INamedTypeSymbol[] parameterTypes)
   {
      foreach (var candidate in UserDefinedPart.GetMethods(name))
      {
         if (ParametersMatch(candidate.Parameters, parameterTypes) && ReturnTypeMatches(candidate.ReturnType, returnType))
            return true;
      }

      return false;
   }

   public bool ContainsMethod(string name, params INamedTypeSymbol[] parameterTypes)
   {
      return UserDefinedPart.GetMethods(name)
         .Any(candidate => ParametersMatch(candidate.Parameters, parameterTypes));
   }

   public bool ContainsProperty(string name)
   {
      if (name == null)
         throw new ArgumentNullException(nameof(name));

      return UserDefinedPart.GetProperty(p => p.Name == name) != null;
   }

   #endregion

   #region Methods

   internal string GenerateCode()
   {
      CloseActiveBuilder();

      SourceBuilder.AppendLine("}");

      if (!Namespace.IsGlobalNamespace)
         SourceBuilder.AppendLine("}");

      return SourceBuilder.ToString();
   }

   private string ComputeClassName()
   {
      if (UserDefinedPart != null)
         return UserDefinedPart.Name;
      return ClassName;
   }

   private StringBuilder InitializeSourceBuilder()
   {
      var builder = new StringBuilder();
      if (!Namespace.IsGlobalNamespace)
      {
         builder.AppendLine($"namespace {Namespace.ToDisplayString()}");
         builder.AppendLine("{");
      }

      builder.AppendLine("[global::System.Runtime.CompilerServices.CompilerGenerated]");
      if (Modifier != null)
         builder.Append($"{Modifier} ");

      if (IsStatic)
         builder.Append("static ");

      builder.AppendLine($"partial class {ComputeClassName()}");
      builder.AppendLine("{");
      return builder;
   }

   private bool ParametersMatch(ImmutableArray<IParameterSymbol> definedParameter, INamedTypeSymbol[] parameterTypes)
   {
      if (definedParameter.Length != parameterTypes.Length)
         return false;

      for (var i = 0; i < definedParameter.Length; i++)
      {
         if (!definedParameter[i].Type.Equals(parameterTypes[i], SymbolEqualityComparer.Default))
            return false;
      }

      return true;
   }

   private bool ReturnTypeMatches(ITypeSymbol actualReturnType, INamedTypeSymbol expectedReturnType)
   {
      // TODO handle void
      if (actualReturnType.Equals(expectedReturnType, SymbolEqualityComparer.Default))
         return true;
      return false;
   }

   private T UpdateActiveBuilder<T>(T builder)
      where T : IMemberBuilder
   {
      if (activeBuilder != null)
         SourceBuilder.AppendLine(activeBuilder.Build());

      activeBuilder = builder;
      return builder;
   }

   #endregion


}