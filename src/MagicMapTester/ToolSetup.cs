namespace FluentSetupTester
{
   using System.Collections.Generic;
   using System.Runtime.CompilerServices;
   using MagicMap;

   [TypeMapper(typeof(Person), typeof(PersonModel))]
   public partial class PersonMapper
   {
      private void MapAge(Person target, double age)
      {
         target.Age = (int)age;
      }
   }

   class Person
   {
      public string Name { get; set; }
      public int Age{ get; set; }
   }

   class PersonModel
   {
      public string Name { get; set; }
      public double Age{ get; set; }
   }
}