// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUniqueNameProvider.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

internal interface IUniqueNameProvider
{
   #region Public Methods and Operators

   string GetFileNameForClass(string hintClassName);

   #endregion
}