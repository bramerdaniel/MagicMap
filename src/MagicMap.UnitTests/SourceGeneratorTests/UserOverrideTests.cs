// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserOverrideTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MagicMap.UnitTests.Setups;

namespace MagicMap.UnitTests.SourceGeneratorTests;

[TestClass]
public class UserOverrideTests
{
   [TestMethod]
   public void EnsureDefaultPropertyCanBeOverwritten()
   {
      var code = @"namespace NS
                   {   
                      internal class A { }
                      internal class B { }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class AToBMapper 
                      { 
                          public static AToBMapper Default => null;
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("NS.AToBMapper")
         .WhereProperty("Default")
         .HasInitializationExpression("null")
         .IsPublic();

      result.Print();
   }
      
   [TestMethod]
   [Ignore]
   public void EnsureNonStaticDefaultOverrideIsHandledCorrectly()
   {
      var code = @"namespace NS
                   {   
                      internal class A { }
                      internal class B { }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class AToBMapper 
                      { 
                          public AToBMapper Default => null;
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      // TODO: what should be generated here ?

      result.Print();
   }

   [TestMethod]
   public void EnsureMappingFromLeftToRightCanBeCustomized()
   {
      var code = @"namespace NS
                   {   
                      internal class A { }
                      internal class B { }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class AToBMapper 
                      { 
                          public void Map(A a, B b)
                          { 
                             // do nothing 
                          }
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      //result.Print();
      //return;

      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("NS.AToBMapper")
         .WhereMethod("Map", "NS.A a, NS.B b")
         .Contains("// do nothing");

      result.Should()
         .HaveClass("NS.AToBMapper")
         .WhereMethod("Map", "NS.B source, NS.A target")
         .IsPublic();
      
      result.Print();
   }

   [TestMethod]
   public void EnsureMappingFromRightToLeftCanBeCustomized()
   {
      var code = @"namespace NS
                   {   
                      internal class A { }
                      internal class B { }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class AToBMapper 
                      { 
                          public void Map(B b, A a)
                          { 
                             // do nothing 
                          }
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("NS.AToBMapper")
         .WhereMethod("Map", "NS.B b, NS.A a")
         .Contains("// do nothing");

      result.Should()
         .HaveClass("NS.AToBMapper")
         .WhereMethod("Map", "NS.A source, NS.B target")
         .IsPublic();

      result.Print();
   }
   
   [TestMethod]
   [Ignore]
   public void EnsurePrivateMapOverrideIsHandledCorrectly()
   {
      var code = @"namespace NS
                   {   
                      internal class A { }
                      internal class B { }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class AToBMapper 
                      { 
                          private void Map(A a, B b)
                          { 
                             // do nothing 
                          }
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();
      
      // TODO: what should be generated here ?
      result.Should().NotHaveErrors();
      result.Print();
   }

   [TestMethod]
   public void EnsurePropertyMappingCanBeUserDefinedWithAttribute()
   {
      var code = @"namespace Root
                   {   
                      using MagicMap;

                      internal class Animal
                      {
                          public int AgeInMonths{ get; set; }
                      }

                      internal class Person
                      {
                          public int AgeInYears { get; set; }
                      }
                                        
                      [TypeMapper(typeof(Person), typeof(Animal))]
                      partial class ObjectMapper
                      {
                          [PropertyMapping(nameof(Person.AgeInYears), nameof(Animal.AgeInMonths))]
                          private void MapWithCustomName(Person person, Animal animal)
                          {
                              animal.AgeInMonths = person.AgeInYears * 12;
                          }

                          [PropertyMapping(nameof(Animal.AgeInMonths), nameof(Person.AgeInYears))]
                          private void AnyOtherName(Animal animal, Person person)
                          {
                              person.AgeInYears = animal.AgeInMonths / 12;
                          }
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();
      
      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Person source, Root.Animal target")
         .Contains("MapWithCustomName(source, target);");
      
      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Animal source, Root.Person target")
         .Contains("AnyOtherName(source, target);");
      
      result.Print();
   }

   [TestMethod]
   public void EnsurePropertyMappingCanBeCustomizedWithSourceValueOnly()
   {
      var code = @"namespace Root
                   {   
                      using MagicMap;

                      internal class Animal
                      {
                          public int AgeInMonths{ get; set; }
                      }

                      internal class Person
                      {
                          public int AgeInYears { get; set; }
                      }
                                        
                      [TypeMapper(typeof(Person), typeof(Animal))]
                      partial class ObjectMapper
                      {
                          [PropertyMapping(nameof(Person.AgeInYears), nameof(Animal.AgeInMonths))]
                          private void AgeFromYears(int years, Animal animal)
                          {
                              animal.AgeInMonths = years * 12;
                          }

                          [PropertyMapping(nameof(Animal.AgeInMonths), nameof(Person.AgeInYears))]
                          private void AgeFromMonths(int months, Person person)
                          {
                              person.AgeInYears = months / 12;
                          }
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Person source, Root.Animal target")
         .Contains("AgeFromYears(source.AgeInYears, target);");

      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Animal source, Root.Person target")
         .Contains("AgeFromMonths(source.AgeInMonths, target);");

      result.Print();
   }

   [TestMethod]
   public void EnsurePropertyMappingCanBeUserDefinedWithAttributeUsingOnlyPropertyValues()
   {
      var code = @"namespace Root
                   {   
                      using MagicMap;

                      internal class Animal
                      {
                          public int AgeInMonths{ get; set; }
                      }

                      internal class Person
                      {
                          public int AgeInYears { get; set; }
                      }
                                        
                      [TypeMapper(typeof(Person), typeof(Animal))]
                      partial class ObjectMapper
                      {
                          [PropertyMapping(nameof(Person.AgeInYears), nameof(Animal.AgeInMonths))]
                          private int YearsToMonths(int years)
                          {
                              return years * 12;
                          }

                          [PropertyMapping(nameof(Animal.AgeInMonths), nameof(Person.AgeInYears))]
                          private int MonthsToYears(int months)
                          {
                              return months / 12;
                          }
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Person source, Root.Animal target")
         .Contains("target.AgeInMonths = YearsToMonths(source.AgeInYears);");

      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Animal source, Root.Person target")
         .Contains("target.AgeInYears = MonthsToYears(source.AgeInMonths);");

      result.Print();
   }

   [TestMethod]
   public void EnsurePropertyMappingCanBeUserDefinedWithAttributeUsingOnlyPropertyValues2()
   {
      var code = @"namespace Root
                   {   
                      using MagicMap;

                      internal class Animal
                      {
                          public int AgeInMonths{ get; set; }
                      }

                      internal class Person
                      {
                          public int AgeInYears { get; set; }
                      }
                                        
                      [TypeMapper(typeof(Person), typeof(Animal))]
                      partial class ObjectMapper
                      {
                          [PropertyMapping(nameof(Person.AgeInYears), nameof(Animal.AgeInMonths))]
                          private int YearsToMonths(Person person)
                          {
                              return person.AgeInYears * 12;
                          }

                          [PropertyMapping(nameof(Animal.AgeInMonths), nameof(Person.AgeInYears))]
                          private int MonthsToYears(Animal animal)
                          {
                              return animal.AgeInMonths / 12;
                          }
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Person source, Root.Animal target")
         .Contains("target.AgeInMonths = YearsToMonths(source);");

      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Animal source, Root.Person target")
         .Contains("target.AgeInYears = MonthsToYears(source);");

      result.Print();
   }

   [TestMethod]
   public void EnsureCustomMappingOfSamePropertiesIsPossible()
   {
      var code = @"namespace Root
                   {   
                      using MagicMap;

                      internal class Animal
                      {
                          public int Age{ get; set; }
                      }

                      internal class Person
                      {
                          public int Age { get; set; }
                      }
                                        
                      [TypeMapper(typeof(Person), typeof(Animal))]
                      partial class ObjectMapper
                      {
                          [PropertyMapping(nameof(Person.Age), nameof(Animal.Age))]
                          private int PersonToAnimalAge(int personAge) => personAge * 2;

                          [PropertyMapping(nameof(Animal.Age), nameof(Person.Age))]
                          private int AnimalToPersonAge(int animalAge) =>  animalAge / 2;
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Person source, Root.Animal target")
         .Contains("target.Age = PersonToAnimalAge(source.Age);");

      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Animal source, Root.Person target")
         .Contains("target.Age = AnimalToPersonAge(source.Age);");

      result.Print();
   }

   [TestMethod]
   public void EnsureCustomMappingOfTwoPropertiesIsPossible()
   {
      var code = @"namespace Root
                   {   
                      using MagicMap;

                      internal class Animal
                      {
                          public string Name{ get; set; }
                          public int Age{ get; set; }
                      }

                      internal class Person
                      {
                          public string Name{ get; set; }
                          public int Age { get; set; }
                      }
                                        
                      [TypeMapper(typeof(Person), typeof(Animal))]
                      partial class ObjectMapper
                      {
                          [PropertyMapping(nameof(Person.Age), nameof(Animal.Age))]
                          private int PersonToAnimalAge(Person person) => person.Age * 2;

                          [PropertyMapping(nameof(Animal.Age), nameof(Person.Age))]
                          private int AnimalToPersonAge(Animal animal) => animal.Age / 2;

                          [PropertyMapping(nameof(Person.Name), nameof(Animal.Name))]
                          private string ConvertName(Person person) => person.Name;

                          [PropertyMapping(nameof(Animal.Name), nameof(Person.Name))]
                          private string ConvertName(Animal animal) => animal.Name;
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Person source, Root.Animal target")
         .Contains("target.Age = PersonToAnimalAge(source);")
         .Contains("target.Name = ConvertName(source);");

      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Animal source, Root.Person target")
         .Contains("target.Age = AnimalToPersonAge(source);")
         .Contains("target.Name = ConvertName(source);");

      result.Print();
   }
}