// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForceFlagTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.SourceGeneratorTests;

using MagicMap.UnitTests.Setups;

[TestClass]
public class ForceFlagTests
{
   #region Public Methods and Operators

   [TestMethod]
   public void EnsureNotMatchingTypesAreForced()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(Person), typeof(PersonModel), ForceMappings = true)]
                   internal partial class PersonMapper { }
                    
                   internal class Person 
                   { 
                       public int Age { get; set; }
                   }

                   internal class PersonModel
                   { 
                       public string Age { get; set; }
                   }
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();
      
      result.Should().HaveError("CS8795");

      result.Should().HaveClass("PersonMapper")
         .WhereMethod("MapAge", "int").IsPartialDefinition();

      result.Should().HaveClass("PersonMapper")
         .WhereMethod("MapAge", "string").IsPartialDefinition();

      result.Print();
   }


   [TestMethod]
   public void EnsureUnmatchedPropertiesAreForces()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(Person), typeof(PersonModel), ForceMappings = true)]
                   internal partial class PersonMapper { }
                    
                   internal class Person 
                   { 
                       public int Age { get; set; }
                   }

                   internal class PersonModel
                   { 
                       public string Name { get; set; }
                   }
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();
      
      result.Should().HaveError("CS8795");

      result.Should().HaveClass("PersonMapper")
         .WhereMethod("MapName", "Person value").IsPartialDefinition();

      result.Should().HaveClass("PersonMapper")
         .WhereMethod("MapAge", "PersonModel value").IsPartialDefinition();


      result.Print();
   }

   #endregion
}