// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodBuilderBase.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class MethodBuilderBase<T>
{
   #region Constructors and Destructors

   protected MethodBuilderBase(ICodeBuilder owner)
   {
      Owner = owner ?? throw new ArgumentNullException(nameof(owner));
   }

   #endregion

   #region Properties

   protected IList<Predicate<T>> Conditions { get; set; } = new List<Predicate<T>>();

   protected Func<string> Modifier { get; set; }

   protected Func<string> Name { get; set; }

   protected ICodeBuilder Owner { get; }

   private List<(Func<string> type, Func<string> name)> Parameters { get; } = new();

   protected Func<string> ReturnType { get; set; } = () => "void";

   private Func<string> Summary { get; set; }

   #endregion

   #region Public Methods and Operators

   public T WithCondition(Predicate<T> condition)
   {
      Conditions.Add(condition);
      return (T)(object)this;
   }

   protected bool GenerationRequired()
   {
      foreach (var condition in Conditions)
      {
         if (!condition((T)(object)this))
            return false;
      }

      return true;
   }

   public T WithModifier(string modifier)
   {
      if (modifier == null)
         throw new ArgumentNullException(nameof(modifier));

      return WithModifier(() => modifier);
   }

   public T WithSummary(string summary)
   {
      if (summary == null)
         throw new ArgumentNullException(nameof(summary));

      return WithSummary(() => summary);
   }

   public T WithModifier(Func<string> modifier)
   {
      Modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
      return (T)(object)this;
   }

   public T WithName(Func<string> name)
   {
      Name = name;
      return (T)(object)this;
   }

   public T WithName(string name)
   {
      return WithName(() => name);
   }

   public T WithParameter(Func<string> type, Func<string> name)
   {
      if (type == null)
         throw new ArgumentNullException(nameof(type));
      if (name == null)
         throw new ArgumentNullException(nameof(name));

      Parameters.Add((type, name));
      return (T)(object)this;
   }

   public T WithReturnType(Func<string> returnType)
   {
      ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
      return (T)(object)this;
   }

   public T WithSummary(Action<StringBuilder> summary)
   {
      if (summary == null)
         throw new ArgumentNullException(nameof(summary));

      var summaryBuilder = new StringBuilder();
      summary(summaryBuilder);
      return WithSummary(summaryBuilder.ToString);
   }

   public T WithSummary(Func<string> summary)
   {
      Summary = summary ?? throw new ArgumentNullException(nameof(summary));
      return (T)(object)this;
   }

   protected void AppendSignature(StringBuilder sourceBuilder)
   {
      var parameterString = string.Join(", ", Parameters.Select(parameter => $"{parameter.type()} {parameter.name()}"));
      sourceBuilder.Append(parameterString);
   }
   protected void AppendDescription(StringBuilder sourceBuilder)
   {
      if (Summary == null)
         return;

      var lines = SplitDescriptionIntoLines();
      if (lines.Length == 1)
      {
         sourceBuilder.AppendLine($"/// <summary>{lines[0]}</summary>");
      }
      else
      {
         sourceBuilder.AppendLine("/// <summary>");
         foreach (var line in lines)
            sourceBuilder.AppendLine(line);
         sourceBuilder.AppendLine("/// </summary>");
      }
   }

   private string[] SplitDescriptionIntoLines()
   {
      return Summary()
         .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
         .Select(line => $"/// {line.TrimStart('/')}")
         .ToArray();
   }


   #endregion
}