// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentGeneratorContext.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using System;
   using System.Collections.Generic;

   using Microsoft.CodeAnalysis;
   using Microsoft.CodeAnalysis.CSharp.Syntax;

   internal class MagicGeneratorManager
    {
        #region Public Properties

        public ITypeSymbol BooleanType { get; private set; }

        public Compilation Compilation { get; private set; }
        
        public INamedTypeSymbol VoidType { get; private set; }

        #endregion

        #region Properties


        internal static string TypeMapperAttributeName => "MagicMap.TypeMapperAttribute";

        internal INamedTypeSymbol TypeMapperAttribute { get; private set; }

        #endregion

        #region Public Methods and Operators

        public static MagicGeneratorManager FromCompilation(Compilation compilation)
        {
            return new MagicGeneratorManager
            {
                Compilation = compilation,
                TypeMapperAttribute = compilation.GetTypeByMetadataName(TypeMapperAttributeName),
                BooleanType = compilation.GetTypeByMetadataName("System.Boolean"),
                VoidType = compilation.GetTypeByMetadataName("System.Void")
            };
        }

        public SetupClassInfo CreateFluentSetupInfo(ClassDeclarationSyntax setupCandidate)
        {
            if (InspectAndInitialize(setupCandidate, out SetupClassInfo classInfo))
                return classInfo;

            throw new ArgumentException($"The specified {nameof(ClassDeclarationSyntax)} is not a fluent setup class", nameof(setupCandidate));
        }

        public IEnumerable<SetupClassInfo> FindFluentSetups(IEnumerable<ClassDeclarationSyntax> setupCandidates)
        {
            foreach (var setupCandidate in setupCandidates)
            {
                if (InspectAndInitialize(setupCandidate, out SetupClassInfo classInfo))
                    yield return classInfo;
            }
        }

        public bool TryGetMissingType(out string missingType)
        {
            if (TypeMapperAttribute == null)
            {
                missingType = TypeMapperAttributeName;
                return true;
            }

            missingType = null;
            return false;
        }

        #endregion

        #region Methods

        internal bool IsTypeMapperAttribute(AttributeData attributeData)
        {
            if (TypeMapperAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                return true;

            return false;
        }

        private bool InspectAndInitialize(ClassDeclarationSyntax candidate, out SetupClassInfo setupClassInfo)
        {
            setupClassInfo = null;
            var semanticModel = Compilation.GetSemanticModel(candidate.SyntaxTree);

            if (!(ModelExtensions.GetDeclaredSymbol(semanticModel, candidate) is INamedTypeSymbol classSymbol))
                return false;

            foreach (var attributeData in classSymbol.GetAttributes())
            {
                if (IsTypeMapperAttribute(attributeData))
                {
                    setupClassInfo = new SetupClassInfo(this, candidate, semanticModel, classSymbol, attributeData);
                    return true;
                }
            }

            return false;
        }
      
        private bool IsSetupClass(ClassDeclarationSyntax candidate)
        {
            var semanticModel = Compilation.GetSemanticModel(candidate.SyntaxTree);

            var classSymbol = (ITypeSymbol)ModelExtensions.GetDeclaredSymbol(semanticModel, candidate);

            if (classSymbol == null)
                return false;

            foreach (var attributeData in classSymbol.GetAttributes())
            {
                if (TypeMapperAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                    return true;

                var attributeName = attributeData.AttributeClass?.Name;

                if (attributeName == "FluentSetupAttribute")
                    return true;

                if (attributeName == "FluentSetup")
                    return true;
            }

            return false;
        }

        #endregion

 
      

        public bool TryCreateMagicGenerator(ClassDeclarationSyntax classDeclarationSyntax, out IMagicGenerator generator)
        {
           generator = null;
           var semanticModel = Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
           var classSymbol = ModelExtensions.GetDeclaredSymbol(semanticModel, classDeclarationSyntax);
           foreach (var attribute in classSymbol.GetAttributes())
           {

              if (IsTypeMapperAttribute(attribute))
              {
                 generator = new TypeMapperGenerator();
                 return true;
              }

           }

           return false;
        }
    }
}
