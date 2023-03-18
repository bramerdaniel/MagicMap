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

    }
}
