// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiPropTarget.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace FluentSetups.IntegrationTests.Targets;

public class MultiPropTarget
{
    #region Public Properties

    public int Kind { get; set; }

    public string Name { get; set; }

    public Type PropertyType { get; set; }

    #endregion
}
