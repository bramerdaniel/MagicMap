// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiplePartialMembers.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace FluentSetups.IntegrationTests.Setups;

[FluentSetup]
[SuppressMessage("IDE", "IDE0044:Add readonly modifier")]
[SuppressMessage("IDE", "IDE0052:Remove unused private members")]
public partial class MultiplePartialMembers
{
    #region Constants and Fields

    [FluentMember]
#pragma warning disable CS0414
    private string value = string.Empty;
#pragma warning restore CS0414

    #endregion
}

public partial class MultiplePartialMembers
{
}
