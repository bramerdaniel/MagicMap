// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttributeDataExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
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

      private static bool TryGetNamedArgument(this AttributeData attribute, string argumentName, out TypedConstant typedConstant)
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

      internal static TypedConstant GetTargetType(this AttributeData attribute)
      {
         if (TryGetConstructorArgument(attribute, TypedConstantKind.Type, out var targetType))
            return targetType;

         if (TryGetNamedArgument(attribute, "TargetType", out targetType) && targetType.Kind == TypedConstantKind.Type)
            return targetType;
         return default;
      }

      internal static TypedConstant GetTargetMode(this AttributeData attribute)
      {
         if (TryGetNamedArgument(attribute, "TargetMode", out var targetType) && targetType.Kind == TypedConstantKind.Enum)
            return targetType;
         return default;
      }

      internal static string GetSetupEntryNameSpace(this AttributeData attribute, string defaultValue)
      {
         if (TryGetNamedArgument(attribute, "EntryNamespace", out var targetType) && targetType.Kind == TypedConstantKind.Primitive)
            return targetType.Value?.ToString();
         return defaultValue;
      }

      internal static string GetSetupEntryClassName(this AttributeData attribute, string defaultValue)
      {
         if (TryGetConstructorArgument(attribute, TypedConstantKind.Primitive, out var targetType))
            return targetType.Value?.ToString();

         if (TryGetNamedArgument(attribute, "EntryClassName", out targetType) && targetType.Kind == TypedConstantKind.Primitive)
            return targetType.Value?.ToString();

         return defaultValue;
      }

      internal static string GetSetupMethod(this AttributeData attribute)
      {
         if (TryGetNamedArgument(attribute, "SetupMethod", out var targetType) && targetType.Kind == TypedConstantKind.Primitive)
            return targetType.Value?.ToString();
         
         return null;
      }

   }
}