// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratorMode.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators;

/// <summary>Enum for configuring the source generator mode</summary>
internal enum GeneratorMode
{
   /// <summary>Mappers are generated for both ways, from left to right and back</summary>
   TwoWay,

   /// <summary>Only a mapper from left to right typ is generated</summary>
   LeftToRight,

   /// <summary>Only a mapper from right to left typ is generated</summary>
   RightToLeft
}