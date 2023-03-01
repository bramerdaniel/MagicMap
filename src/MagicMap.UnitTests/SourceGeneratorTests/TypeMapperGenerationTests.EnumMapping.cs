// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bam.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MagicMap.UnitTests.Setups;

namespace MagicMap.UnitTests.SourceGeneratorTests;

public partial class TypeMapperGenerationTests
{
   [TestMethod]
   public void EnsureEnumValuesAreMappedCorrectly()
   {
      var code = @"namespace NS
                   {   
                      internal class A
                      {
                         public int Raw { get; set; } 
                         public EnumValues Value { get; set; } 
                      }

                      internal class B
                      {
                         public int Raw { get; set; } 
                         public EnumTypes Value { get; set; }
                      }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class Mup 
                      {
                      }

                      public enum EnumValues
                      {
                         First,
                         Second,
                         Third
                      }

                      public enum EnumTypes
                      {
                         First,
                         Second,
                         Third
                      }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("NS.Mup")
         .WhereMethod("ConvertEnum")
         .IsPrivate();

      result.Print();
      Assert.Fail("No real expectation");
   }
}