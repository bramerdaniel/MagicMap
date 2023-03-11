﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WobblerMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests
{
    [TypeMapper(typeof(Wobbler), typeof(WobblerModel))]
    internal partial class WobblerMapper
    {
        public string Name { get; set; }
    }
}
