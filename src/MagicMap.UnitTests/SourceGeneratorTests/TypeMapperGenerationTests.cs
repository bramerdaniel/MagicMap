// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperGenerationTests.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.SourceGeneratorTests;

using MagicMap.UnitTests.Setups;

[TestClass]
public partial class TypeMapperGenerationTests
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
      result.Should()
         .HaveClass("RootNameSpace.MapIt")
         .WithMethod("Map", "RootNameSpace.B", "RootNameSpace.A")
         .WithInternalModifier();

      result.Print();
   }

   [TestMethod]
   public void EnsureOnlyOneMethodWithSameMappingTypes()
   {
      var code = @"namespace SameTypes
                   {   
                      internal class A { }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(A))]
                      internal partial class SameTypeMapper { }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("SameTypes.SameTypeMapper")
         .WithMethod("Map", "SameTypes.A", "SameTypes.A")
         .WithInternalModifier();

      result.Print();
   }


   [TestMethod]
   public void EnsureMatchingPropertiesWithDifferentTypesCreateMapperPartials()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(Person), typeof(Material))]
                   internal partial class TestMapper { }
                       
                   internal class Person 
                   {
                       public int Age { get; set; }
                   }

                   internal class Material 
                   {
                       public double Age { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should().HaveClass("TestMapper")
         .WhereMethod("MapAge", "Person target, double value")
         .IsPartialDefinition();

      result.Should().HaveClass("TestMapper")
         .WhereMethod("MapAge", "Material target, int value")
         .IsPartialDefinition();

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

   [TestMethod]
   public void EnsureMappingCodeIsGeneratedCorrectly()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(A), typeof(B))]
                   internal partial class TestMapper { }
                       
                   internal class A 
                   {
                       public string Name { get; set; }
                   }

                   internal class B 
                   {
                       public string Name { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("TestMapper")
         .WithMethod("Map", "A", "B")
         .WhereMethod("Map", "A source, B target")
         .Contains("target.Name = source.Name");

      result.Print();
   }

   [TestMethod]
   public void EnsureNotAccessiblePropertiesIsIgnored()
   {
      var code = @"
                   [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                   internal partial class TestMapper { }
                       
                   internal class A 
                   {
                       string Name { get; set; }
                       int Age { get; set; }
                   }

                   internal class B 
                   {
                       string Name { get; set; }
                       private int Age { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("TestMapper")
         .WhereMethod("Map", "A")
         .NotContains("B.Name = A.Name")
         .NotContains("B.Age = A.Age");

      result.Print();
   }

   [TestMethod]
   public void EnsureReadonlyPropertiesAreIgnored()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(A), typeof(B))]
                   internal partial class TestMapper { }
                       
                   internal class A 
                   {
                       public string Name { get; set; }
                   }

                   internal class B 
                   {
                       public string Name { get; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should().HaveClass("TestMapper")
         .WhereMethod("Map", "A source, B target")
         .NotContains("target.Name = source.Name");

      result.Should().HaveClass("TestMapper")
         .WhereMethod("Map", "B source, A target")
         .Contains("target.Name = source.Name");

      result.Print();

   }

   [TestMethod]
   public void EnsureMappingCanMeCustomized()
   {
      var code = @"using MagicMap;

                   [TypeMapper(typeof(A), typeof(B))]
                   [PropertyMapping(nameof(A.Name), nameof(B.Value))]
                   internal partial class TestMapper { }
                       
                   internal class A 
                   {
                       public string Name { get; set; }
                   }

                   internal class B 
                   {
                       public string Value { get; set; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();

      result.Should().HaveClass("TestMapper")
         .WhereMethod("Map", "A source, B target")
         .Contains("target.Value = source.Name");

      result.Should().HaveClass("TestMapper")
         .WhereMethod("Map", "B source, A target")
         .Contains("target.Name = source.Value");

      result.Print();
   }

   [TestMethod]
   public void EnsureCustomHandlingOfPropertiesIsPossible()
   {
      var code = @"using MagicMap;
                   using System.Collections.Generic;   

                   [TypeMapper(typeof(A), typeof(B))]
                   internal partial class TestMapper 
                   { 
                       partial void MapOverride(A source, B target)
                          => target.Values.AddRange(source.Values);

                       partial void MapOverride(B source, A target)
                          => target.Values.AddRange(source.Values);
                   }
                       
                   internal class A 
                   {
                       public List<string> Values { get; }
                   }

                   internal class B 
                   {
                       public List<string> Values { get; }
                   }            
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();

      result.Should().HaveClass("TestMapper")
         .WhereMethod("MapOverride", "A source, B target").IsPartialDefinition();

      result.Should().HaveClass("TestMapper")
         .WhereMethod("MapOverride", "B source, A target").IsPartialDefinition();

      result.Print();
   }

   [TestMethod]
   public void EnsureNestedMapperTypesAreIgnored()
   {
      var code = @"namespace First
                   {   
                      internal class A { }
                      internal class B { }

                      internal class Outer
                      {
                         [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                         internal partial class Mapper
                         {

                         }
                      }
                   }
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();
      result.Should().NotHaveClass("First.Mapper");


      result.Print();
   }

   [TestMethod]
   public void EnsureTwoTypeMapperInDifferentNamespacesAreGeneratedCorrectly()
   {
      var code = @"namespace First
                   {   
                      internal class A { }
                      internal class B { }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class Mapper { }
                   }

                   namespace Second
                   {   
                      internal class A { }
                      internal class B { }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class Mapper { }
                   }
";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("First.Mapper")
         .WithMethod("Map", "First.A", "First.B")
         .WithInternalModifier();

      result.Should()
         .HaveClass("Second.Mapper")
         .WithMethod("Map", "Second.A", "Second.B")
         .WithInternalModifier();

      result.Print();
   }

   [TestMethod]
   public void EnsureDefaultPropertyIsCreatedOnTypeMapper()
   {
      var code = @"namespace NS
                   {   
                      internal class A { }
                      internal class B { }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class MapMaster { }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("NS.MapMaster")
         .WhereProperty("Default")
         .HasInitializationExpression("new MapMaster()")
         .IsStatic();

      result.Print();
   }
   
   [TestMethod]
   public void EnsurePartialMemberForTargetCreationIsCreated()
   {
      var code = @"
                      internal class A { }
                      internal class B { private B(bool flag){ } }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class ElMapper { }
                  ";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("ElMapperExtensions")
         .WhereMethod("ToB")
         .Contains("throw new global::System.NotSupportedException(\"The target type B can not be created");

      result.Print();
   }

   [TestMethod]
   public void EnsureTargetGenerationCanBeOverwritten()
   {
      var code = @"
                      internal class A { }
                      internal class B { internal B(bool flag){ } }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class ElMapper { }

                      internal static partial class ElMapperExtensions 
                      { 
                         private static B CreateB() => new B(true);
                      }
                  ";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors().And
         .HaveClass("ElMapperExtensions")
         .WhereMethod("CreateB")
         .Contains("new B(true);")
         .IsPrivate();

      result.Print();
   }

}