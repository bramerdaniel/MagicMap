// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonModel.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.ReferenceTypeMappings.RecursiveMappingTests
{
    public class PersonModel
    {
        public string Name { get; set; }

        public PersonModel Father { get; set; }

        public PersonModel Mother { get; set; }

        public PersonModel Son { get; set; }
    }
}
