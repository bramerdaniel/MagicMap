// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CtorArgs.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FluentSetups.IntegrationTests.Targets;

public class CtorArgs
{
    #region Constructors and Destructors

    public CtorArgs(string first, int second, bool third)
    {
        First = first;
        Second = second;
        Third = third;
    }

    #endregion

    #region Public Properties

    public string First { get; }

    public int Second { get; }

    public bool Third { get; }

    #endregion
}
