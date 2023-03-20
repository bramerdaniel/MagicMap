// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NonStaticDefaultMapperTests.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.InvalidOverrides.NonStaticDefaultMapper
{
    [TestClass]
    public class NonStaticDefaultMapperTests
    {
        [TestMethod]
        public void EnsureMappingsIsPossible()
        {
            var person = new Person { Name = "Sam" };
            person.Invoking(x => x.ToAnimal()).Should().NotThrow<NotSupportedException>();
            person.ToAnimal().Name.Should().Be("Sam");
        }
    }
}
