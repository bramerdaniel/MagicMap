// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedTypeSymbolExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

public static class NamedTypeSymbolExtensions
{
   #region Public Methods and Operators

   public static IMethodSymbol GetMethod(this INamedTypeSymbol typeSymbol, Func<IMethodSymbol, bool> selector)
   {
      if (typeSymbol == null)
         throw new ArgumentNullException(nameof(typeSymbol));

      foreach (var method in typeSymbol.GetMethods())
      {
         if (selector(method))
            return method;
      }

      return null;
   }

   public static IEnumerable<IMethodSymbol> GetMethods(this INamedTypeSymbol typeSymbol)
   {
      if (typeSymbol == null)
         throw new ArgumentNullException(nameof(typeSymbol));

      return typeSymbol.GetMembers().OfType<IMethodSymbol>();
   }

   public static IEnumerable<IMethodSymbol> GetMethods(this INamedTypeSymbol typeSymbol, string name)
   {
      if (typeSymbol == null)
         throw new ArgumentNullException(nameof(typeSymbol));

      return typeSymbol.GetMembers()
         .OfType<IMethodSymbol>()
         .Where(x => string.Equals(x.Name, name, StringComparison.InvariantCulture));
   }

   public static IMethodSymbol FindMethod(this INamedTypeSymbol typeSymbol, string name, ITypeSymbol returnType, params ITypeSymbol[] parameters)
   {
      var candidates = GetMethods(typeSymbol, name)
         .Where(x => x.Parameters.Length == parameters.Length)
         .ToArray();

      if (returnType != null)
         candidates = candidates.Where(x => x.ReturnType.Equals(returnType, SymbolEqualityComparer.Default)).ToArray();

      foreach (var candidate in candidates)
      {
         if (ParametersTypesMatch(candidate, parameters))
            return candidate;
      }

      return null;
   }

   private static bool ParametersTypesMatch(IMethodSymbol method, ITypeSymbol[] parameters)
   {
      if (parameters.Length != method.Parameters.Length)
         return false;

      for (var i = 0; i < parameters.Length; i++)
      {
         if (!parameters[i].Equals(method.Parameters[i].Type, SymbolEqualityComparer.Default))
            return false;
      }

      return true;
   }

   #endregion
}