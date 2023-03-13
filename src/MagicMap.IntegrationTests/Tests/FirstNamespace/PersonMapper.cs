// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.FirstNamespace
{
    [TypeMapper(typeof(Person), typeof(PersonModel))]
    internal partial class PersonMapper
    {
    }

    // [TypeMapper(typeof(Person), typeof(TrowTarget))]
    internal partial class ThrowMupped
    {
        
    }

    internal class TrowTarget
    {
    }
}
