// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

using Microsoft.CodeAnalysis;

class TypeMapperGenerator : IMagicGenerator
{
   public GeneratedSource Generate()
   {
      var generatedSource = new GeneratedSource();
      generatedSource.AddDiagnostic(Diagnostic.Create(MagicMapDiagnostics.NotSupported, null));
      return generatedSource;
   }
}