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
         .WithMethod("Map", "RootNameSpace.A", "RootNameSpace.B")
         .WithInternalModifier();

      result.Print();
   }

   [TestMethod]
   public void EnsureTypeMapperInRootNamespaceIsGeneratedCorrectly()
   {
      var code = @"
                      internal class A { }
                      internal class B { }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class MyMapper { }
                  ";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("MyMapper")
         .WithMethod("Map", "A", "B")
         .WithInternalModifier();

      result.Print();
   }

   [TestMethod]
   public void EnsureTypeMapperForTypesWithOtherNamespacesIsGeneratedCorrectly()
   {
      var code = @"namespace RootNameSpace
                   {                         
                      [MagicMap.TypeMapperAttribute(typeof(First.A), typeof(Second.B))]
                      internal partial class MapIt { }
                   }
                   
                   namespace First
                   {                         
                      internal class A { }
                      internal class B { }
                   }           

                   namespace Second
                   {                         
                      internal class A { }
                      internal class B { }
                   }
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("RootNameSpace.MapIt")
         .WithMethod("Map", "First.A", "Second.B")
         .WithInternalModifier();

      result.Print();
   }
}