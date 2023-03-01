// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MagicContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

internal struct GeneratorContext : IGeneratorContext
{
   public string TargetName => "EmptyContext"; 

   public bool IsEnabled() => false;

   public static GeneratorContext Empty { get; } = new();
}