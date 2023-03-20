// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.CustomizedPropertyMappingTest.CustomReadonlyProperty
{
    [TypeMapper(typeof(Person), typeof(PersonModel))]
    internal partial class PersonMapper
    {
        [PropertyMapper(typeof(PersonModel), nameof(PersonModel.Age))]
        private void MapAge(Person person, PersonModel personModel)
        {
            personModel.SetAge(person.Age);
        }

        [PropertyMapper(typeof(Person), nameof(Person.Age))]
        private void MapAge(int age, Person person)
        {
            person.SetAge(age);
        }
    }
}
