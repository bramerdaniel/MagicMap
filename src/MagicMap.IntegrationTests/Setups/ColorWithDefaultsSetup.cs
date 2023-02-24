// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorWithDefaultsSetup.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentSetups.IntegrationTests.Targets;

namespace FluentSetups.IntegrationTests.Setups
{
#pragma warning disable CS0414
    [FluentSetup(typeof(Color), SetupMethod = "ColorWithDefaults")]
    public partial class ColorWithDefaultsSetup
    {
        #region Constants and Fields

        [FluentMember]
        private string name = string.Empty;

        private bool nameWasSet;

        [FluentMember]
        private int opacity = InitOpacity();

        #endregion

        #region Methods

        private static int InitOpacity()
        {
            return -1;
        }

        #endregion
    }
#pragma warning restore CS0414
}
