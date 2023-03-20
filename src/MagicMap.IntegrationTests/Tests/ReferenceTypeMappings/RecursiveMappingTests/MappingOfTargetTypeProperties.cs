// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingOfReadonlyProperties.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.ReferenceTypeMappings.RecursiveMappingTests
{
    [TestClass]
    public class MappingOfTargetTypeProperties
    {
        #region Public Methods and Operators

        [TestMethod]
        public void EnsurePropertiesOfTargetTypeAreCloned()
        {
            var person = new Person { Name = "Miles", Mother = new Person { Name = "Cathy" } };
            var personModel = person.ToPersonModel();

            personModel.Name.Should().Be("Miles");
            personModel.Mother.Name.Should().Be("Cathy");
        }

        [TestMethod]
        public void EnsurePropertiesAreCloned()
        {
            var person = new Person { Name = "Miles", Father = new Person { Name = "Bob" } };
            var personModel = person.ToPersonModel();

            personModel.Name.Should().Be("Miles");
            personModel.Father.Name.Should().Be("Bob");
            personModel.Father.Son.Name.Should().Be("Miles");
            personModel.Father.Son.Should().BeSameAs(personModel);
        }

        #endregion
    }
}
