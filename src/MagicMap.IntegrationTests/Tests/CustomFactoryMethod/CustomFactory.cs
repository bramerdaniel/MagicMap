// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomFactory.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.CustomFactoryMethod
{
    [TestClass]
    public class MapByName
    {
        #region Public Methods and Operators

        [TestMethod]
        public void EnsureExtensionClassUsedCustomFactory()
        {
            new SourceHandler().Invoking(x => x.ToTargetHandler())
                .Should().NotThrow();
        }

        [TestMethod]
        public void EnsureSingletonUsesClassUsedCustomFactory()
        {
            this.Invoking(t => AnimalMapper.Default.Map(new SourceAnimal(), new TargetAnimal()))
                .Should().NotThrow();
        }

        #endregion
    }
}
