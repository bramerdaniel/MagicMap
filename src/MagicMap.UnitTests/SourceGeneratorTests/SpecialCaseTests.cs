// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpecialCaseTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.SourceGeneratorTests;

using MagicMap.UnitTests.Setups;

using Microsoft.CodeAnalysis.CSharp;

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


   [TestMethod]
   public void NoStructSupportForLanguageLevelSmallerThanNine()
   {
      var code = @"using MagicMap;
                   using System.Collections.Generic;

                   [TypeMapper(typeof(Person), typeof(PersonStruct))]
                   internal partial class PersonMapper { }
                       
                   internal class Person 
                   {
                       public int Age { get; set; }
                   }

                   internal struct PersonStruct
                   {
                       public int Age { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithLanguageLevel(LanguageVersion.CSharp9)
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should().HaveClass("PersonMapper")
         .WithoutMethod("Map", "Person source", "PersonStruct target");

      result.Print();
   }

   [TestMethod]
   public void NoStructSupportForLanguageLevelSmallerThanNineFromRightToLeft()
   {
      var code = @"using MagicMap;
                   using System.Collections.Generic;

                   [TypeMapper(typeof(PersonStruct), typeof(Person))]
                   internal partial class PersonMapper { }
                       
                   internal class Person 
                   {
                       public int Age { get; set; }
                   }

                   internal struct PersonStruct
                   {
                       public int Age { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithLanguageLevel(LanguageVersion.CSharp9)
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should().HaveClass("PersonMapper")
         .WithoutMethod("Map", "Person source", "PersonStruct target");

      result.Print();
   }
}