// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomizedAnimalMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace MagicMap.IntegrationTests.Tests.CustomFactoryMethod
{
    [TypeMapper(typeof(SourcePerson), typeof(TargetPerson))]
    internal partial class MauleMapper
    {
        public MauleMapper()
        {
            throw new AssertFailedException("Should never be called with true");
        }

        public MauleMapper(bool flag)
        {
        }
    }

    //internal static partial class MauleMapperExtensions
    //{
    //    private static MauleMapper Mapper => new MauleMapper(true);
    //}
}
