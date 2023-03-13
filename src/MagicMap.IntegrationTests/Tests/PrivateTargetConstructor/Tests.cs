// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tests.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.PrivateTargetConstructor
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void EnsureTypeFactoryIsUsedEvenIfConstructorWithoutParameterIsAvailable()
        {
            var wobbler = new Source { Number = 66 };
            wobbler.Invoking(x => x.ToTarget()).Should().Throw<NotSupportedException>();
        }

    }
}
