// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratorSyntaxContextExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Extensions;

using System.Threading;

using Microsoft.CodeAnalysis;

internal static class GeneratorSyntaxContextExtensions
{
    #region Public Methods and Operators

    internal static INamedTypeSymbol GetNamedType(this GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        return context.SemanticModel.GetDeclaredSymbol(context.Node, cancellationToken) as INamedTypeSymbol;
    }

    #endregion
}