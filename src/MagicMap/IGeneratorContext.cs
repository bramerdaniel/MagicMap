// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGeneratorData.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

internal interface IGeneratorContext
{
   string TargetName { get; }

   bool IsEnabled();
}