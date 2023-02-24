// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MugliTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentSetups.IntegrationTests.Tests
{
    [TestClass]
    public class MugliTests
    {
        [TestMethod]
        public void EnsureDefaultFieldValuesWorkCorrectly()
        {
            var target = Setup.Mugli()
                .WithName("Mugelo")
                .Done();
            
            target.Name.Should().Be("Mugelo");
            target.LightPower.Should().Be(20);
        }
    }
}
