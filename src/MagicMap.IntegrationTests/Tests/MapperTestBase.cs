// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MapperTestBase.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests
{
    public class MapperTestBase<T>
        where T : new()
    {
        internal T Mapper { get; set; } = new T();
    }
}
