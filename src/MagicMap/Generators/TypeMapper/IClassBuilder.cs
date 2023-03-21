// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClassBuilder.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators.TypeMapper;

using MagicMap.Utils;
using Microsoft.CodeAnalysis;

internal interface IClassBuilder
{
   #region Public Methods and Operators

   PartialMethodBuilder<IClassBuilder> AddPartialMethod();

   MethodBuilder<IClassBuilder> AddMethod();

   IClassBuilder AddNestedClass(string name);

   bool ContainsMethod(string name, params INamedTypeSymbol[] parameterTypes);

   bool ContainsProperty(string name);

   IClassBuilder AppendLine(string text);

   IClassBuilder Append(string text);

   public INamedTypeSymbol UserDefinedPart { get; }

   /// <summary>Gets the simple name of the class.</summary>
   string ClassName { get; }

   #endregion

   string GenerateCode();
}