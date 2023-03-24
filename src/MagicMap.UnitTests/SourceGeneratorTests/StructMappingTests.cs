// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructMappingTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.SourceGeneratorTests;

using MagicMap.UnitTests.Setups;

[TestClass]
public class StructMappingTests
{
   #region Public Methods and Operators

   [TestMethod]
   public void EnsureMappingToAStructIsPossible()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(PersonClass), typeof(PersonStruct))]
                   internal partial class PersonMapper { }
                       
                   internal class PersonClass 
                   {
                       public string Name { get; set; }
                   }

                   internal struct PersonStruct 
                   {
                       public string Name { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should().HaveClass("PersonMapper")
         .WithMethod("Map", "PersonClass", "PersonStruct")
         .WithMethod("Map", "PersonStruct", "PersonClass");


      result.Print();
   }

   [TestMethod]
   public void EnsureMappingFromAStructIsPossible()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(PersonStruct), typeof(PersonClass))]
                   internal partial class PersonMapper { }
                       
                   internal class PersonClass 
                   {
                       public string Name { get; set; }
                   }

                   internal struct PersonStruct 
                   {
                       public string Name { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should().HaveClass("PersonMapper")
         .WithMethod("Map", "PersonClass", "PersonStruct")
         .WithMethod("Map", "PersonStruct", "PersonClass");


      result.Print();
   }

   #endregion
}