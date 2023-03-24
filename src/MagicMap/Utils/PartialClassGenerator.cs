// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartialClassGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

using MagicMap.Extensions;
using MagicMap.Generators.TypeMapper;

using Microsoft.CodeAnalysis;

[DebuggerDisplay("{ClassName}")]
internal sealed class PartialClassGenerator : IClassBuilder
{
   #region Constants and Fields

   private readonly Dictionary<string, IClassBuilder> nestedClasses = new();

   private IMemberBuilder activeBuilder;

   private INamespaceSymbol containingNamespace;

   private StringBuilder sourceBuilder;

   #endregion

   #region Constructors and Destructors

   public PartialClassGenerator(string className)
   {
      ClassName = className ?? throw new ArgumentNullException(nameof(className));
      Diagnostics = new List<Diagnostic>();
   }

   public PartialClassGenerator(INamedTypeSymbol userDefinedPart)
   {
      UserDefinedPart = userDefinedPart ?? throw new ArgumentNullException(nameof(userDefinedPart));
      ClassName = userDefinedPart.Name;
      IsStatic = userDefinedPart.IsStatic;
      Diagnostics = new List<Diagnostic>();
   }

   #endregion

   #region IClassBuilder Members

   public IClassBuilder Append(string code)
   {
      CloseActiveBuilder();

      if (!string.IsNullOrWhiteSpace(code))
         SourceBuilder.Append(code);

      return this;
   }

   public IClassBuilder AppendLine(string code)
   {
      CloseActiveBuilder();

      if (!string.IsNullOrWhiteSpace(code))
         SourceBuilder.AppendLine(code);

      return this;
   }

   public string ClassName { get; }

   public INamedTypeSymbol UserDefinedPart { get; }

   public MethodBuilder<IClassBuilder> AddMethod()
   {
      return UpdateActiveBuilder(new MethodBuilder<IClassBuilder>(this));
   }

   public IClassBuilder AddNestedClass(string name)
   {
      if (!nestedClasses.TryGetValue(name, out var nestedClass))
      {
         nestedClass = new PartialClassGenerator(name);
         nestedClasses[name] = nestedClass;
      }

      return nestedClass;
   }

   public PartialMethodBuilder<IClassBuilder> AddPartialMethod()
   {
      return UpdateActiveBuilder(new PartialMethodBuilder<IClassBuilder>(this));
   }

   public bool ContainsMethod(string name, params INamedTypeSymbol[] parameterTypes)
   {
      if (UserDefinedPart == null)
         return false;

      return UserDefinedPart.GetMethods(name)
         .Any(candidate => ParametersMatch(candidate.Parameters, parameterTypes));
   }

   public bool ContainsProperty(string name)
   {
      if (name == null)
         throw new ArgumentNullException(nameof(name));

      if (UserDefinedPart == null)
         return false;

      return UserDefinedPart.GetProperty(p => p.Name == name) != null;
   }

   public string GenerateCode()
   {
      CloseActiveBuilder();
      GenerateNestedClasses();
      SourceBuilder.AppendLine("}");

      if (Namespace is { IsGlobalNamespace: false })
         SourceBuilder.AppendLine("}");

      return SourceBuilder.ToString();
   }

   #endregion

   #region Properties

   internal IList<Diagnostic> Diagnostics { get; }

   internal bool IsStatic { get; set; }

   internal string Modifier { get; set; }

   internal INamespaceSymbol Namespace
   {
      get => containingNamespace ??= UserDefinedPart?.ContainingNamespace;
      set => containingNamespace = value;
   }

   private StringBuilder SourceBuilder => sourceBuilder ??= InitializeSourceBuilder();

   #endregion

   #region Public Methods and Operators

   public void AddDiagnostic(Diagnostic diagnostic)
   {
      if (diagnostic == null)
         throw new ArgumentNullException(nameof(diagnostic));

      Diagnostics.Add(diagnostic);
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

   #endregion

   #region Methods

   private void CloseActiveBuilder()
   {
      UpdateActiveBuilder((IMemberBuilder)null);
   }

   private string ComputeClassName()
   {
      if (UserDefinedPart != null)
         return UserDefinedPart.Name;
      return ClassName;
   }

   private void GenerateNestedClasses()
   {
      foreach (var nestedClass in nestedClasses.Values)
      {
         SourceBuilder.AppendLine(nestedClass.GenerateCode());
      }
   }

   private StringBuilder InitializeSourceBuilder()
   {
      var builder = new StringBuilder();
      if (Namespace is { IsGlobalNamespace: false })
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