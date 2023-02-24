// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticModelExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

using Microsoft.CodeAnalysis;

public static class SemanticModelExtensions
{
   public static INamedTypeSymbol GetNameType(this SemanticModel model, SyntaxNode node)
   {
      return model.GetDeclaredSymbol(node) as INamedTypeSymbol;
   }
}