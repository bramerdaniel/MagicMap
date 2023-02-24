// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using System;

   internal class EmptyGenerator : IMagicGenerator
   {
      #region Constructors and Destructors

      private EmptyGenerator()
      {
      }

      #endregion

      #region IMagicGenerator Members

      public GeneratedSource Generate()
      {
         throw new NotSupportedException();
      }

      #endregion

      #region Public Properties

      public static IMagicGenerator Instance { get; } = new EmptyGenerator();

      #endregion
   }
}