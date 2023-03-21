// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeMapperGenerator.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap.Generators.TypeMapper;

using System;
using System.Collections.Generic;
using System.Text;
using MagicMap.Extensions;
using MagicMap.Generators;
using MagicMap.Utils;
using Microsoft.CodeAnalysis;

/// <summary>Generator for type mapping</summary>
/// <seealso cref="MagicMap.Generators.IGenerator" />
internal class TypeMapperGenerator : IGenerator
{
   #region Constants and Fields

   private readonly ITypeMapperContext context;

   private readonly IUniqueNameProvider uniqueNameProvider;

   #endregion

   #region Constructors and Destructors

   public TypeMapperGenerator(ITypeMapperContext context, IUniqueNameProvider uniqueNameProvider)
   {
      this.context = context ?? throw new ArgumentNullException(nameof(context));
      this.uniqueNameProvider = uniqueNameProvider ?? throw new ArgumentNullException(nameof(uniqueNameProvider));
   }

   #endregion

   #region IGenerator Members

   public IEnumerable<GeneratedSource> Generate()
   {
      var mapperClass = GenerateMapperClass(context);
      yield return new GeneratedSource
      {
         Name = uniqueNameProvider.GetFileNameForClass(context.MapperName),
         Code = mapperClass.GenerateCode(),
         Diagnostics = mapperClass.Diagnostics
      };

      var extensionsClass = GenerateExtensionsClass(context);
      var extensionFileName = uniqueNameProvider.GetFileNameForClass(extensionsClass.ClassName);
      yield return new GeneratedSource
      {
         Name = extensionFileName,
         Code = extensionsClass.GenerateCode(),
         Diagnostics = extensionsClass.Diagnostics
      };
   }

   #endregion

   #region Properties

   private string MapperTypeName => context.MapperName;

   #endregion

   #region Methods

   private static bool IsInternal(INamedTypeSymbol typeSymbol)
   {
      switch (typeSymbol.DeclaredAccessibility)
      {
         case Accessibility.Public:
            return false;
      }

      return true;
   }


   private static string ComputeModifier(ITypeMapperContext context)
   {
      if (context.MapperType == null)
         return "public";

      if (IsInternal(context.MapperType))
         return "internal";
      if (IsInternal(context.TargetType))
         return "internal";
      if (IsInternal(context.SourceType))
         return "internal";

      return "public";
   }

   private PartialClassGenerator GenerateExtensionsClass(ITypeMapperContext mapperContext)
   {
      var mapperExtensions = CreateGenerationContext(mapperContext);
      GenerateMapperProperty(mapperExtensions);
      GenerateExtensionMethod(mapperExtensions, mapperContext.TargetType, mapperContext.SourceType);

      if (!mapperContext.SourceEqualsTargetType)
         GenerateExtensionMethod(mapperExtensions, mapperContext.SourceType, mapperContext.TargetType);

      return mapperExtensions;
   }

   private PartialClassGenerator CreateGenerationContext(ITypeMapperContext typeMapperContext)
   {
      if (context.MapperExtensionsType == null)
      {
         return new PartialClassGenerator($"{MapperTypeName}Extensions")
         {
            IsStatic = true,
            Modifier = ComputeModifier(typeMapperContext),
            Namespace = context.MapperType?.ContainingNamespace
         };
      }

      return new PartialClassGenerator(context.MapperExtensionsType)
      {
         IsStatic = true,
      };
   }

   private void GenerateMapperProperty(PartialClassGenerator generationContext)
   {
      if (generationContext.ContainsProperty("Mapper"))
         return;

      var builder = new StringBuilder();

      var defaultMapper = context.MapperType?.GetProperty(x => x.Name == "Default");
      var fullMapperName = context.MapperType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? context.MapperName;
      if (defaultMapper == null || defaultMapper.IsStatic)
      {
         builder.AppendLine("/// <summary>");
         builder.AppendLine($"/// The instance of the <see cref=\"{MapperTypeName}\"/> all extension methods use.");
         builder.AppendLine("/// You can customize this by implementing this property in your own partial implementation of the extensions class.");
         builder.AppendLine("/// </summary>");
         builder.AppendLine($"private static {fullMapperName} Mapper => {fullMapperName}.Default;/*NEWLINE*/");
         generationContext.AppendLine(builder.ToString());
      }
      else
      {
         var defaultConstructor = context.MapperType.GetDefaultConstructor();
         if (defaultConstructor == null || defaultConstructor.IsPrivate())
         {
            var message = "No default mapper could be found or can be created. Make sure there is a static Default property in the mapper class, or provide a accessible parameterless constructor.";
            builder.AppendLine($"private static {fullMapperName} Mapper => throw new global::System.NotSupportedException(\"{message}\");/*NEWLINE*/");
         }
         else
         {
            builder.AppendLine($"private static {fullMapperName} Mapper {{ get; }} = new {fullMapperName}();/*NEWLINE*/");
         }

         generationContext.AppendLine(builder.ToString());
      }
   }

   private static PartialClassGenerator GenerateMapperClass(ITypeMapperContext context)
   {
      var mapperGenerator = CreateMapperGenerator(context);
      var generationLogic = new TypeMapperGenerationLogic(context);
      generationLogic.Generate(mapperGenerator);
      return mapperGenerator;
   }

   private static PartialClassGenerator CreateMapperGenerator(ITypeMapperContext context)
   {
      if (context.MapperType != null)
         return new PartialClassGenerator(context.MapperType);
      if (context.MapperName != null)
         return new PartialClassGenerator(context.MapperName);

      return null;
   }

   private void GenerateExtensionMethod(PartialClassGenerator extensionsClass, INamedTypeSymbol sourceType, INamedTypeSymbol targetType)
   {
      var mapMethod = context.MapperType?.FindMethod("Map", sourceType, targetType);
      if (mapMethod != null && mapMethod.IsPrivate())
      {
         // If the map method is private (this happens only if the user declared it on its own, and made it private)
         // we can not generate an reasonable extension method
         return;
      }

      var targetName = targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      var sourceName = sourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      var valueName = sourceType.Name.ToFirstLower();

      var builder = new StringBuilder();
      builder.AppendLine($"{ComputeModifier(context)} static {targetName} To{targetType.Name}(this {sourceName} {valueName})");
      builder.AppendLine("{");

      builder.AppendLine($"if ({valueName} == null)");
      builder.AppendLine($"throw new global::System.ArgumentNullException(nameof({valueName}));/*NEWLINE*/");

      builder.AppendLine($"var result = Mapper is MagicMap.ITypeFactory<{targetName}, {sourceName}> factory");
      builder.AppendLine($"? factory.Create({valueName})");

      var constructor = targetType.GetDefaultConstructor();
      if (constructor == null || constructor.IsPrivate())
      {
         builder.AppendLine($": throw new global::System.NotSupportedException(\"The target type {targetType.Name} can not be created. Provide a accessible constructor without parameters or implement the ITypeFactory interface in the responsible mapper.\");");
      }
      else
      {
         builder.AppendLine($": new {targetName}();");
      }
      builder.AppendLine($"Mapper.Map({valueName}, result);");
      builder.AppendLine("return result;");
      builder.AppendLine("}");

      extensionsClass.AppendLine(builder.ToString());
   }

   #endregion


}