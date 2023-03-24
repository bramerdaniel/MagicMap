// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratorModeTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.SourceGeneratorTests;

using MagicMap.UnitTests.Setups;

[TestClass]
public class GeneratorModeTests
{
   #region Public Methods and Operators

   [TestMethod]
   public void EnsureGeneratorModeLeftToRightWorksCorrectly()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(Person), typeof(PersonModel), Mode = GeneratorMode.LeftToRight)]
                   internal partial class PersonMapper { }
                       
                   internal class Person {  }

                   internal class PersonModel {  }         
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();

      result.Should().HaveClass("PersonMapper")
         .WithMethod("Map", "Person", "PersonModel")
         .WithMethod("MapFrom", "Person")
         .WithMethod("MapOverride", "Person", "PersonModel")
         .WithoutMethod("Map", "PersonModel", "Person")
         .WithoutMethod("MapFrom", "PersonModel")
         .WithoutMethod("MapOverride", "PersonModel", "Person");

      result.Should().HaveClass("PersonMapperExtensions")
         .WithMethod("ToPersonModel", "Person")
         .WithoutMethod("ToPerson", "PersonModel");

      result.Print();
   }

   [TestMethod]
   public void EnsureGeneratorModeRightToLeftWorksCorrectly()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(Person), typeof(PersonModel), Mode = GeneratorMode.RightToLeft)]
                   internal partial class PersonMapper { }
                       
                   internal class Person {  }

                   internal class PersonModel {  }         
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();

      result.Should().HaveClass("PersonMapper")
         .WithMethod("Map", "PersonModel", "Person")
         .WithMethod("MapFrom", "PersonModel")
         .WithMethod("MapOverride", "PersonModel", "Person")
         .WithoutMethod("Map", "Person", "PersonModel")
         .WithoutMethod("MapFrom", "Person")
         .WithoutMethod("MapOverride", "Person", "PersonModel");

      result.Should().HaveClass("PersonMapperExtensions")
         .WithMethod("ToPerson", "PersonModel")
         .WithoutMethod("ToPersonModel", "Person");

      result.Print();
   }

   [TestMethod]
   public void EnsureGeneratorModeTwoWaySetExplicitlyWorksCorrectly()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(Person), typeof(PersonModel), Mode = GeneratorMode.TwoWay)]
                   internal partial class PersonMapper { }
                       
                   internal class Person {  }

                   internal class PersonModel {  }         
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();

      result.Should().HaveClass("PersonMapper")
         .WithMethod("Map", "Person", "PersonModel")
         .WithMethod("MapFrom", "Person")
         .WithMethod("Map", "PersonModel", "Person")
         .WithMethod("MapFrom", "PersonModel");

      result.Should().HaveClass("PersonMapperExtensions")
         .WithMethod("ToPersonModel", "Person")
         .WithMethod("ToPerson", "PersonModel");

      result.Print();
   }

   #endregion
}