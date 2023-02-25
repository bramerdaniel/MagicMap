// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperExtensionsTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.SourceGeneratorTests;

using MagicMap.UnitTests.Setups;

[TestClass]
public class TypeMapperExtensionsTests
{
   [TestMethod]
   public void EnsureExtensionClassIsPublicWhenMapperIsPublic()
   {
      var code = @"[MagicMap.TypeMapper(typeof(Person), typeof(Employee))]
                   public partial class PersonMapper { }
                       
                   public class Person 
                   {
                       internal string Name { get; set; }
                   }

                   public class Employee 
                   {
                       internal string Name { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();

      result.Should().HaveClass("PersonMapperExtensions").WithPublicModifier()
         .WhereMethod("ToEmployee", "Person")
         .IsPublic();

      result.Print();
   }

   [TestMethod]
   public void EnsureExtensionClassIsInternalWhenMapperIsInternal()
   {
      var code = @"[MagicMap.TypeMapper(typeof(Person), typeof(Employee))]
                   internal partial class PersonMapper { }
                       
                   internal class Person 
                   {
                       internal string Name { get; set; }
                   }

                   internal class Employee 
                   {
                       internal string Name { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();

      result.Should().HaveClass("PersonMapperExtensions").WithInternalModifier()
         .WhereMethod("ToEmployee", "Person")
         .IsInternal();

      result.Print();
   }

   [TestMethod]
   public void EnsureExtensionMethodFromLeftToRightIsGeneratedCorrectly()
   {
      var code = @"[MagicMap.TypeMapper(typeof(Person), typeof(Employee))]
                   internal partial class PersonMapper { }
                       
                   internal class Person 
                   {
                       internal string Name { get; set; }
                       internal int Age { get; set; }
                   }

                   internal class Employee 
                   {
                       internal string Name { get; set; }
                       internal int Age { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();

      result.Should().HaveClass("PersonMapperExtensions")
         .WhereMethod("ToEmployee", "Person")
         .IsInternal()
         .Contains("mapper.Map(value, result)");

      result.Print();
   }
   
   [TestMethod]
   public void EnsureExtensionMethodFromRightToLeftIsGeneratedCorrectly()
   {
      var code = @"[MagicMap.TypeMapper(typeof(Person), typeof(Employee))]
                   internal partial class PersonMapper { }
                       
                   internal class Person 
                   {
                       internal string Name { get; set; }
                       internal int Age { get; set; }
                   }

                   internal class Employee 
                   {
                       internal string Name { get; set; }
                       internal int Age { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();

      result.Should().HaveClass("PersonMapperExtensions")
         .WhereMethod("ToPerson", "Employee")
         .IsInternal()
         .Contains("mapper.Map(value, result)");

      result.Print();
   }
}