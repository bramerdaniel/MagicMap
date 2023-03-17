// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartialClassGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;

internal class PartialClassGenerator : ICodeBuilder
{
    #region Constants and Fields

    private List<string> lazyMembers = new();

    private StringBuilder sourceBuilder;

    #endregion

    #region Constructors and Destructors

    public PartialClassGenerator(INamedTypeSymbol userDefinedPart)
    {
        UserDefinedPart = userDefinedPart ?? throw new ArgumentNullException(nameof(userDefinedPart));
        ClassName = userDefinedPart.Name;
        IsStatic = userDefinedPart.IsStatic;
    }

    #endregion

    #region ICodeBuilder Members

    public ICodeBuilder Append(string code)
    {
        if (!string.IsNullOrWhiteSpace(code))
            SourceBuilder.Append(code);

        return this;
    }

    public ICodeBuilder AppendLine(string code)
    {
        if (!string.IsNullOrWhiteSpace(code))
            SourceBuilder.AppendLine(code);

        return this;
    }

    #endregion

    #region Public Properties

    public string ClassName { get; }

    public bool IsStatic { get; set; }

    public string Modifier { get; set; }

    public INamespaceSymbol Namespace => UserDefinedPart.ContainingNamespace;

    public INamedTypeSymbol UserDefinedPart { get; }

    #endregion

    #region Properties

    private StringBuilder SourceBuilder => sourceBuilder ??= InitializeSourceBuilder();

    #endregion

    #region Public Methods and Operators

    public MethodBuilder AddMethod()
    {
        return new MethodBuilder(this);
    }

    public bool ContainsMethod(INamedTypeSymbol returnType, string name, params INamedTypeSymbol[] parameterTypes)
    {
        foreach (var candidate in UserDefinedPart.GetMethods(name))
        {
            if (ParametersMatch(candidate.Parameters, parameterTypes) && ReturnTypeMatches(candidate.ReturnType, returnType))
                return true;
        }

        return false;
    }

    public bool ContainsMethod(string name, params INamedTypeSymbol[] parameterTypes)
    {
        return UserDefinedPart.GetMethods(name)
           .Any(candidate => ParametersMatch(candidate.Parameters, parameterTypes));
    }

    public bool ContainsProperty(string name)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        return UserDefinedPart.GetProperty(p => p.Name == name) != null;
    }

    #endregion

    #region Methods

    internal string GenerateCode()
    {
        GenerateLazyMembers();
        SourceBuilder.AppendLine("}");

        if (!Namespace.IsGlobalNamespace)
            SourceBuilder.AppendLine("}");

        return sourceBuilder.ToString();
    }

    private string ComputeClassName()
    {
        if (UserDefinedPart != null)
            return UserDefinedPart.Name;
        return ClassName;
    }

    private void GenerateLazyMembers()
    {
        foreach (var lazyMember in lazyMembers)
            SourceBuilder.AppendLine(lazyMember);
    }

    private StringBuilder InitializeSourceBuilder()
    {
        var builder = new StringBuilder();
        if (!Namespace.IsGlobalNamespace)
        {
            builder.AppendLine($"namespace {Namespace.ToDisplayString()}");
            builder.AppendLine("{");
        }

        builder.AppendLine("[global::System.Runtime.CompilerServices.CompilerGenerated]");
        if (Modifier != null)
            builder.Append($"{Modifier} ");

        if (IsStatic)
            builder.Append("static ");

        builder.AppendLine($"partial class {ComputeClassName()}");
        builder.AppendLine("{");
        return builder;
    }

    private bool ParametersMatch(ImmutableArray<IParameterSymbol> definedParameter, INamedTypeSymbol[] parameterTypes)
    {
        if (definedParameter.Length != parameterTypes.Length)
            return false;

        for (var i = 0; i < definedParameter.Length; i++)
        {
            if (!definedParameter[i].Type.Equals(parameterTypes[i], SymbolEqualityComparer.Default))
                return false;
        }

        return true;
    }

    private bool ReturnTypeMatches(ITypeSymbol actualReturnType, INamedTypeSymbol expectedReturnType)
    {
        // TODO handle void
        if (actualReturnType.Equals(expectedReturnType, SymbolEqualityComparer.Default))
            return true;
        return false;
    }

    #endregion
}