// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimalMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.CustomFactoryMethod
{
    [TypeMapper(typeof(SourceAnimal), typeof(TargetAnimal))]
    partial class AnimalMapper
    {
        #region Constructors and Destructors

        public AnimalMapper()
            : this(true)
        {

        }

        public AnimalMapper(bool throwFailure)
        {
            if (throwFailure)
                throw new AssertFailedException("Should never be called with true");
        }

        #endregion

        #region Methods

        [MapperFactory]
        static AnimalMapper CreateDefaultMapper() => new(false);

        #endregion
    }
}
