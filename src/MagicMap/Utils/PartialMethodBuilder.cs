﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartialMethodBuilder.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System.Text;

internal class PartialMethodBuilder : MethodBuilderBase<PartialMethodBuilder>
{
   #region Constructors and Destructors

   public PartialMethodBuilder(ICodeBuilder owner)
      : base(owner)
   {
   }

   #endregion

   #region Public Methods and Operators

   protected override string BuildOverride(StringBuilder sourceBuilder)
   {
      if (Modifier != null)
         sourceBuilder.Append($"{Modifier()} ");

      sourceBuilder.Append($"partial {ReturnType()} {Name()}");
      sourceBuilder.Append("(");
      AppendSignature(sourceBuilder);
      sourceBuilder.AppendLine(");/*NEWLINE*/");

      return sourceBuilder.ToString();
   }

   #endregion
}