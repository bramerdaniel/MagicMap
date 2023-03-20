// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonModel.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.CustomizedPropertyMappingTest.CustomReadonlyProperty
{
    internal class PersonModel
    {
        internal int Age { get; private set; }

        internal void SetAge(int age) => Age = age;

        internal string Name { get; set; }
    }
}
