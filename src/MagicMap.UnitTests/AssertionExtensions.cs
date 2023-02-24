// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertionExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests;

using MagicMap.UnitTests.Assertions;
using MagicMap.UnitTests.Setups;

internal static class AssertionExtensions
{
   #region Public Methods and Operators

   public static GenerationResultAssertion Should(this GenerationResult target)
   {
      return new GenerationResultAssertion(target);
   }



   #endregion
}