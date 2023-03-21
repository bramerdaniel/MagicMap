// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratedSource.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using System;
   using System.Collections.Generic;
   using System.Linq;

   using Microsoft.CodeAnalysis;

   internal class GeneratedSource
   {
      private IList<Diagnostic> diagnostics;

      #region Public Properties

      /// <summary>Gets or sets the generated source code.</summary>
      public string Code { get; set; }

      /// <summary>Gets the reported diagnostics.</summary>
      public IList<Diagnostic> Diagnostics
      {
         get => diagnostics ??= new List<Diagnostic>();
         set
         {
            diagnostics = value;
            foreach (var diagnostic in Diagnostics)
               if (diagnostic.Severity == DiagnosticSeverity.Error)
                  Disable();
         }
      }

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

         Diagnostics.Add(diagnostic);
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