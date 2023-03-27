﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFactoryInterfaceGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators;

using System;

using Microsoft.CodeAnalysis;

internal class TypeFactoryInterfaceGenerator
{
   #region Constructors and Destructors

   private TypeFactoryInterfaceGenerator(INamedTypeSymbol interfaceType)
   {
      InterfaceType = interfaceType;
   }

   #endregion

   #region Public Properties

   public static string Code { get; } = @"//------------------------------------------------
// <auto-generated>
//     Generated by the MagicMap source generator
// </auto-generated>
//------------------------------------------------
namespace MagicMap 
{
   [global::System.Runtime.CompilerServices.CompilerGenerated]
   internal interface ITypeFactory<TTarget, TSource>
   {
      /// <summary>Created an instance of the type TTarget, that is used for a mapping.</summary>
      TTarget Create(TSource source);
   }
}
";

   public INamedTypeSymbol InterfaceType { get; }

   #endregion

   #region Properties

   internal static string TypeMapperAttributeName => "MagicMap.ITypeFactory`2";

   protected Compilation Compilation { get; private set; }

   #endregion

   #region Methods

   internal static TypeFactoryInterfaceGenerator FromCompilation(Compilation compilation)
   {
      var attributeType = compilation.GetTypeByMetadataName(TypeMapperAttributeName);
      if (attributeType == null)
         throw new InvalidOperationException($"The source generator should have generated the type {TypeMapperAttributeName} before");

      return new TypeFactoryInterfaceGenerator(attributeType) { Compilation = compilation };
   }

   #endregion
}