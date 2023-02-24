// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using System;

   internal static class StringExtensions
   {
      internal static string ToFirstLower(this string value)
      {
         if (value == null)
            throw new ArgumentNullException(nameof(value));

         return $"{char.ToLowerInvariant(value[0])}{value.Substring(1)}";
      }

      internal static string ToFirstUpper(this string value)
      {
         if (value == null)
            throw new ArgumentNullException(nameof(value));

         return $"{char.ToUpperInvariant(value[0])}{value.Substring(1)}";
      }

   }
}