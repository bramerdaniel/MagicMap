// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SemanticModelExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap;

using System.Threading;

using Microsoft.CodeAnalysis;

public static class SemanticModelExtensions
{

   
   public static INamedTypeSymbol GetNamedType(this GeneratorSyntaxContext context, CancellationToken cancellationToken)
   {
      return context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) as INamedTypeSymbol;
   }
}