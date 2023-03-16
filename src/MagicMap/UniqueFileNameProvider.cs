// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileNameProvider.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace MagicMap;

internal class UniqueFileNameProvider : IUniqueNameProvider

{
   private readonly HashSet<string> usedFileNames = new();

   public string GetFileNameForClass(string hintClassName)
   {
      var fileName = $"{hintClassName}.generated.cs";
      var counter = 0;

      while (usedFileNames.Contains(fileName))
         fileName = $"{hintClassName}.{++counter}.generated.cs";

      usedFileNames.Add(fileName);
      return fileName;
   }
}