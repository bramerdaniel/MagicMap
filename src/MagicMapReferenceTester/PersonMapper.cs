// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonToPersonMapper.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMapReferenceTester;

using MagicMap;

[TypeMapper(typeof(Person), typeof(Person))]
internal partial class PersonMapper
{
}