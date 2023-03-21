// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedClassBuilder.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System;
using System.Text;

internal class NestedClassBuilder<TOwner> : ClassBuilderBase<NestedClassBuilder<TOwner>>
{
   #region Constructors and Destructors

   public NestedClassBuilder(TOwner owner)
   {
      Owner = owner ?? throw new ArgumentNullException(nameof(owner));
   }


   #endregion

   #region Public Properties

   public TOwner Owner { get; }

   #endregion

   #region Public Methods and Operators

   protected override void GenerateMembers()
   {

   }

   protected override void CloseNamespace()
   {
      // Nested classes do not have a namespace
   }

   protected override void OpenNamespace(StringBuilder builder)
   {
      // Nested classes do not have a namespace
   }

   #endregion

   public void AppendLine(string text)
   {
      SourceBuilder.AppendLine(text);
   }
}