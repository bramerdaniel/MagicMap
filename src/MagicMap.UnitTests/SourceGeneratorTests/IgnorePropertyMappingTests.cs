// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IgnorePropertyMappingTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.SourceGeneratorTests;

using MagicMap.UnitTests.Setups;

[TestClass]
public class IgnorePropertyMappingTests
{
   [TestMethod]
   public void EnsurePropertiesWithSameNameCanBeIgnored()
   {
      var code = @"namespace Root
                   {   
                      using MagicMap;

                      internal class Animal
                      {
                          public string Name { get; set; }
                          public int Age { get; set; }
                      }

                      internal class Person
                      {
                          public string Name { get; set; }
                          public int Age { get; set; }
                      }
                                        
                      [TypeMapper(typeof(Person), typeof(Animal))]
                      [PropertyMapping(nameof(Person.Age), nameof(Animal.Age), Ignore = true)]
                      partial class ObjectMapper { }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Person source, Root.Animal target")
         .Contains("target.Name = source.Name");

      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Person source, Root.Animal target")
         .NotContains("target.Age = ");

      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Animal source, Root.Person target")
         .NotContains("target.Age = ");

      result.Print();
   }

   [TestMethod]
   public void EnsurePropertiesWithDifferentNameCanBeIgnored()
   {
      var code = @"namespace Root
                   {   
                      using MagicMap;

                      internal class Animal
                      {
                          public string Name { get; set; }
                          public int AnimalAge { get; set; }
                      }

                      internal class Person
                      {
                          public string Name { get; set; }
                          public int PersonAge { get; set; }
                      }
                                        
                      [TypeMapper(typeof(Person), typeof(Animal))]
                      [PropertyMapping(nameof(Person.PersonAge), nameof(Animal.AnimalAge), Ignore = true)]
                      partial class ObjectMapper { }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Person source, Root.Animal target")
         .Contains("target.Name = source.Name");

      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Person source, Root.Animal target")
         .NotContains("target.AnimalAge = ");

      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Animal source, Root.Person target")
         .NotContains("target.PersonAge = ");

      result.Print();
   }

}