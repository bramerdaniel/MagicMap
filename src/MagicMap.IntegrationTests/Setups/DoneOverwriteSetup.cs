// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoneOverwriteSetup.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentSetups.IntegrationTests.Targets;

namespace FluentSetups.IntegrationTests.Setups;

[FluentSetup(typeof(Person), EntryClassName = "CustomSetup")]
internal partial class DoneOverwriteSetup
{
    #region Methods

    internal Person Done()
    {
        return new Person { FirstName = GetLastName(null), LastName = GetFirstName(null), Age = GetAge(() => 10) };
    }

    #endregion
}
