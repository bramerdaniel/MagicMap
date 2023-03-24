// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassBuilderBase.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System;
using System.Text;

internal abstract class ClassBuilderBase<TOwner>
{
   #region Constants and Fields

   private StringBuilder sourceBuilder;

   #endregion

   #region Public Properties

   public bool IsStatic { get; set; }

   #endregion

   #region Properties

   public string ClassName => className();

   protected Func<string> className;

   protected Func<string> Modifier { get; set; }

   protected Func<string> Namespace { get; set; }

   protected StringBuilder SourceBuilder => sourceBuilder ??= InitializeSourceBuilder();

   #endregion

   #region Public Methods and Operators

   public string GenerateCode()
   {
      GenerateMembers();
      CloseNamespace();
      OnGenerationCompleted();
      return SourceBuilder.ToString();
   }

   protected virtual void OnGenerationCompleted()
   {
   }

   protected abstract void GenerateMembers();

   public TOwner WithName(Func<string> className)
   {
      this.className = className;
      return (TOwner)(object)this;
   }

   public TOwner WithName(string name)
   {
      return WithName(() => name);
   }

   #endregion

   #region Methods

   protected virtual void CloseNamespace()
   {
      if (Namespace != null)
         SourceBuilder.AppendLine("}");
   }

   private StringBuilder InitializeSourceBuilder()
   {
      sourceBuilder = new StringBuilder();
      OnInitialize();
      OpenNamespace(sourceBuilder);

      sourceBuilder.AppendLine("[global::System.Runtime.CompilerServices.CompilerGenerated]");
      if (Modifier != null)
         sourceBuilder.Append($"{Modifier()} ");

      if (IsStatic)
         sourceBuilder.Append("static ");

      sourceBuilder.AppendLine($"partial class {className()}");
      sourceBuilder.AppendLine("{");
      return sourceBuilder;
   }

   protected virtual void OnInitialize()
   {
   }

   protected virtual void OpenNamespace(StringBuilder builder)
   {
      if (Namespace is not null)
      {
         builder.AppendLine($"namespace {Namespace()}");
         builder.AppendLine("{");
      }
   }

   #endregion
}