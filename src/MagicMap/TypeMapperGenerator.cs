// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

using System.Text;

using Microsoft.CodeAnalysis;

class TypeMapperGenerator : IMagicGenerator
{
   private readonly ISymbol classSymbol;

   public TypeMapperGenerator(ISymbol classSymbol, AttributeData attribute)
   {
      this.classSymbol = classSymbol;
   }

   public GeneratedSource Generate()
   {
      var builder = new StringBuilder();
      builder.Append("public partial class ");
      builder.Append(classSymbol.Name);
      builder.Append("{ }");

      var generatedSource = new GeneratedSource{ Code = builder.ToString() };
      // generatedSource.AddDiagnostic(Diagnostic.Create(MagicMapDiagnostics.NotSupported, null));
      return generatedSource;
   }
}