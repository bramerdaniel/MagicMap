// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.InvalidOverrides.NonStaticDefaultMapper
{
    [TypeMapper(typeof(Person), typeof(Animal))]
    internal partial class InvalidMapper
    {
        public InvalidMapper Default => new InvalidMapper();
    }
}
