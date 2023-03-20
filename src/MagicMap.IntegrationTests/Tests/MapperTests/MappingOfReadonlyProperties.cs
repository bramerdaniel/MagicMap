// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingOfReadonlyProperties.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.MapperTests
{
    [TestClass]
    public class MappingOfTargetTypeProperties
    {
        #region Public Methods and Operators

        [TestMethod]
        public void EnsureMappingFromLeftToRightWorksCorrectly()
        {
            var person = new Person();
            PersonMapper.Default.Map(new PersonModel { Name = "Model" }, person);
            person.Name.Should().Be("Model");
            person.Age.Should().Be(66);
        }

        [TestMethod]
        public void EnsureMappingFromRightToLeftWorksCorrectly()
        {
            var personModel = new PersonModel();
            PersonMapper.Default.Map(new Person { Name = "Person", Age = 23 }, personModel);
            personModel.Name.Should().Be("Person");
            personModel.Age.Should().Be(66);
        }


        #endregion



    }
}
