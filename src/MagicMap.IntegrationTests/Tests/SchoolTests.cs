// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchoolTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

using FluentAssertions;

using FluentSetups.IntegrationTests.Targets;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentSetups.IntegrationTests.Tests
{
    [TestClass]
    public class ListSetupTests
    {
        #region Public Methods and Operators

        [TestMethod]
        public void SetupSchool()
        {
            var children = new List<Child> { new() { Name = "Rob" }, new() { Name = "Bob" } };

            var school = Setup.School()
                .WithChildren(children)
                .Done();

            school.Children.Should().HaveCount(2);
        }

        [TestMethod]
        public void SetupSchoolWithSingleChildren()
        {
            var school = Setup.School()
                .WithChild(new Child { Name = "Rob" })
                .WithChild(new Child { Name = "Bob" })
                .WithChild(new Child { Name = "John" })
                .Done();

            school.Children.Should().HaveCount(3);
        }

        [TestMethod]
        public void SetupSingleTargetWithIEnumerableConstructor()
        {
            var room = Setup.Room()
                .WithPerson(new Person { FirstName = "John", LastName = "Doe" })
                .WithPerson(new Person { FirstName = "Fred", LastName = "Roberts" })
                .Done();

            room.People.Should().HaveCount(2);
        }

        [TestMethod]
        public void SetupTargetsWithIEnumerableConstructor()
        {
            var persons = new List<Person> { new() { FirstName = "John", LastName = "Doe" }, new() { FirstName = "Fred", LastName = "Roberts" } };

            var room = Setup.Room()
                .WithPeople(persons)
                .Done();

            room.People.Should().HaveCount(2);
        }

        #endregion
    }
}
