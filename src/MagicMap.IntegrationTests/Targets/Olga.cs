// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Olga.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace FluentSetups.IntegrationTests.Targets;

record Spider(IEnumerable<string> Legs);

[FluentSetup(typeof(Spider))]
public partial class SpiderSetup
{
    #region Constants and Fields

    [FluentMember]
    private List<string> legs;

    #endregion
}

public class Olga
{
    #region Public Properties

    public string LastName { get; set; }

    #endregion
}
