// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialCaseTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.SourceGeneratorTests;

using MagicMap.UnitTests.Setups;

/// <summary>
/// 
/// </summary>
[TestClass]
public class SpecialCaseTests
{
   [TestMethod]
   public void EnsureListsWithOwnerTypeAreMappedCorrectly()
   {
      var code = @"using MagicMap;
                   using System.Collections.Generic;

                   [TypeMapper(typeof(Person), typeof(PersonModel))]
                   internal partial class PersonMapper { }
                       
                   internal class Person 
                   {
                       public List<Person> Children { get; set; }
                   }

                   internal class PersonModel 
                   {
                       public List<PersonModel> Children { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should().HaveClass("PersonMapper")
         .WhereMethod("Map", "Person source")
         .NotContains("target.Children");

      result.Print();
   }
}