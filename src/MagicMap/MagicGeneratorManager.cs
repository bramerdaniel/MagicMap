// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MagicGeneratorManager.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using MagicMap.Generators;
   using MagicMap.Generators.TypeMapper;

   using Microsoft.CodeAnalysis;
   using Microsoft.CodeAnalysis.CSharp;

   internal class MagicGeneratorManager
   {
      #region Constants and Fields

      private readonly UniqueFileNameProvider uniqueNameProvider = new();

      #endregion

      #region Public Properties

      public Compilation Compilation { get; private set; }

      public CSharpParseOptions ParseOptions { get; private set; }

      #endregion

      #region Properties

      internal static string TypeMapperAttributeName => "MagicMap.TypeMapperAttribute";

      #endregion

      #region Public Methods and Operators

      public static MagicGeneratorManager FromCompilation(Compilation compilation, CSharpParseOptions parseOptions)
      {
         return new MagicGeneratorManager { Compilation = compilation, ParseOptions = parseOptions };
      }

      public bool TryFindGenerator(IGeneratorContext generatorContext, out IGenerator generator)
      {
         if (generatorContext is ITypeMapperContext typeMapperContext)
         {
            generator = new TypeMapperGenerator(typeMapperContext, uniqueNameProvider, ParseOptions);
            return true;
         }

         generator = null;
         return false;
      }

      #endregion
   }
}