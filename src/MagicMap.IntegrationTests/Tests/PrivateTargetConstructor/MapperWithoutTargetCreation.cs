// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.PrivateTargetConstructor
{
    [TypeMapper(typeof(Source), typeof(Target))]
    internal partial class MapperWithoutTargetCreation
    {
    }
}
