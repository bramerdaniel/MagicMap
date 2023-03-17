// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostInitializationOutputTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.SourceGeneratorTests;


using MagicMap.UnitTests.Setups;

[TestClass]
public class PostInitializationOutputTests
{
   [TestMethod]
   public void EnsureTypeMapperAttributeIsAlwaysGenerated()
   {
      var code = @"namespace RonnyTheRobber
                   {   
                      internal class DoesNotMatter
                      {
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("MagicMap.TypeMapperAttribute")
         .WithInternalModifier();

      result.Print(0 , 1);
   }

   [TestMethod]
    public void EnsureTypeMapperAttributeIsGenerated()
    {
        var code = @"namespace RonnyTheRobber
                   {   
                      internal class A {  }
                      internal class B {  }

                      [MagicMap.TypeMapper(typeof(A), typeof(B))]
                      internal partial class Mapper
                      {
                      }
                   }";

        var result = Setup.SourceGeneratorTest()
           .WithSource(code)
           .Done();

        result.Should().NotHaveErrors().And
           .HaveClass("MagicMap.TypeMapperAttribute")
           .WithInternalModifier();
      
        result.Print(0, 1);
   }

    [TestMethod]
    public void EnsureTypeMapperFactoryInterfaceIsGenerated()
    {
       var code = @"namespace RonnyTheRobber
                   {   
                      internal class A {  }
                      internal class B {  }

                      [MagicMap.TypeMapper(typeof(A), typeof(B))]
                      internal partial class Mapper { }
                   }";

       var result = Setup.SourceGeneratorTest()
          .WithSource(code)
          .Done();

       result.Should().NotHaveErrors().And
          .HaveInterface("MagicMap.ITypeFactory`2")
          .WithInternalModifier();

       result.Print(2, 1);
   }

    [TestMethod]
    public void EnsurePropertyMappingAttributeIsGenerated()
    {
       var code = @"namespace RonnyTheRobber
                   {   
                      internal partial class Mapper { }
                   }";

       var result = Setup.SourceGeneratorTest()
          .WithSource(code)
          .Done();

       result.Should().NotHaveErrors().And
          .HaveInterface("MagicMap.PropertyMappingAttribute")
          .WithInternalModifier();

       result.Print(1, 1);
   }
}