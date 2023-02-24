// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="consolovers">
//   Copyright (c) daniel bramer 2022 - 2022
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MagicMapReferenceTester
{
   static class Program
   {
   
      #region Methods

     

      static void Main()
      {
         var source = new Person{ Name = "Peter", Age = 45 };
         var personModel = new PersonModel();

         new PersonMapper().Map(source, personModel);

         Console.WriteLine(personModel);
         Console.ReadLine();
      }

 

      #endregion
   }

}