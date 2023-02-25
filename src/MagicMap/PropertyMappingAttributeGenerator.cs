﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyMappingAttributeText.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

/// <summary>Attribute that identifies property with different names on the left and right object</summary>
internal class PropertyMappingAttributeGenerator
{
   private readonly INamedTypeSymbol attributeSymbol;

   #region Public Properties

   public static string Code { get; } = @"//------------------------------------------------
// <auto-generated>
//     Generated by the MagicMap source generator
// </auto-generated>
//------------------------------------------------
namespace MagicMap 
{
   [global::System.Runtime.CompilerServices.CompilerGenerated]
   [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
   /// <summary>Attribute that can be used to map a specific property from the left to the right object by their names</summary>
   internal sealed class PropertyMappingAttribute : global::System.Attribute
   {
      public PropertyMappingAttribute(string leftName, string rightName)
      {
         LeftName = leftName ?? throw new global::System.ArgumentNullException(nameof(leftName));
         RightName = rightName ?? throw new global::System.ArgumentNullException(nameof(rightName));
      }

      /// <summary>Gets the name of the left property.</summary>
      public string LeftName { get; }

      /// <summary>Gets the name of the right property.</summary>
      public string RightName { get; }
   }
}
";

   #endregion

   internal static string PropertyMappingAttributeName => "MagicMap.PropertyMappingAttribute";

   internal static PropertyMappingAttributeGenerator FromCompilation(Compilation compilation)
   {
      var attributeType = compilation.GetTypeByMetadataName(PropertyMappingAttributeName);
      if (attributeType == null)
         throw new InvalidOperationException($"The source generator should have generated the type {PropertyMappingAttributeName} before");
      
      return new PropertyMappingAttributeGenerator(attributeType);
   }

   private PropertyMappingAttributeGenerator(INamedTypeSymbol attributeSymbol)
   {
      this.attributeSymbol = attributeSymbol ?? throw new ArgumentNullException(nameof(attributeSymbol));
   }

   public IEnumerable<(string leftName, string rightName)> ComputePropertyMappings(INamedTypeSymbol classSymbol)
   {
      foreach (var propertyMapping in classSymbol.GetAttributes().Where(x => attributeSymbol.Equals(x.AttributeClass, SymbolEqualityComparer.Default)))
      {
         if (propertyMapping.ConstructorArguments.Length == 2)
         {
            var leftName = propertyMapping.ConstructorArguments[0].Value as string;
            var rightName = propertyMapping.ConstructorArguments[1].Value as string;
            yield return (leftName, rightName);
         }


      }
   }
}