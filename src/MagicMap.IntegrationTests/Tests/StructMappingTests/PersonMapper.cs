// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonMapper.cs" company="KUKA Deutschland GmbH">
//   Copyright (c) KUKA Deutschland GmbH 2006 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.IntegrationTests.Tests.StructMappingTests
{
    [TypeMapper(typeof(PersonClass), typeof(PersonStruct))]
    public partial class PersonMapper
    {
        /// <summary>Maps all properties of the <see cref = "source"/> to the properties of the <see cref = "target"/></summary>
        public void Map(global::MagicMap.IntegrationTests.Tests.StructMappingTests.PersonClass source, ref global::MagicMap.IntegrationTests.Tests.StructMappingTests.PersonStruct target)
        {
            target = target with
            {
                Age = source.Age
            };
            target = target with
            {
                Name = source.Name
            };
            MapOverride(source, target);
        }

        public global::MagicMap.IntegrationTests.Tests.StructMappingTests.PersonStruct MapFrom(global::MagicMap.IntegrationTests.Tests.StructMappingTests.PersonClass source)
        {
            var target = Default is MagicMap.ITypeFactory<global::MagicMap.IntegrationTests.Tests.StructMappingTests.PersonStruct, global::MagicMap.IntegrationTests.Tests.StructMappingTests.PersonClass> factory ? factory.Create(source) : new global::MagicMap.IntegrationTests.Tests.StructMappingTests.PersonStruct();
            Default.Map(source, ref target);
            return target;
        }
    }
}
