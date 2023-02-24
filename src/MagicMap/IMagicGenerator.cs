// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMagicGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

internal interface IMagicGenerator
{
   #region Public Methods and Operators

   GeneratedSource Generate();

   #endregion
}