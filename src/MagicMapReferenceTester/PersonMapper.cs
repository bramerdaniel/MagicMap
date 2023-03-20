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
   [PropertyMapper(typeof(PersonModel), nameof(PersonModel.Name), nameof(Person.Name))]
   private string ToPersonModelName(Person person)
   {
      return person.Name.Replace("[Person]", "[Animal]");
   }

   [PropertyMapper(typeof(Person), nameof(Person.Name), nameof(PersonModel.Name))]
   private string ToPersonName(PersonModel person)
   {
      return person?.Name?.Replace("[Person]", "[Animal]");
   }

   [PropertyMapper(typeof(PersonModel), nameof(Person.Age), nameof(PersonModel.Age))]
   private long ConvertAge(int age)
   {
      return age;
   }

   [PropertyMapper(typeof(Person), nameof(Person.Age), nameof(PersonModel.Age))]
   private int ConvertAge(long age)
   {
      return (int)age;
   }
}