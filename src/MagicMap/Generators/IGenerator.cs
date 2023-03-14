// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMagicGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators;

using System.Collections.Generic;

internal interface IGenerator
{
   #region Public Methods and Operators

   /// <summary>Generates the sources of the generator.</summary>
   /// <returns>The generated source file of the generator</returns>
   IEnumerable<GeneratedSource> Generate();

    #endregion
}