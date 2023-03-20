// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.ReferenceTypeMappings.RecursiveMappingTests
{
    [TypeMapper(typeof(Person), typeof(PersonModel))]
    internal partial class PersonMapper
    {
        [PropertyMapper(typeof(PersonModel), nameof(PersonModel.Father))]
        private void MapFather(Person person, PersonModel personModel)
        {
            personModel.Father = new PersonModel { Name = person.Father?.Name, Son = personModel };
        }
    }
}
