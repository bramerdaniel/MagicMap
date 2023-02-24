// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonSetup.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentSetups.IntegrationTests.Targets;

namespace FluentSetups.IntegrationTests.Setups;

[FluentSetup(typeof(Person))]
internal partial class PersonSetup
{
    #region Public Properties

    [FluentMember] public string FirstName { get; set; }

    #endregion

    #region Public Methods and Operators

    public PersonSetup WithDefaults()
    {
        return WithFirstName("Lila")
            .WithLastName("Sheer");
    }

    #endregion
}
