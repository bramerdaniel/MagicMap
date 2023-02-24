// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SinglePropTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentSetups.IntegrationTests.Tests;

[TestClass]
public class SinglePropTests
{
    #region Public Methods and Operators

    [TestMethod]
    public void CreateSingleProp()
    {
        var car = Setup.SinglePropTarget()
            .WithBrand("BMW")
            .Done();

        car.Brand.Should().Be("BMW");
    }

    #endregion
}
