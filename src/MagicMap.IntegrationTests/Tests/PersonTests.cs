// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentSetups.IntegrationTests.Tests;

[TestClass]
public class PersonTests
{
    #region Public Methods and Operators

    [TestMethod]
    public void SetupPersonWithCustomDefaultName()
    {
        var person = Setup.Person()
            .WithDefaults()
            .Done();

        person.FirstName.Should().Be("Lila");
        person.LastName.Should().Be("Sheer");
    }

    [TestMethod]
    public void SetupPersonWithDefaultName()
    {
        var person = Setup.PersonWithDefaultName()
            .Done();

        person.FirstName.Should().Be("John");
        person.LastName.Should().Be("Doe");
    }

    [TestMethod]
    public void SetupThatIsHidingAPublicProperty()
    {
        var person = Setup.Person()
            .WithFirstName("Robert")
            .WithLastName("Ramirez")
            .WithAge(34)
            .Done();

        person.FirstName.Should().Be("Robert");
        person.LastName.Should().Be("Ramirez");
        person.Age.Should().Be(34);
    }

    #endregion
}
