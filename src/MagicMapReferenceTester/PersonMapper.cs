// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMapper.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMapReferenceTester;

using MagicMap;

[TypeMapper(typeof(Person), typeof(PersonModel))]
internal partial class PersonMapper 
{
   partial void MapAge(PersonModel target, double value)
   {
      target.Age = (int)value;
   }
}

class MapperUsage
{
    public MapperUsage()
    {
        new PersonMapper().Map(new Person(), new PersonModel());
    }
}