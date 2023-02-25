// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMagicGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators;

internal interface IGenerator
{
    #region Public Methods and Operators

    GeneratedSource Generate();

    #endregion
}