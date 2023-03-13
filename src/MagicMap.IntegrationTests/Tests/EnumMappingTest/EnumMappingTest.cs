// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumMappingTest.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using MagicMap.IntegrationTests.Tests.FirstNamespace;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.EnumMappingTest
{
    [TestClass]
    public class MapByName
    {
        [TestMethod]
        public void MapEnumByName()
        {
            MapPersonType(PersonType.Normal).Should().Be(SecondNamespace.PersonType.Normal);
            MapPersonType(PersonType.Superman).Should().Be(SecondNamespace.PersonType.Superman);
            MapPersonType(PersonType.Alien).Should().Be(SecondNamespace.PersonType.Alien);
        }

        private static SecondNamespace.PersonType MapPersonType(PersonType personType)
        {
            return new PersonContainer { Type = personType }
                .ToPersonContainer().Type;
        }
    }

    [TypeMapper(typeof(FirstNamespace.PersonContainer), typeof(SecondNamespace.PersonContainer))]
    partial class ContainerMapper
    {
        
    }
}
