// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingOfReadonlyProperties.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests.MapperTests
{
    [TestClass]
    public class MappingOfReadonlyProperties
    {
        #region Public Methods and Operators


        [TestMethod]
        public void EnsureMappingFromLeftToRightWorksCorrectly()
        {
            var value = "Robert";


            var personModel = new Person { Name = value, Age = 34 }.ToPersonModel();
            personModel.Name.Should().Be(value);
            personModel.Age.Should().Be(34);
        }


        #endregion

  
   
    }
}
