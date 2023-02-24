// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Setup.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.UnitTests.Setups;

internal static class Setup
{
   #region Public Methods and Operators

   public static SetupClassInfoSetup SetupClassInfo()
   {
      return new SetupClassInfoSetup();
   }

   public static SourceGeneratorTestSetup SourceGeneratorTest()
   {
      return new SourceGeneratorTestSetup();
   }

   #endregion
}