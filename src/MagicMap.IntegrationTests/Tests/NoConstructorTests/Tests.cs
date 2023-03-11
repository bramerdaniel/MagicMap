// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoConstructorTests.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests
{
    [TestClass]
    public class NoConstructorTests
    {
        [TestMethod]
        public void Name()
        {
            var wobbler = new Wobbler(){ Name = "Robert"};
            wobbler.ToWobblerModel().Name.Should().Be("Robert");
        }
    }

    internal class Wobbler
    {
        public string Name { get; set; }
    }

    internal class WobblerModel
    {
        public WobblerModel(bool withFlag)
        {
        }

        public string Name { get; set; }
    }
}
