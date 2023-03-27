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

      result.Should().NotHaveErrors();

      result.Should()
         .HaveClass("NS.Mup")
         .WhereMethod("ConvertEnum", "NS.EnumValues value")
         .Contains("switch (value)")
         .IsPrivate();

      result.Should()
         .HaveClass("NS.Mup")
         .WhereMethod("ConvertEnum", "NS.EnumTypes value")
         .Contains("switch (value)")
         .IsPrivate();

      result.Print();
   }

   [TestMethod]
   public void EnsureEnumMappingCanBeCustomized()
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
                         EnumTypes ConvertEnum(EnumValues value) => throw new global::System.NotSupportedException();
                         EnumValues ConvertEnum(EnumTypes value) => throw new global::System.NotSupportedException();
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
         .WhereMethod("ConvertEnum", "NS.EnumValues value")
         .Contains("throw new global::System.NotSupportedException()")
         .IsPrivate();

      result.Should().HaveClass("NS.Mup")
         .WhereMethod("ConvertEnum", "NS.EnumTypes value")
         .Contains("throw new global::System.NotSupportedException()")
         .IsPrivate();

      result.Print();
   }

   [TestMethod]
   [Ignore]
   public void EnsureTwoPropertiesWithSameEnumTypeWorkCorrectly()
   {
      var code = @"namespace NS
                   {   
                      internal class A
                      {
                         public EnumValues First { get; set; } 
                         public EnumValues Second { get; set; } 
                      }

                      internal class B
                      {
                         public EnumTypes First { get; set; }
                         public EnumTypes Second { get; set; }
                      }
                      
                      [MagicMap.TypeMapperAttribute(typeof(A), typeof(B))]
                      internal partial class Mup {  }

                      public enum EnumValues { First, Second }
                      public enum EnumTypes { First, Second }
                   }";

      var result = Setup.SourceGeneratorTest()
         .WithSource(code)
         .Done();

      result.Should().NotHaveErrors();

      result.Print();
   }

   [TestMethod]
   public void EnsureEnumMappingToStructWorksCorrectly()
   {
      var code = @"namespace NS
                   {   
                      internal class Person
                      {
                         public EnumValues Value { get; set; } 
                      }

                      internal struct PersonStruct
                      {
                         public EnumTypes Value { get; set; }
                      }
                      
                      [MagicMap.TypeMapperAttribute(typeof(Person), typeof(PersonStruct))]
                      internal partial class Mapper { }

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
         .HaveClass("NS.Mapper")
         .WhereMethod("Map", "NS.Person source, NS.PersonStruct target")
         .Contains("target = target with {Value = ConvertEnum(source.Value)}");

      result.Print();
   }
}