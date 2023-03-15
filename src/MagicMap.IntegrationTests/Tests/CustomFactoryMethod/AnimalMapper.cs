// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimalMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.CustomFactoryMethod
{
    [TypeMapper(typeof(SourceAnimal), typeof(TargetAnimal))]
    internal partial class AnimalMapper
    {
        #region Constructors and Destructors

        public AnimalMapper()
            : this(true)
        {
        }

        private AnimalMapper(bool throwFailure)
        {
            if (throwFailure)
                throw new AssertFailedException("Should never be called with true");
        }

        #endregion

        #region Public Properties

        public static AnimalMapper Default => new(false);

        #endregion
    }
}
