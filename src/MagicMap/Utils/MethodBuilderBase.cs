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

using Microsoft.CodeAnalysis;

internal abstract class MethodBuilderBase<T, TOwner> : IMemberBuilder
{
   #region Constants and Fields

   private string code;

   private bool wasBuild;

   #endregion

   #region Constructors and Destructors

   protected MethodBuilderBase(TOwner owner)
   {
      Owner = owner ?? throw new ArgumentNullException(nameof(owner));
   }

   #endregion

   #region IMemberBuilder Members

   public string Build()
   {
      try
      {
         if (wasBuild)
            return code;

         if (GenerationRequired())
         {
            var sourceBuilder = new StringBuilder();
            AppendDescription(sourceBuilder);
            code = BuildOverride(sourceBuilder);
         }
         else
         {
            code = string.Empty;
         }

         return code;
      }
      finally
      {
         wasBuild = true;
      }
   }

   #endregion

   #region Properties

   protected IList<Predicate<T>> Conditions { get; } = new List<Predicate<T>>();

   protected Func<string> Modifier { get; set; } = () => string.Empty;

   protected Func<string> Name { get; set; }

   protected TOwner Owner { get; }

   protected Func<string> ReturnType { get; set; } = () => "void";

   private List<(Func<string> type, Func<string> name)> Parameters { get; } = new();

   private Func<string> Description { get; set; }

   #endregion

   #region Public Methods and Operators

   public T WithCondition(Predicate<T> condition)
   {
      Conditions.Add(condition);
      return (T)(object)this;
   }

   public T WithModifier(string modifier)
   {
      if (modifier == null)
         throw new ArgumentNullException(nameof(modifier));

      return WithModifier(() => modifier);
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

   public T WithReturnType(ITypeSymbol returnType)
   {
      return WithReturnType(() => returnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
   }

   public T WithSummary(string summary)
   {
      if (summary == null)
         throw new ArgumentNullException(nameof(summary));

      return WithSummary(() => summary);
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
      Description = summary ?? throw new ArgumentNullException(nameof(summary));
      return (T)(object)this;
   }

   #endregion

   #region Methods

   private void AppendDescription(StringBuilder sourceBuilder)
   {
      if (Description == null)
         return;

      var lines = SplitDescriptionIntoLines();
      if (lines.Length == 1)
      {
         sourceBuilder.AppendLine($"/// <summary>{lines[0].TrimStart('/', ' ')}</summary>");
      }
      else
      {
         sourceBuilder.AppendLine("/// <summary>");
         foreach (var line in lines)
            sourceBuilder.AppendLine(line);
         sourceBuilder.AppendLine("/// </summary>");
      }
   }

   protected void AppendSignature(StringBuilder sourceBuilder)
   {
      var parameterString = string.Join(", ", Parameters.Select(parameter => $"{parameter.type()} {parameter.name()}"));
      sourceBuilder.Append(parameterString);
   }

   protected abstract string BuildOverride(StringBuilder sourceBuilder);

   private bool GenerationRequired()
   {
      foreach (var condition in Conditions)
      {
         if (!condition((T)(object)this))
            return false;
      }

      return true;
   }

   private string[] SplitDescriptionIntoLines()
   {
      return Description()
         .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
         .Select(line => $"/// {line.TrimStart('/')}")
         .ToArray();
   }

   #endregion
}