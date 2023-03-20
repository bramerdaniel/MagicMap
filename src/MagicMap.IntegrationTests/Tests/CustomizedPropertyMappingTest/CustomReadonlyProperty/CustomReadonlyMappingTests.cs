// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomReadonlyMappingTests.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.CustomizedPropertyMappingTest.CustomReadonlyProperty
{
    [TestClass]
    public class CustomReadonlyMappingTests
    {
        [TestMethod]
        public void EnsureCustomMappingIsCalledForReadonlyProperties()
        {
            var person = new Person { Name = "Miles" }.SetAge(34);
            var personModel = person.ToPersonModel();

            personModel.Name.Should().Be("Miles");
            personModel.Age.Should().Be(34);
        }

        [TestMethod]
        public void EnsureCustomMappingBackwardIsCalledForReadonlyProperties()
        {
            var personModel = new PersonModel { Name = "Miles" };
            personModel.SetAge(34);
            var person = personModel.ToPerson();

            person.Name.Should().Be("Miles");
            person.Age.Should().Be(34);
        }
    }
}
