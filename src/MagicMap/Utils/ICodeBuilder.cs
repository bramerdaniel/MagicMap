// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICodeBuilder.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

internal interface ICodeBuilder
{
    #region Public Methods and Operators

    ICodeBuilder Append(string code);
   

    #endregion
}