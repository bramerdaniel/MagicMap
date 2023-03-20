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

   public static MethodBuilder AddMethod(this PartialClassGenerator owner, string name)
   {
      return owner.AddMethod()
         .WithName(name);
   }

   public static MethodBuilder AddMethod(this PartialClassGenerator owner, string name, params (INamedTypeSymbol Type, string Name)[] parameters)
   {
      var methodBuilder = owner.AddMethod()
         .WithCondition(_ => !owner.ContainsMethod(name, parameters.Select(x => x.Type).ToArray()))
         .WithName(name);

      foreach (var parameter in parameters)
      {
         methodBuilder.WithParameter(
            () => parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            () => parameter.Name);
      }

      return methodBuilder;
   }

   public static PartialMethodBuilder AddPartialMethod(this PartialClassGenerator owner, string name,
      params (INamedTypeSymbol Type, string Name)[] parameters)
   {
      var partialMethod = owner.AddPartialMethod()
         .WithName(name);

      foreach (var parameter in parameters)
      {
         partialMethod.WithParameter(
            () => parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            () => parameter.Name);
      }

      return partialMethod;
   }

   #endregion
}