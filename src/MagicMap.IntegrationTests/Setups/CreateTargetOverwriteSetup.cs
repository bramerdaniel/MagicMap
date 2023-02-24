// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateTargetOverwriteSetup.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentSetups.IntegrationTests.Targets;

namespace FluentSetups.IntegrationTests.Setups;

[FluentSetup(typeof(Person), SetupMethod = "CustomPerson")]
internal partial class CreateTargetOverwriteSetup
{
    #region Public Properties

    public bool CreateTargetCalled { get; set; }

    #endregion

    #region Methods

    internal Person CreateTarget()
    {
        CreateTargetCalled = true;
        return new Person { FirstName = GetLastName(null), LastName = GetFirstName(null), Age = GetAge(() => 10) };
    }

    #endregion
}
