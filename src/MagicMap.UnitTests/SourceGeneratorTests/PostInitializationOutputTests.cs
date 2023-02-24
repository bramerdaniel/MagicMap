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

      result.Print();
   }

   [TestMethod]
    public void EnsureTypeMapperAttributeIsGenerated()
    {
        var code = @"namespace RonnyTheRobber
                   {   
                      using FluentSetups;

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

        result.Print();
    }
}