// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using System.Collections.Generic;
   using System.Collections.Immutable;
   using System.Linq;

   internal static class EnumerableExtensions
   {
      public static IEnumerable<T> WhereNotNull<T>(this ImmutableArray<T> source)
         where T : class
      {
         return source.Where(it => it !=null);
      }
   }
}