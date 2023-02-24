// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vocabularies.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMap
{
   using System;
   using System.Diagnostics.CodeAnalysis;
   using System.Threading;

   /// <summary>Container for registered Vocabularies.  At present, only a single vocabulary is supported: Default.</summary>
   [SuppressMessage("ReSharper", "StringLiteralTypo")]
   public static class Vocabularies
   {
      #region Constants and Fields

      private static readonly Lazy<Vocabulary> Instance = new Lazy<Vocabulary>(BuildDefault, LazyThreadSafetyMode.PublicationOnly);

      #endregion

      #region Public Properties

      /// <summary>
      ///    The default vocabulary used for singular/plural irregularities. Rules can be added to this vocabulary and will be picked up by called to
      ///    Singularize() and Pluralize(). At this time, multiple vocabularies and removing existing rules are not supported.
      /// </summary>
      public static Vocabulary Default => Instance.Value;

      #endregion

      #region Methods

      private static Vocabulary BuildDefault()
      {
         var defaultVocabulary = new Vocabulary();

         defaultVocabulary.AddPlural("$", "s");
         defaultVocabulary.AddPlural("s$", "s");
         defaultVocabulary.AddPlural("(ax|test)is$", "$1es");
         defaultVocabulary.AddPlural("(octop|vir|alumn|fung|cact|foc|hippopotam|radi|stimul|syllab|nucle)us$", "$1i");
         defaultVocabulary.AddPlural("(alias|bias|iris|status|campus|apparatus|virus|walrus|trellis)$", "$1es");
         defaultVocabulary.AddPlural("(buffal|tomat|volcan|ech|embarg|her|mosquit|potat|torped|vet)o$", "$1oes");
         defaultVocabulary.AddPlural("([dti])um$", "$1a");
         defaultVocabulary.AddPlural("sis$", "ses");
         defaultVocabulary.AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
         defaultVocabulary.AddPlural("(hive)$", "$1s");
         defaultVocabulary.AddPlural("([^aeiouy]|qu)y$", "$1ies");
         defaultVocabulary.AddPlural("(x|ch|ss|sh)$", "$1es");
         defaultVocabulary.AddPlural("(matr|vert|ind|d)(ix|ex)$", "$1ices");
         defaultVocabulary.AddPlural("(^[m|l])ouse$", "$1ice");
         defaultVocabulary.AddPlural("^(ox)$", "$1en");
         defaultVocabulary.AddPlural("(quiz)$", "$1zes");
         defaultVocabulary.AddPlural("(buz|blit|walt)z$", "$1zes");
         defaultVocabulary.AddPlural("(hoo|lea|loa|thie)f$", "$1ves");
         defaultVocabulary.AddPlural("(alumn|alg|larv|vertebr)a$", "$1ae");
         defaultVocabulary.AddPlural("(criteri|phenomen)on$", "$1a");

         defaultVocabulary.AddSingular("s$", "");
         defaultVocabulary.AddSingular("(n)ews$", "$1ews");
         defaultVocabulary.AddSingular("([dti])a$", "$1um");
         defaultVocabulary.AddSingular("(analy|ba|diagno|parenthe|progno|synop|the|ellip|empha|neuro|oa|paraly)ses$", "$1sis");
         defaultVocabulary.AddSingular("([^f])ves$", "$1fe");
         defaultVocabulary.AddSingular("(hive)s$", "$1");
         defaultVocabulary.AddSingular("(tive)s$", "$1");
         defaultVocabulary.AddSingular("([lr]|hoo|lea|loa|thie)ves$", "$1f");
         defaultVocabulary.AddSingular("(^zomb)?([^aeiouy]|qu)ies$", "$2y");
         defaultVocabulary.AddSingular("(s)eries$", "$1eries");
         defaultVocabulary.AddSingular("(m)ovies$", "$1ovie");
         defaultVocabulary.AddSingular("(x|ch|ss|sh)es$", "$1");
         defaultVocabulary.AddSingular("(^[m|l])ice$", "$1ouse");
         defaultVocabulary.AddSingular("(?<!^[a-z])(o)es$", "$1");
         defaultVocabulary.AddSingular("(shoe)s$", "$1");
         defaultVocabulary.AddSingular("(cris|ax|test)es$", "$1is");
         defaultVocabulary.AddSingular("(octop|vir|alumn|fung|cact|foc|hippopotam|radi|stimul|syllab|nucle)i$", "$1us");
         defaultVocabulary.AddSingular("(alias|bias|iris|status|campus|apparatus|virus|walrus|trellis)es$", "$1");
         defaultVocabulary.AddSingular("^(ox)en", "$1");
         defaultVocabulary.AddSingular("(matr|d)ices$", "$1ix");
         defaultVocabulary.AddSingular("(vert|ind)ices$", "$1ex");
         defaultVocabulary.AddSingular("(quiz)zes$", "$1");
         defaultVocabulary.AddSingular("(buz|blit|walt)zes$", "$1z");
         defaultVocabulary.AddSingular("(alumn|alg|larv|vertebr)ae$", "$1a");
         defaultVocabulary.AddSingular("(criteri|phenomen)a$", "$1on");
         defaultVocabulary.AddSingular("([b|r|c]ook|room|smooth)ies$", "$1ie");

         defaultVocabulary.AddIrregular("person", "people");
         defaultVocabulary.AddIrregular("man", "men");
         defaultVocabulary.AddIrregular("human", "humans");
         defaultVocabulary.AddIrregular("child", "children");
         defaultVocabulary.AddIrregular("sex", "sexes");
         defaultVocabulary.AddIrregular("glove", "gloves");
         defaultVocabulary.AddIrregular("move", "moves");
         defaultVocabulary.AddIrregular("goose", "geese");
         defaultVocabulary.AddIrregular("wave", "waves");
         defaultVocabulary.AddIrregular("foot", "feet");
         defaultVocabulary.AddIrregular("tooth", "teeth");
         defaultVocabulary.AddIrregular("curriculum", "curricula");
         defaultVocabulary.AddIrregular("database", "databases");
         defaultVocabulary.AddIrregular("zombie", "zombies");
         defaultVocabulary.AddIrregular("personnel", "personnel");
         defaultVocabulary.AddIrregular("cache", "caches");
         defaultVocabulary.AddIrregular("ex", "exes", matchEnding: false);
         defaultVocabulary.AddIrregular("is", "are", matchEnding: false);
         defaultVocabulary.AddIrregular("that", "those", matchEnding: false);
         defaultVocabulary.AddIrregular("this", "these", matchEnding: false);
         defaultVocabulary.AddIrregular("bus", "buses", matchEnding: false);
         defaultVocabulary.AddIrregular("die", "dice", matchEnding: false);
         defaultVocabulary.AddIrregular("tie", "ties", matchEnding: false);

         defaultVocabulary.AddUncountable("staff");
         defaultVocabulary.AddUncountable("training");
         defaultVocabulary.AddUncountable("equipment");
         defaultVocabulary.AddUncountable("information");
         defaultVocabulary.AddUncountable("corn");
         defaultVocabulary.AddUncountable("milk");
         defaultVocabulary.AddUncountable("rice");
         defaultVocabulary.AddUncountable("money");
         defaultVocabulary.AddUncountable("species");
         defaultVocabulary.AddUncountable("series");
         defaultVocabulary.AddUncountable("fish");
         defaultVocabulary.AddUncountable("sheep");
         defaultVocabulary.AddUncountable("deer");
         defaultVocabulary.AddUncountable("aircraft");
         defaultVocabulary.AddUncountable("oz");
         defaultVocabulary.AddUncountable("tsp");
         defaultVocabulary.AddUncountable("tbsp");
         defaultVocabulary.AddUncountable("ml");
         defaultVocabulary.AddUncountable("l");
         defaultVocabulary.AddUncountable("water");
         defaultVocabulary.AddUncountable("waters");
         defaultVocabulary.AddUncountable("semen");
         defaultVocabulary.AddUncountable("sperm");
         defaultVocabulary.AddUncountable("bison");
         defaultVocabulary.AddUncountable("grass");
         defaultVocabulary.AddUncountable("hair");
         defaultVocabulary.AddUncountable("mud");
         defaultVocabulary.AddUncountable("elk");
         defaultVocabulary.AddUncountable("luggage");
         defaultVocabulary.AddUncountable("moose");
         defaultVocabulary.AddUncountable("offspring");
         defaultVocabulary.AddUncountable("salmon");
         defaultVocabulary.AddUncountable("shrimp");
         defaultVocabulary.AddUncountable("someone");
         defaultVocabulary.AddUncountable("swine");
         defaultVocabulary.AddUncountable("trout");
         defaultVocabulary.AddUncountable("tuna");
         defaultVocabulary.AddUncountable("corps");
         defaultVocabulary.AddUncountable("scissors");
         defaultVocabulary.AddUncountable("means");
         defaultVocabulary.AddUncountable("mail");
         defaultVocabulary.AddUncountable("metadata");

         return defaultVocabulary;
      }

      #endregion
   }
}