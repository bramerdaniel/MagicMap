// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtorArgsTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentSetups.IntegrationTests.Tests;

[TestClass]
public class CtorArgsTests
{
    #region Public Methods and Operators

    [TestMethod]
    public void EnsureMissingFirstConstructorArgumentThrows()
    {
        var setup = Setup.CtorArgs()
            .WithSecond(5)
            .WithThird(true);

        setup.Invoking(s => s.Done()).Should()
            .Throw<SetupMemberNotInitializedException>()
            .Where(e => e.MemberName == "first");
    }

    [TestMethod]
    public void EnsureMissingSecondConstructorArgumentThrows()
    {
        var setup = Setup.CtorArgs()
            .WithFirst("Hans")
            .WithThird(true);

        setup.Invoking(s => s.Done()).Should()
            .Throw<SetupMemberNotInitializedException>()
            .Where(e => e.MemberName == "second");
    }

    [TestMethod]
    public void EnsureMissingThirdConstructorArgumentThrows()
    {
        var setup = Setup.CtorArgs()
            .WithFirst("Hans")
            .WithSecond(5);

        setup.Invoking(s => s.Done()).Should()
            .Throw<SetupMemberNotInitializedException>()
            .Where(e => e.MemberName == "third");
    }

    [TestMethod]
    public void EnsureTheConstructorIsCalledCorrectly()
    {
        var ctorArgs = Setup.CtorArgs()
            .WithFirst("Hans")
            .WithSecond(5)
            .WithThird(true)
            .Done();

        ctorArgs.First.Should().Be("Hans");
        ctorArgs.Second.Should().Be(5);
        ctorArgs.Third.Should().Be(true);
    }

    #endregion
}
