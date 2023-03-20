// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Person.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.CustomizedPropertyMappingTest.CustomReadonlyProperty
{
    internal class Person
    {
        internal int Age { get; private set; }

        internal Person SetAge(int age)
        {
            Age = age;
            return this;
        }

        internal string Name { get; set; }

    }
}
