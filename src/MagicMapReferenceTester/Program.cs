// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2023
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMapReferenceTester
{
   static class Program
   {
      #region Methods

      static void Main()
      {
         var source = new Person { Name = "Peter", Age = 45 };
         var model = source.ToPersonModel();
         Console.WriteLine(model);

         var personModel = new PersonModel();


         new PersonMapper().Map(source, personModel);

         Console.WriteLine(personModel);
         var person = personModel.ToPerson();
         Console.ReadLine();
      }

      #endregion
   }
}