// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverrideCreateTargetTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentSetups.IntegrationTests.Tests;

[TestClass]
public class OverrideCreateTargetTests
{
    #region Public Methods and Operators

    [TestMethod]
    public void EnsureOverwrittenDoneCompiles()
    {
        var setup = Setup.CustomPerson()
            .WithFirstName("Robert")
            .WithLastName("Ramirez");

        setup.CreateTargetCalled.Should().BeFalse();

        var person = setup.Done();

        setup.CreateTargetCalled.Should().BeTrue();
        person.FirstName.Should().Be("Robert");
        person.LastName.Should().Be("Ramirez");
        person.Age.Should().Be(10);
    }

    #endregion
}
