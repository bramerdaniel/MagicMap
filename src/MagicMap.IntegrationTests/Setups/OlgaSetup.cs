// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OlgaSetup.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentSetups.IntegrationTests.Targets;

namespace FluentSetups.IntegrationTests.Setups
{
    [FluentSetup(typeof(Olga))]
    public partial class OlgaSetup
    {
        #region Methods

        private void SetupTarget(Olga target)
        {
            target.LastName = "OlgaSetup";
        }

        #endregion
    }

    [FluentSetup(typeof(Olga), SetupMethod = "Olga1")]
    public partial class AnotherOlgaSetup
    {
        #region Methods

        private void SetupTarget(Olga target)
        {
            target.LastName = "AnotherOlgaSetup";
        }

        #endregion
    }
}
