// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HideMethodColorSetup.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentSetups.IntegrationTests.Targets;

namespace FluentSetups.IntegrationTests.Setups;

[FluentSetup(typeof(Color))]
public partial class HideMethodColorSetup
{
    #region Public Methods and Operators

    public HideMethodColorSetup WithName(string value)
    {
        // This method hides the generated one => it is not generated
        name = value;
        return this;
    }

    public HideMethodColorSetup WithOpacity(string value)
    {
        // This is an overload
        return WithOpacity(int.Parse(value));
    }

    #endregion
}
