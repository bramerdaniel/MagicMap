// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorSetup.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using FluentSetups.IntegrationTests.Targets;

namespace FluentSetups.IntegrationTests.Setups;

[FluentSetup(typeof(Color))]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public partial class ColorSetup
{
    #region Constants and Fields

    [FluentMember]
    private string name;

    #endregion

    #region Public Methods and Operators

    public ColorSetup WithName(string value)
    {
        // This method hides the generated one => it is not generated
        name = value;
        return this;
    }

    public ColorSetup WithOpacity(string value)
    {
        // This is an overload
        return WithOpacity(int.Parse(value));
    }

    #endregion
}
