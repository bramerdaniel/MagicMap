// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentSetupTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

using FluentAssertions;

using FluentSetups.IntegrationTests.Targets;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentSetups.IntegrationTests.Tests
{
    [TestClass]
    public class ArgumentSetupTests
    {
        #region Public Methods and Operators

        [TestMethod]
        public void SetupArgumentWithValue()
        {
            var argument = Setup.Argument()
                .WithName("FilePath")
                .WithValue("SomeFilePath.cs")
                .Done();

            argument.Name.Should().Be("FilePath");
            argument.Value.Should().Be("SomeFilePath.cs");
        }

        #endregion
    }
}
