// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartialClassGenerationContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;

internal class PartialClassGenerator
{
   private StringBuilder sourceBuilder;

   private List<string> lazyMembers = new();

   public PartialClassGenerator(INamedTypeSymbol userDefinedPart)
   {
      UserDefinedPart = userDefinedPart ?? throw new ArgumentNullException(nameof(userDefinedPart));
      ClassName = userDefinedPart.Name;
      IsStatic = userDefinedPart.IsStatic;
   }

   private StringBuilder SourceBuilder => sourceBuilder ??= InitializeSourceBuilder();

   public bool ContainsProperty(string name)
   {
      if (name == null)
         throw new ArgumentNullException(nameof(name));

      return UserDefinedPart.GetProperty(p => p.Name == name) != null;
   }

   internal string GenerateCode()
   {
      GenerateLazyMembers();
      SourceBuilder.AppendLine("}");

      if (!Namespace.IsGlobalNamespace)
         SourceBuilder.AppendLine("}");

      return sourceBuilder.ToString();
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

      if (Partial)
         builder.Append("partial ");

      builder.AppendLine($"class {ComputeClassName()}");
      builder.AppendLine("{");
      return builder;
   }

   private void GenerateLazyMembers()
   {
      foreach (var lazyMember in lazyMembers)
         SourceBuilder.AppendLine(lazyMember);
   }

   private string ComputeClassName()
   {
      if (UserDefinedPart != null)
         return UserDefinedPart.Name;
      return ClassName;
   }

   public string ClassName { get; }

   public string Modifier { get; set; }

   public bool IsStatic { get; set; }

   public bool Partial { get; set; } = true;

   public INamespaceSymbol Namespace => UserDefinedPart.ContainingNamespace;

   public INamedTypeSymbol UserDefinedPart { get; }

   public void AppendLine(string code)
   {
      if (string.IsNullOrWhiteSpace(code))
         return;

      SourceBuilder.AppendLine(code);
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

   private bool ReturnTypeMatches(ITypeSymbol actualReturnType, INamedTypeSymbol expectedReturnType)
   {
      // TODO handle void
      if (actualReturnType.Equals(expectedReturnType, SymbolEqualityComparer.Default))
         return true;
      return false;

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
}