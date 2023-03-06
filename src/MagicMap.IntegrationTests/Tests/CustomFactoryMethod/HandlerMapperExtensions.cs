// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandlerMapperExtensions.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.CustomFactoryMethod
{
    internal static partial class HandlerMapperExtensions
    {
        internal static HandlerMapper Mapper => new HandlerMapper(true);
    }
}
