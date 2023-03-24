// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedGeneratorTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.SourceGeneratorTests;

using MagicMap.UnitTests.Setups;

[TestClass]
public class NestedGeneratorTests
{
   [TestMethod]
   public void EnsureCustomMappingIsPossibleWithoutSourceProperty()
   {
      var code = @"namespace Root
                   {   
                      using MagicMap;

                      internal class Address
                      {
                          public string Street { get; set; }
                      }

                      internal class AddressModel
                      {
                          public string Street { get; set; }
                      }

                      internal class Person
                      {
                          public Address Address { get; set; }
                      }

                      internal class PersonModel
                      {
                          public AddressModel Address { get; set; }
                      }
                                        
                      [TypeMapper(typeof(Person), typeof(PersonModel))]
                      partial class ObjectMapper
                      {
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.Person source, Root.PersonModel target")
         .Contains("target.Address = AddressMapper.Default.MapFrom(source.Address);");

      result.Should()
         .HaveClass("Root.ObjectMapper")
         .WhereMethod("Map", "Root.PersonModel source, Root.Person target")
         .Contains("target.Address = AddressMapper.Default.MapFrom(source.Address);");

      result.Print();
   }

}