namespace FluentSetupTester
{
   using System.Collections.Generic;
   using System.Runtime.CompilerServices;
   using MagicMap;

   [TypeMapper(typeof(A), typeof(B))]
   public partial class RoomSetup
   {
      private void MapAge(A target, double age)
      {
         target.Age = (int)age;
      }
   }

   class A
   {
      public string Name { get; set; }
      public int Age{ get; set; }
   }

   class B
   {
      public string Name { get; set; }
      public double Age{ get; set; }
   }
}