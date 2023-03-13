// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonModel.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.MapperTests
{
    internal class PersonModel
    {
        public PersonModel()
        {
            Age = 66;
        }

        #region Public Properties

        public string Name { get; set; }

        public int Age { get; }

        #endregion
    }
}
