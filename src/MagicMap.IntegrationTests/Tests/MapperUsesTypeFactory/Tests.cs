// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoConstructorTests.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.MapperUsesTypeFactory
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void EnsureTypeFactoryIsUsedEvenIfConstructorWithoutParameterIsAvailable()
        {
            var source = new Source { Name = "Robert"};
            var target = source.ToTarget();

            target.Name.Should().Be("Robert");
            target.Creator.Should().Be(nameof(MapperWithTypeFactory));
        }

        [TestMethod]
        public void EnsureBackwardsNoTypeFactoryIsCalled()
        {
            var target = new Target { Name = "Robert" };
            var source = target.ToSource();

            source.Name.Should().Be("Robert");
        }
    }
}
