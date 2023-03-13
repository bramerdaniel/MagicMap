// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WobblerMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.MapperUsesTypeFactory
{
    [TypeMapper(typeof(Source), typeof(Target))]
    internal partial class MapperWithTypeFactory : ITypeFactory<Target, Source>
    {
        public Target Create(Source source) => new(nameof(MapperWithTypeFactory));
    }
}
