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

   /// <summary>Gets the default constructor of a type.</summary>
   /// <param name="typeSymbol">The type symbol.</param>
   /// <returns></returns>
   /// <exception cref="System.ArgumentNullException">typeSymbol</exception>
   public static IMethodSymbol GetDefaultConstructor(this INamedTypeSymbol typeSymbol)
   {
      if (typeSymbol == null)
         throw new ArgumentNullException(nameof(typeSymbol));

      return typeSymbol.Constructors.FirstOrDefault(c => c.Parameters.Length == 0);
   }

   public static IEnumerable<IMethodSymbol> GetMethods(this INamedTypeSymbol typeSymbol)
   {
      if (typeSymbol == null)
         throw new ArgumentNullException(nameof(typeSymbol));

      return typeSymbol.GetMembers().OfType<IMethodSymbol>();
   }

   public static bool IsPrivate(this IMethodSymbol methodSymbol)
   {
      return methodSymbol.DeclaredAccessibility.HasFlag(Accessibility.Private);
   }

   public static IEnumerable<IMethodSymbol> GetMethods(this INamedTypeSymbol typeSymbol, string name)
   {
      if (typeSymbol == null)
         throw new ArgumentNullException(nameof(typeSymbol));

      return typeSymbol.GetMembers()
         .OfType<IMethodSymbol>()
         .Where(x => string.Equals(x.Name, name, StringComparison.InvariantCulture));
   }

   public static IEnumerable<(IMethodSymbol, AttributeData)> GetMethodsWithAttribute(this INamedTypeSymbol typeSymbol, INamedTypeSymbol attributeClass)
   {
      if (typeSymbol == null)
         throw new ArgumentNullException(nameof(typeSymbol));

      foreach (var methodSymbol in typeSymbol.GetMembers().OfType<IMethodSymbol>())
      {
         if (TryGetAttribute(methodSymbol, attributeClass, out var  attributeData))
            yield return (methodSymbol, attributeData);
      }
   }

   private static bool TryGetAttribute(IMethodSymbol methodSymbol, INamedTypeSymbol attributeClass, out AttributeData attributeData)
   {
      foreach (var candidate in methodSymbol.GetAttributes().Where(x => x != null))
      {
         if (attributeClass.Equals(candidate.AttributeClass, SymbolEqualityComparer.Default))
         {
            attributeData = candidate;
            return true;
         }
      }

      attributeData = null;
      return false;
   }

   public static IEnumerable<IPropertySymbol> GetProperties(this INamedTypeSymbol typeSymbol)
   {
      if (typeSymbol == null)
         throw new ArgumentNullException(nameof(typeSymbol));

      return typeSymbol.GetMembers().OfType<IPropertySymbol>();
   }

   public static IPropertySymbol GetProperty(this INamedTypeSymbol typeSymbol, Func<IPropertySymbol, bool> selector)
   {
      if (typeSymbol == null)
         throw new ArgumentNullException(nameof(typeSymbol));

      foreach (var method in typeSymbol.GetProperties())
      {
         if (selector(method))
            return method;
      }

      return null;
   }

   #endregion

   #region Methods

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