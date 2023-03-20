// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.CustomizedPropertyMappingTest
{
    [TypeMapper(typeof(Person), typeof(Animal))]
    partial class ObjectMapper
    {
        [PropertyMapper(typeof(Animal), nameof(Animal.AgeInMonths), nameof(Person.AgeInYears))]
        private void MapWithCustomName(Person person, Animal animal)
        {
            animal.AgeInMonths = person.AgeInYears * 12;
        }

        [PropertyMapper(typeof(Person), nameof(Person.AgeInYears), nameof(Animal.AgeInMonths))]
        private void AnyOtherName(Animal animal, Person person)
        {
            person.AgeInYears = animal.AgeInMonths / 12;
        }

        [PropertyMapper(typeof(Animal), nameof(Animal.Name), nameof(Person.Name))]
        private string PersonToAnimal(string personName)
        {
            return personName?.Replace("[Person]", "[Animal]");
        }

        [PropertyMapper(typeof(Person), nameof(Person.Name), nameof(Animal.Name))]
        private string AnimalToPerson(Animal animal)
        {
            return animal.Name?.Replace("[Animal]", "[Person]");
        }
    }
}
