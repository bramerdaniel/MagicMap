// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverrideDoneTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentSetups.IntegrationTests.Tests;

[TestClass]
public class OverrideDoneTests
{
    #region Public Methods and Operators

    [TestMethod]
    public void EnsureOverwrittenDoneCompiles()
    {
        var person = CustomSetup.Person()
            .WithFirstName("Robert")
            .WithLastName("Ramirez")
            .Done();

        person.FirstName.Should().Be("Ramirez");
        person.LastName.Should().Be("Robert");
        person.Age.Should().Be(10);
    }

    #endregion
}
