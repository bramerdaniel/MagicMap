// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompileErrorTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentSetups.IntegrationTests.Tests;

/// <summary>Test class that ensures that all generated member are available, otherwise it would not compile</summary>
[TestClass]
public class CompileErrorTests
{
    #region Public Methods and Operators

    [TestMethod]
    public void EnsureMultipleTargetPropertiesGenerateFluentMethods()
    {
        var target = Setup.MultiPropTarget()
            .WithName("Brand")
            .WithKind(3)
            .WithPropertyType(typeof(string))
            .Done();

        target.Should().NotBeNull();
    }

    [TestMethod]
    public void EnsureOverloadExists()
    {
        var car = Setup.Color()
            .WithName("Name")
            .WithOpacity(5)
            .WithOpacity("7")
            .Done();

        car.Should().NotBeNull();
    }

    [TestMethod]
    public void EnsureSingleTargetPropertyIsGeneratedCorrectly()
    {
        var target = Setup.SinglePropTarget()
            .WithBrand("Brand")
            .Done();

        target.Should().NotBeNull();
    }

    #endregion
}
