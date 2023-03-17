﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodBuilder.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System;
using System.Text;

internal class MethodBuilder : MethodBuilderBase<MethodBuilder>
{
   #region Constructors and Destructors

   public MethodBuilder(ICodeBuilder owner)
      : base(owner)
   {
   }

   #endregion

   #region Properties

   private Func<string> MethodBody { get; set; }

   #endregion

   #region Public Methods and Operators

   public void Build()
   {
      if (!GenerationRequired())
         return;

      var sourceBuilder = new StringBuilder();

      AppendDescription(sourceBuilder);

      sourceBuilder.Append($"{Modifier()} {ReturnType()} {Name()}");
      sourceBuilder.Append("(");
      AppendSignature(sourceBuilder);
      sourceBuilder.AppendLine(")");
      sourceBuilder.AppendLine("{");
      sourceBuilder.AppendLine(MethodBody());
      sourceBuilder.AppendLine("}");

      Owner.AppendLine(sourceBuilder.ToString());
   }

   public MethodBuilder WithBody(Func<string> body)
   {
      MethodBody = body ?? throw new ArgumentNullException(nameof(body));
      return this;
   }

   #endregion
}