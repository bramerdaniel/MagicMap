﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactoryInterfaceGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators;

using System;
using System.Collections.Generic;
using System.Linq;

using MagicMap.Generators.TypeMapper;

using Microsoft.CodeAnalysis;

internal class TypeFactoryInterfaceGenerator
{
   private readonly INamedTypeSymbol interfaceSymbol;

   private readonly PropertyMappingAttributeGenerator propertyMappingAttribute;

   private readonly MapperFactoryAttributeGenerator mapperFactoryAttribute;

   #region Public Properties

   public static string Code { get; } = @"//------------------------------------------------
// <auto-generated>
//     Generated by the MagicMap source generator
// </auto-generated>
//------------------------------------------------
namespace MagicMap 
{
   [global::System.Runtime.CompilerServices.CompilerGenerated]
   internal interface ITypeFactory<TTarget, TSource>  where TTarget : class
   {
      /// <summary>Created an instance of the type TTarget, that is used for a mapping.</summary>
      TTarget Create(TSource source);
   }
}
";

   #endregion

   internal static string TypeMapperAttributeName => "MagicMap.ITypeFactory`2";

   internal static TypeFactoryInterfaceGenerator FromCompilation(Compilation compilation)
   {
      var attributeType = compilation.GetTypeByMetadataName(TypeMapperAttributeName);
      if (attributeType == null)
         throw new InvalidOperationException($"The source generator should have generated the type {TypeMapperAttributeName} before");

      return new TypeFactoryInterfaceGenerator(attributeType) { Compilation = compilation };
   }

   protected Compilation Compilation { get; private set; }

   private TypeFactoryInterfaceGenerator(INamedTypeSymbol interfaceType)
   {
      this.interfaceSymbol = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
   }

}