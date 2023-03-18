// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonToPersonMapper.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMapReferenceTester;

using MagicMap;

[TypeMapper(typeof(Person), typeof(PersonModel))]
internal partial class PersonMapper
{
   [PropertyMapping(nameof(Person.Name), nameof(PersonModel.Name))]
   private string ToPersonModelName(Person person)
   {
      return person.Name.Replace("[Person]", "[Animal]");
   }

   [PropertyMapping(nameof(PersonModel.Name), nameof(Person.Name))]
   private string ToPersonName(PersonModel person)
   {
      return person.Name.Replace("[Person]", "[Animal]");
   }

   [PropertyMapping(nameof(PersonModel.Age), nameof(Person.Age))]
   private long ConvertAge(int age)
   {
      return age;
   }

   [PropertyMapping(nameof(PersonModel.Age), nameof(Person.Age))]
   private int ConvertAge(long age)
   {
      return (int)age;
   }
}