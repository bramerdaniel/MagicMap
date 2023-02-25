// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMapper.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMapReferenceTester;

using MagicMap;

[TypeMapper(typeof(Person), typeof(PersonModel))]
[PropertyMapping(nameof(Person.AgeInYears), nameof(PersonModel.AgeInMonths))]
internal partial class PersonToModelMapper 
{
   partial void MapSize(PersonModel target, double value)
   {
      target.Size = (int)value;
   }

   partial void MapSize(Person target, int value)
   {
      target.Size= value;
   }
}