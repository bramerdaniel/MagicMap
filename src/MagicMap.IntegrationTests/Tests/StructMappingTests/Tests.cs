// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tests.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.StructMappingTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void EnsureMappingToAStructIsPossible()
        {
            var personClass = new PersonClass { Age = 12, Name = "Peter" };
            var personStruct = personClass.ToPersonStruct();

            personStruct.Name.Should().Be("Peter");
            personStruct.Age.Should().Be(12);

        }
    }
}
