// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerationResult.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.Setups;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

using Microsoft.CodeAnalysis;

internal class GenerationResult
{
   #region Constants and Fields

   private IReadOnlyList<SyntaxTree> inputSyntaxTrees;

   private IReadOnlyList<SyntaxTree> outputSyntaxTrees;

   #endregion

   #region Public Properties

   public ImmutableArray<Diagnostic> GeneratedDiagnostics { get; set; }

   public Compilation InputCompilation { get; set; }

   public IReadOnlyList<SyntaxTree> InputSyntaxTrees => inputSyntaxTrees ??= InputCompilation.SyntaxTrees.ToArray();

   public Compilation OutputCompilation { get; set; }

   public IReadOnlyList<SyntaxTree> OutputSyntaxTrees => outputSyntaxTrees ??= OutputCompilation.SyntaxTrees.ToArray();

   #endregion

   #region Public Methods and Operators

   public void FailWith(string id, string hint)
   {
      if (!OutputCompilation.GetDiagnostics().Any(d => d.Id == id))
         Assert.Fail($"The expected diagnostic {id} was not found. Hint : {hint}");
   }

   public void Print(int skip = 3, int take = int.MaxValue)
   {
      Debug.WriteLine(GetGeneratedCode());

      string GetGeneratedCode()
      {
         var builder = new StringBuilder();
         builder.AppendLine();
         builder.AppendLine("### GENERATED CODE ###");
         builder.AppendLine();

         foreach (var resultSyntaxTree in OutputSyntaxTrees.Skip(1 + skip).Take(take))
         {
            builder.AppendLine(resultSyntaxTree.ToString());
            builder.AppendLine();
            builder.AppendLine("".PadRight(50, '-'));
         }

         return builder.ToString();
      }
   }

   #endregion
}