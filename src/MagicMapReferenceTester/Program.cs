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
         var source = new Person { Name = "Peter", Size = 45.3, AgeInYears = 2 };
         var model = source.ToPersonModel();
         Console.WriteLine(model);

         model = model.ToPerson().ToPersonModel();
         Console.WriteLine(model);

         var personModel = new PersonModel();

         var person = personModel.ToPerson();
         var person2 = person.ToPerson();
         

         new PersonToModelMapper().Map(source, personModel);

         Console.WriteLine(personModel);
         Console.ReadLine();
      }

      #endregion
   }
}