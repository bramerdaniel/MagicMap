// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecursiveMapperUsageTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MagicMap.UnitTests.Setups;

namespace MagicMap.UnitTests.SourceGeneratorTests;

[TestClass]
public class RecursiveMapperUsageTests
{
   [TestMethod]
   public void EnsurePropertiesThatMatchMapperTypeAreHandledCorrectly()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(Person), typeof(PersonModel))]
                   internal partial class PersonMapper { }
                       
                   internal class Person 
                   {
                       public string Name { get; set; }
                       public Person Father { get; set; }
                   }

                   internal class PersonModel 
                   {
                       public string Name { get; set; }
                       public PersonModel Father { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should().HaveClass("PersonMapper")
         .WhereMethod("Map", "Person source")
         .Contains("target.Father = source.Father.ToPersonModel();");

      result.Print();
   }
}