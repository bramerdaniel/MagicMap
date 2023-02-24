// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DuplicateSetupMethodNames.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentSetups.IntegrationTests.Tests;

[TestClass]
public class DuplicateSetupMethodNames
{
    #region Public Methods and Operators

    [TestMethod]
    public void EnsureOverwrittenDoneCompiles()
    {
        Setup.Olga().Done().LastName.Should().Be("OlgaSetup");
        Setup.Olga1().Done().LastName.Should().Be("AnotherOlgaSetup");
    }

    #endregion
}
