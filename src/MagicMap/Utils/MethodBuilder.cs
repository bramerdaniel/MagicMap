// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodBuilder.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class MethodBuilder
{
   #region Constants and Fields

   private Func<string> buildModifier;

   private Func<string> buildName;

   private Func<string> buildReturnType = () => "void";

   private Func<string> buildBody;

   private readonly List<(Func<string> type, Func<string> name)> buildParameters = new();

   private Func<string> buildSummary;

   private Predicate<MethodBuilder> generationEnabled = _ => true;

   #endregion

   #region Constructors and Destructors

   public MethodBuilder(ICodeBuilder owner)
   {
      Owner = owner ?? throw new ArgumentNullException(nameof(owner));
   }

   #endregion

   #region Public Properties

   public ICodeBuilder Owner { get; }

   #endregion

   #region Public Methods and Operators

   public void Build()
   {
      if (!generationEnabled(this))
         return;

      var sourceBuilder = new StringBuilder();

      if (buildSummary != null)
      {
         sourceBuilder.AppendLine("/// <summary>");
         foreach (var line in ComputeSummaryLines())
            sourceBuilder.AppendLine(line);
         sourceBuilder.AppendLine("/// </summary>");
      }

      sourceBuilder.Append($"{buildModifier()} {buildReturnType()} {buildName()}");
      sourceBuilder.Append("(");
      AppendSignature(sourceBuilder);
      sourceBuilder.AppendLine(")");
      sourceBuilder.AppendLine("{");
      sourceBuilder.AppendLine(buildBody());
      sourceBuilder.AppendLine("}");

      Owner.AppendLine(sourceBuilder.ToString());
   }

   private void AppendSignature(StringBuilder sourceBuilder)
   {
      var parameterString = string.Join(", ", buildParameters.Select(parameter => $"{parameter.type()} {parameter.name()}"));
      sourceBuilder.Append(parameterString);
   }

   private IEnumerable<string> ComputeSummaryLines()
   {
      var lines = buildSummary()
         .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

      foreach (var line in lines)
         yield return $"/// {line.TrimStart('/')}";
   }

   public MethodBuilder Condition(Predicate<MethodBuilder> condition)
   {
      generationEnabled = condition ?? throw new ArgumentNullException(nameof(condition));
      return this;
   }

   public MethodBuilder WithModifier(string modifier)
   {
      if (modifier == null)
         throw new ArgumentNullException(nameof(modifier));

      return WithModifier(() => modifier);
   }

   public MethodBuilder WithBody(Func<string> body)
   {
      buildBody = body ?? throw new ArgumentNullException(nameof(body));
      return this;
   }

   public MethodBuilder WithModifier(Func<string> modifier)
   {
      buildModifier = modifier;
      return this;
   }

   public MethodBuilder WithName(string name)
   {
      return WithName(() => name);
   }

   public MethodBuilder WithReturnType(Func<string> returnType)
   {
      buildReturnType = returnType;
      return this;
   }

   #endregion

   #region Methods

   private MethodBuilder WithName(Func<string> name)
   {
      buildName = name;
      return this;
   }

   #endregion

   public MethodBuilder WithParameter(Func<string> type, Func<string> name)
   {
      if (type == null)
         throw new ArgumentNullException(nameof(type));
      if (name == null)
         throw new ArgumentNullException(nameof(name));

      buildParameters.Add((type, name));
      return this;
   }

   public MethodBuilder WithSummary(Action<StringBuilder> summary)
   {
      if (summary == null)
         throw new ArgumentNullException(nameof(summary));

      var summaryBuilder = new StringBuilder();
      summary(summaryBuilder);
      return WithSummary(summaryBuilder.ToString);
   }

   public MethodBuilder WithSummary(Func<string> summary)
   {
      buildSummary = summary ?? throw new ArgumentNullException(nameof(summary));
      return this;
   }
}