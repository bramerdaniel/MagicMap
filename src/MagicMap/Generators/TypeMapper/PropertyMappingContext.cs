// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyMappingContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators.TypeMapper;

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;

internal class PropertyMappingContext
{
   public INamedTypeSymbol SourceType { get; }

   public INamedTypeSymbol TargetType { get; }

   public IDictionary<string, MappingDescription> PropertyMappings { get; }

   public List<Func<string>> MemberDeclarations { get; }

   public PropertyMappingContext(ITypeMapperContext context, INamedTypeSymbol sourceType, INamedTypeSymbol targetType,
      IDictionary<string, MappingDescription> propertyMappings)
   {
      SourceType = sourceType;
      TargetType = targetType;
      PropertyMappings = propertyMappings;
      MemberDeclarations = new List<Func<string>>();

      TargetProperties = TargetType.GetMembers()
         .OfType<IPropertySymbol>()
         .Where(x => !IsIgnored(x.Name))
         .ToDictionary(p => p.Name, StringComparer.InvariantCultureIgnoreCase);

      SourceProperties = SourceType.GetMembers()
         .OfType<IPropertySymbol>()
         .ToDictionary(p => p.Name, StringComparer.InvariantCultureIgnoreCase);

      CustomMappings = context.MapperType.GetMethodsWithAttribute(context.PropertyMapperAttribute).ToArray();
   }

   private bool IsIgnored(string targetName)
   {
      if (PropertyMappings.TryGetValue(targetName, out var mappingDescription))
         return mappingDescription.Ignored;
      return false;
   }

   public (IMethodSymbol method, AttributeData attributeData)[] CustomMappings { get; }

   public IDictionary<string, IPropertySymbol> SourceProperties { get; }

   public IPropertySymbol FindSourceProperty(string name)
   {
      return SourceProperties.TryGetValue(name, out var sourceProperty) ? sourceProperty : null;
   }

   public IDictionary<string, IPropertySymbol> TargetProperties { get; }

   public void AddMemberDeclaration(Func<string> memberDeclaration)
   {
      MemberDeclarations.Add(memberDeclaration);
   }

   private bool TryCreateInvocation(IMethodSymbol method, IPropertySymbol targetProperty, string sourcePropertyName, out string invocation)
   {
      var parameterCount = method.Parameters.Length;
      var sourceProperty = FindSourceProperty(sourcePropertyName ?? targetProperty.Name);

      if (method.ReturnsVoid && parameterCount == 2)
      {
         if (method.IsCallableWith(SourceType, TargetType))
         {
            invocation = $"{method.Name}(source, target);";
            return true;
         }

         if (sourceProperty != null && method.IsCallableWith(sourceProperty.Type, targetProperty.ContainingType))
         {
            invocation = $"{method.Name}(source.{sourceProperty.Name}, target);";
            return true;
         }

         invocation = null;
         return false;
      }

      if (method.ReturnType.Equals(targetProperty.Type) && parameterCount == 1)
      {
         var parameter = method.Parameters[0].Type;
         if (parameter.Equals(SourceType))
         {
            invocation = $"target.{targetProperty.Name} = {method.Name}(source);";
            return true;
         }

         if (sourceProperty != null && parameter.Equals(sourceProperty.Type))
         {
            invocation = $"target.{targetProperty.Name} = {method.Name}(source.{sourceProperty.Name});";
            return true;
         }
      }

      invocation = null;
      return false;
   }



   public bool TryCreateCustomMappings(IPropertySymbol targetProperty, out string mappingCode)
   {
      foreach (var (method, attributeData) in CustomMappings)
      {
         if (attributeData.ConstructorArguments.Length >= 2)
         {
            var targetType = attributeData.ConstructorArguments[0].Value as ITypeSymbol;
            var targetPropertyName = attributeData.ConstructorArguments[1].Value as string;
            var sourcePropertyName = attributeData.ConstructorArguments.Length > 2 ? attributeData.ConstructorArguments[2].Value as string : targetPropertyName;

            if (string.Equals(targetPropertyName, targetProperty.Name) && targetProperty.ContainingType.Equals(targetType, SymbolEqualityComparer.Default))
            {
               if (TryCreateInvocation(method, targetProperty, sourcePropertyName, out mappingCode))
               {
                  return true;
               }
            }
         }
      }

      mappingCode = null;
      return false;
   }
}