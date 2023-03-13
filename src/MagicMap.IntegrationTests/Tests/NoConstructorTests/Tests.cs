// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoConstructorTests.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.NoConstructorTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void Name()
        {
            var wobbler = new Wobbler(){ Name = "Robert"};
            wobbler.ToWobblerModel().Name.Should().Be("Robert");
        }
    }
}
