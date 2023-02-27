// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Room.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace MagicMap.IntegrationTests.Targets
{
    public record Room(IEnumerable<Person> People);
}
