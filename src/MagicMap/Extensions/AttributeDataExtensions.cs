// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeDataExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Extensions
{
   using System;
   using System.Linq;
    using Microsoft.CodeAnalysis;

    public static class AttributeDataExtensions
    {
        private static bool TryGetConstructorArgument(this AttributeData attribute, TypedConstantKind type, out TypedConstant targetType)
        {
            if (attribute != null && attribute.ConstructorArguments.Length > 0)
            {
                targetType = attribute.ConstructorArguments.FirstOrDefault(x => x.Kind == type);
                return !targetType.IsNull;
            }

            targetType = default;
            return false;
        }

        public static bool TryGetNamedArgument(this AttributeData attribute, string argumentName, out TypedConstant typedConstant)
        {
            if (attribute != null && attribute.NamedArguments.Length > 0)
            {
                var match = attribute.NamedArguments.FirstOrDefault(x => x.Key == argumentName);
                if (match.Key != null)
                {
                    typedConstant = match.Value;
                    return true;
                }
            }

            typedConstant = default;
            return false;
        }

        public static T GetNamedArgument<T>(this AttributeData attribute, string argumentName, Func<T> defaultValue)
        {
            if (attribute is { NamedArguments.Length: > 0 })
            {
                var matchedArgument = attribute.NamedArguments.FirstOrDefault(x => x.Key == argumentName);
                if (matchedArgument is { Key: { }, Value.Value: T typedValue })
                   return typedValue;
            }

            return defaultValue();
        }

        internal static TypedConstant GetTargetType(this AttributeData attribute)
        {
            if (attribute.TryGetConstructorArgument(TypedConstantKind.Type, out var targetType))
                return targetType;

            if (attribute.TryGetNamedArgument("TargetType", out targetType) && targetType.Kind == TypedConstantKind.Type)
                return targetType;
            return default;
        }

        internal static TypedConstant GetTargetMode(this AttributeData attribute)
        {
            if (attribute.TryGetNamedArgument("TargetMode", out var targetType) && targetType.Kind == TypedConstantKind.Enum)
                return targetType;
            return default;
        }

        internal static string GetSetupEntryNameSpace(this AttributeData attribute, string defaultValue)
        {
            if (attribute.TryGetNamedArgument("EntryNamespace", out var targetType) && targetType.Kind == TypedConstantKind.Primitive)
                return targetType.Value?.ToString();
            return defaultValue;
        }





    }
}