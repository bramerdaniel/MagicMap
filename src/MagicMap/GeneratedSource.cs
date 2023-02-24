// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratedSource.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using System.Collections.Generic;
   using System.Linq;

   using Microsoft.CodeAnalysis;

   internal class GeneratedSource
   {
      #region Constants and Fields

      private List<Diagnostic> diagnostics;

      #endregion

      #region Public Properties

      /// <summary>Gets or sets the generated source code.</summary>
      public string Code { get; set; }

      /// <summary>Gets the reported diagnostics.</summary>
      public IEnumerable<Diagnostic> Diagnostics => diagnostics ?? Enumerable.Empty<Diagnostic>();

      /// <summary>Gets a value indicating whether the generated source will be added to the results.</summary>
      public bool Enabled { get; private set; } = true;

      /// <summary>Gets or sets the name of the generated source.</summary>
      public string Name { get; set; }

      #endregion

      #region Public Methods and Operators

      /// <summary>Adds the specified diagnostic to the source.</summary>
      /// <param name="diagnostic">The diagnostic.</param>
      /// <returns></returns>
      public void AddDiagnostic(Diagnostic diagnostic)
      {
         EnsureDiagnostics();

         if (diagnostic.Severity == DiagnosticSeverity.Error)
            Disable();

         diagnostics.Add(diagnostic);
      }

      /// <summary>Disables the source.</summary>
      public void Disable()
      {
         Enabled = false;
      }

      #endregion

      #region Methods

      private void EnsureDiagnostics()
      {
         if (diagnostics == null)
            diagnostics = new List<Diagnostic>();
      }

      #endregion
   }
}