// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodBuilder.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System;
using System.Text;

using Microsoft.CodeAnalysis;

internal class MethodBuilder<TOwner> : MethodBuilderBase<MethodBuilder<TOwner>, TOwner>
   where TOwner : IDiagnosticReporter
{
   #region Constructors and Destructors

   public MethodBuilder(TOwner owner)
      : base(owner)
   {
   }

   #endregion

   #region Properties

   private Func<MethodBuilder<TOwner>, string> MethodBody { get; set; }

   #endregion

   #region Public Methods and Operators

   protected override string BuildOverride(StringBuilder sourceBuilder)
   {
      sourceBuilder.Append($"{Modifier()} {ReturnType()} {Name()}");
      sourceBuilder.Append("(");
      AppendSignature(sourceBuilder);
      sourceBuilder.AppendLine(")");
      sourceBuilder.AppendLine("{");
      sourceBuilder.AppendLine(MethodBody(this));
      sourceBuilder.AppendLine("}");

      return sourceBuilder.ToString();
   }

   public MethodBuilder<TOwner> WithBody(Func<string> body)
   {
      if (body == null)
         throw new ArgumentNullException(nameof(body));

      MethodBody = _ => body();
      return this;
   }
   public MethodBuilder<TOwner> WithBody(Func<MethodBuilder<TOwner>, string> body)
   {
      MethodBody = body ?? throw new ArgumentNullException(nameof(body));
      return this;
   }

   #endregion

   public void AddDiagnostic(DiagnosticDescriptor diagnosticDescriptor)
   {
      if (diagnosticDescriptor == null)
         throw new ArgumentNullException(nameof(diagnosticDescriptor));

      Owner.AddDiagnostic(diagnosticDescriptor);
   }
}