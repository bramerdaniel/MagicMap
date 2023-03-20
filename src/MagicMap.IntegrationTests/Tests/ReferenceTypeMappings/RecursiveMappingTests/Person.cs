// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Person.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.ReferenceTypeMappings.RecursiveMappingTests
{
    public class Person
    {
        public string Name { get; set; }

        public Person Father { get; set; }
        public Person Mother { get; set; }
    }
}
