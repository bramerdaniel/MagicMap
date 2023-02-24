// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Setup.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentSetups.IntegrationTests.Setups;

namespace FluentSetups.IntegrationTests
{
    public partial class Setup
    {
        public static MugliSetup Mugli() => new MugliSetup().WithLightPower(20);
    }
}
