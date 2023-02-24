// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultValueTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Reflection;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentSetups.IntegrationTests.Tests
{
    [TestClass]
    public class DefaultValueTests
    {
        #region Public Methods and Operators

        [TestMethod]
        public void EnsureDefaultFieldValuesWorkCorrectly()
        {
            var target = Setup.DefaultField().Done();
            target.Age.Should().Be(10);
        }

        [TestMethod]
        public void EnsureDefaultPropertyValuesWorkCorrectly()
        {
            var target = Setup.DefaultProperty().Done();
            target.Age.Should().Be(20);
        }

        [TestMethod]
        public void EnsureGetAgeReturnsDefaultValueForFields()
        {
            var target = Setup.DefaultField();
            var getAge = target.GetType().GetMethod("GetAge", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(getAge);
            var age = (int)getAge.Invoke(target, new object[] { })!;

            age.Should().Be(10);
        }

        [TestMethod]
        public void EnsureGetAgeReturnsDefaultValueForProperties()
        {
            var target = Setup.DefaultProperty();
            var getAge = target.GetType().GetMethod("GetAge", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(getAge);
            var age = (int)getAge.Invoke(target, new object[] { })!;

            age.Should().Be(20);
        }

        #endregion
    }

    [FluentSetup(typeof(DefaultTarget), SetupMethod = "DefaultField")]
    internal partial class DefaultField
    {
        #region Constants and Fields

        [FluentMember]
        private int age = 10;

        #endregion

        #region Methods

        protected void SetAgeWasSet()
        {
            if (ageWasSet)
                throw new InvalidOperationException();
            ageWasSet = true;
        }

        #endregion
    }

    [FluentSetup(typeof(DefaultTarget), SetupMethod = "DefaultProperty")]
    internal partial class DefaultProperty
    {
        #region Properties

        [FluentMember] private int Age { get; set; } = 20;

        #endregion

        #region Methods

        protected void SetAgeWasSet()
        {
            if (ageWasSet)
                throw new InvalidOperationException();
            ageWasSet = true;
        }

        #endregion
    }

    internal class DefaultTarget
    {
        #region Public Properties

        public int Age { get; set; }

        #endregion
    }
}
