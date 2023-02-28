// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedTypeSymbolExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

using System;
using System.Collections.Generic;

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
      return typeSymbol.GetMembers().OfType<IMethodSymbol>();
   }

   #endregion
}