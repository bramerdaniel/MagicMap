// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuilderExtensions.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Utils;

using System.Collections.Generic;
using System.Linq;

using MagicMap.Generators;
using MagicMap.Generators.TypeMapper;

using Microsoft.CodeAnalysis;

internal static class BuilderExtensions
{
   #region Public Methods and Operators


   public static IClassBuilder AddTypeMapperBuilder(this IClassBuilder owner,string className, INamedTypeSymbol sourceType, INamedTypeSymbol targetType)
   {
      var classBuilder = owner.AddNestedClass(className);
      var mapperContext = new TypeMapperContext(className)
      {
         SourceType = sourceType,
         TargetType = targetType,
         MapperExtensionsType = null,
         MappingSpecifications = new Dictionary<string, MappingDescription>()
      };

      var generationLogic = new TypeMapperGenerationLogic(mapperContext);
      generationLogic.Generate(classBuilder);
      return classBuilder;
   }

   public static MethodBuilder<IClassBuilder> AddMethod(this IClassBuilder owner, string name, params (INamedTypeSymbol Type, string Name)[] parameters)
   {
      var methodBuilder = owner.AddMethod()
         .WithCondition(_ => !owner.ContainsMethod(name, parameters.Select(x => x.Type).ToArray()))
         .WithName(name);

      foreach (var parameter in parameters)
      {
         methodBuilder.WithParameter(
            () => parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            () => parameter.Name);
      }

      return methodBuilder;
   }


   public static PartialMethodBuilder<IClassBuilder> AddPartialMethod(this IClassBuilder owner, string name, params (INamedTypeSymbol Type, string Name)[] parameters)
   {
      var partialMethod = owner.AddPartialMethod()
         .WithName(name);

      foreach (var parameter in parameters)
      {
         partialMethod.WithParameter(
            () => parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            () => parameter.Name);
      }

      return partialMethod;
   }

   #endregion
}