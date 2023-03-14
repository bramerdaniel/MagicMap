// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassGenerationContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;

internal class ClassGenerationContext
{
   private readonly INamedTypeSymbol userDefinedPart;

   private StringBuilder sourceBuilder;

   private List<string> lazyMembers = new();

   public ClassGenerationContext(INamedTypeSymbol userDefinedPart)
   {
      this.userDefinedPart = userDefinedPart ?? throw new ArgumentNullException(nameof(userDefinedPart));
      ClassName = userDefinedPart.Name;
      IsStatic = userDefinedPart.IsStatic;
      Namespace = userDefinedPart.ContainingNamespace;
   }

   public ClassGenerationContext(string className)
   {
      ClassName = className ?? throw new ArgumentNullException(nameof(className));
   }

   private StringBuilder SourceBuilder => sourceBuilder ??= InitializeSourceBuilder();

   public bool ContainsProperty(string name)
   {
      if (name == null)
         throw new ArgumentNullException(nameof(name));

      return userDefinedPart?.GetProperty(p => p.Name == name) != null;
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
      if (userDefinedPart != null)
         return userDefinedPart.Name;
      return ClassName;
   }

   public string ClassName { get; }

   public string Modifier { get; set; }

   public bool IsStatic { get; set; }

   public bool Partial { get; set; } = true;

   public INamespaceSymbol Namespace { get; set; }

   public void AddCode(string code)
   {
      if(string.IsNullOrWhiteSpace(code))
         return;

      SourceBuilder.AppendLine(code);
   }
}