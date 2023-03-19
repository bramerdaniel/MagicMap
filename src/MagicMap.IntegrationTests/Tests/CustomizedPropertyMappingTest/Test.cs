// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Test.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.CustomizedPropertyMappingTest
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void EnsureForwardMappingWorksCorrectly()
        {
            var person = new Person{ AgeInYears = 10 };
            person.ToAnimal().AgeInMonths.Should().Be(120);
        }

        [TestMethod]
        public void EnsureBackwardMappingWorksCorrectly()
        {
            var animal = new Animal{ AgeInMonths= 132 };
            animal.ToPerson().AgeInYears.Should().Be(11);
        }


        [TestMethod]
        public void MapPropertyWithSameNameForward()
        {
            var person = new Person{ Name = "Robert [Person]"};
            person.ToAnimal().Name.Should().Be("Robert [Animal]");
        }

        [TestMethod]
        public void MapPropertyWithSameNameBackward()
        {
            var animal = new Animal{ Name = "Robert [Animal]" };
            animal.ToPerson().Name.Should().Be("Robert [Person]");
        }


    }
}
