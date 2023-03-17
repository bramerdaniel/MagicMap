// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuilderExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System.Linq;

using Microsoft.CodeAnalysis;

internal static class BuilderExtensions
{
   #region Public Methods and Operators

   /// <summary>Adds the method.</summary>
   /// <param name="owner">The owner.</param>
   /// <param name="name">The name.</param>
   /// <returns></returns>
   public static MethodBuilder AddMethod(this PartialClassGenerator owner, string name)
   {
      return owner.AddMethod()
         .WithName(name);
   }

   public static MethodBuilder AddMethod(this PartialClassGenerator owner, string name, params (INamedTypeSymbol Type, string Name)[] parameters)
   {
      var methodBuilder = owner.AddMethod()
         .Condition(_ => !owner.ContainsMethod("Map", parameters.Select(x => x.Type).ToArray()))
         .WithName(name);

      foreach (var parameter in parameters)
      {
         methodBuilder.WithParameter(
            () => parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            () => parameter.Name);
      }

      return methodBuilder;
   }

   #endregion
}