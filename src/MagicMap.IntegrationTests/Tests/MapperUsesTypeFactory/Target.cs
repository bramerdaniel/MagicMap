// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WobblerModel.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace MagicMap.IntegrationTests.Tests.MapperUsesTypeFactory
{
    internal class Target
    {
        #region Constructors and Destructors

        public Target()
            : this(nameof(Target))
        {
        }

        public Target(string creator)
        {
            Creator = creator ?? throw new ArgumentNullException(nameof(creator));
        }

        #endregion

        #region Public Properties

        public string Creator { get; }

        public string Name { get; set; }

        #endregion
    }
}
