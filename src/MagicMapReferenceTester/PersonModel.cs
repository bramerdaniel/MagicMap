// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonModel.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMapReferenceTester;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class PersonModel
{
   public string Name { get; set; } = null!;

   public int Age { get; set; }
   
}