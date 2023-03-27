// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.StructMappingTests
{
    [TypeMapper(typeof(PersonClass), typeof(PersonStruct))]
    public partial class PersonMapper
    {
    }
}
