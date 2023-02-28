// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentSetupTests.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests
{
    [TestClass]
    public class NormalMappingExtensionMethodTests
    {
        #region Public Methods and Operators

        [TestMethod]
        public void EnsureMappingFromLeftToRightWorksCorrectly()
        {
            var value = "Robert";
            var personModel = new Person { Name = value , Age = 34}.ToPersonModel();
            personModel.Name.Should().Be(value);
            personModel.Age.Should().Be(34);
        }

        [TestMethod]
        public void EnsureMappingFromRightToLeftWorksCorrectly()
        {
            var value = "Robert";
            var person = new PersonModel { Name = value, Age = 42 }.ToPerson();
            person.Age.Should().Be(42);

            person.Name.Should().Be(value);
        }

        #endregion

        [TypeMapper(typeof(Person), typeof(PersonModel))]
        internal partial class PersonMapper
        {
        }

        internal class Person
        {
            #region Public Properties

            public string Name { get; set; }
            
            public int Age{ get; set; }

            #endregion
        }

        internal class PersonModel
        {
            #region Public Properties

            public string Name { get; set; }

            public int Age { get; set; }

            #endregion
        }
    }
}
