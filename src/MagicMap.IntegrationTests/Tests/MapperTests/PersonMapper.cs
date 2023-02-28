// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.MapperTests
{
    [TypeMapper(typeof(Person), typeof(PersonModel))]
    internal partial class PersonMapper2
    {
    }
}
