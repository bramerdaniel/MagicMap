// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupClassInfo.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using System;
   using System.Linq;

   using Microsoft.CodeAnalysis;
   using Microsoft.CodeAnalysis.CSharp.Syntax;

   /// <summary>Data class containing all the required information along the generation process</summary>
   internal class SetupClassInfo
   {
      #region Constructors and Destructors

      public SetupClassInfo(MagicGeneratorManager context, ClassDeclarationSyntax candidate, SemanticModel semanticModel, INamedTypeSymbol classSymbol,
         AttributeData fluentSetupAttribute)
      {
         Context = context;
         
         // TODO remove this unused property
         ClassSyntax = candidate ?? throw new ArgumentNullException(nameof(candidate));
         // TODO remove this unused property
         SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
         
         ClassSymbol = classSymbol ?? throw new ArgumentNullException(nameof(classSymbol));
         FluentSetupAttribute = fluentSetupAttribute ?? throw new ArgumentNullException(nameof(fluentSetupAttribute));
      }

      #endregion

      #region Public Properties

      /// <summary>Gets or sets the class symbol.</summary>
      public INamedTypeSymbol ClassSymbol { get; }

      public ClassDeclarationSyntax ClassSyntax { get; }

      public AttributeData FluentSetupAttribute { get; }

      public SemanticModel SemanticModel { get; }

      public TypedConstant TargetMode => FluentSetupAttribute.GetTargetMode();

      public TypedConstant TargetType => FluentSetupAttribute.GetTargetType();

      #endregion

      #region Properties

      private MagicGeneratorManager Context { get; }

      #endregion

      #region Public Methods and Operators

      public bool IsValidSetup()
      {
         if (ClassSymbol == null)
            return false;

         if (FluentSetupAttribute == null)
            return false;

         return true;
      }

      #endregion

      #region Methods

      private TypedConstant GetTargetMode()
      {
         if (TryGetNamedArgument("TargetMode", out var targetType) && targetType.Kind == TypedConstantKind.Enum)
            return targetType;
         return default;
      }

      private TypedConstant GetTargetType()
      {
         if (TryGetConstructorArgument(TypedConstantKind.Type, out var targetType))
            return targetType;
         if (TryGetNamedArgument("TargetType", out targetType) && targetType.Kind == TypedConstantKind.Type)
            return targetType;
         return default;
      }

      private bool TryGetConstructorArgument(TypedConstantKind type, out TypedConstant targetType)
      {
         var attribute = FluentSetupAttribute;
         if (attribute != null && attribute.ConstructorArguments.Length > 0)
         {
            targetType = attribute.ConstructorArguments.FirstOrDefault(x => x.Kind == type);
            return !targetType.IsNull;
         }

         targetType = default;
         return false;
      }

      private bool TryGetNamedArgument(string argumentName, out TypedConstant typedConstant)
      {
         var attribute = FluentSetupAttribute;
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

      #endregion
   }
}