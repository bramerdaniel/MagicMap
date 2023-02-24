// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperGenerationTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.SourceGeneratorTests;

using MagicMap.UnitTests.Setups;

[TestClass]
public class TypeMapperGenerationTests
{
   [TestMethod]
   public void EnsureTypeMapperIsGeneratedCorrectly()
   {
      var code = @"namespace RootNameSpace
                   {   
                      internal class A { }
                      internal class B { }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class MapIt { }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("RootNameSpace.MapIt")
         .WithMethod("Map", "A", "B")
         .WithInternalModifier();

      result.Print();
   }
}