// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingOfReadonlyProperties.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MagicMap.IntegrationTests.Tests
{
    [TestClass]
    public class MappingOfReadonlyProperties
    {
        #region Public Methods and Operators

        
        private static PartialPersonMapper CreateTarget()
        {
            return new PartialPersonMapper();
        }

        [TestMethod]
        public void EnsureMappingFromLeftToRightWorksCorrectly()
        {
            var value = "Robert";
            //var personModel = CreateTarget()..Map(new Person { Name = value , Age = 34});
            //personModel.Name.Should().Be(value);
            //personModel.Age.Should().Be(34);
        }


        #endregion

        [TypeMapper(typeof(Person), typeof(PersonModel))]
        internal partial class PartialPersonMapper
        {
        }

        internal class Person
        {
            #region Public Properties

            public string Name { get; set; }
            
            public int Age{ get; set; }

            #endregion
        }

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
}
