// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomizedAnimalMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace MagicMap.IntegrationTests.Tests.CustomFactoryMethod
{
    [TypeMapper(typeof(SourceHandler), typeof(TargetHandler))]
    internal partial class HandlerMapper
    {
        public HandlerMapper()
        {
            throw new AssertFailedException("Should never be called with true");
        }

        public HandlerMapper(bool flag)
        {
        }
    }

    internal static partial class HandlerMapperExtensions
    {
        // private static HandlerMapper Mapper => new HandlerMapper(true);
    }
}
