// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NonStaticDefaultMapperTests.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.InvalidOverrides.NonStaticDefaultMapperAndNoConstructor
{
    [TestClass]
    public class NonStaticDefaultMapperTests
    {
        [TestMethod]
        public void EnsureMappingsIsNotLongerPossibleAsNoMapperCanBeCreated()
        {
            var person = new Person { Name = "Sam" };
            person.Invoking(x => x.ToAnimal()).Should().Throw<NotSupportedException>();
        }
    }
}
