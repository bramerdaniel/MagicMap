
namespace MagicMap.IntegrationTests.Tests.CustomFactoryMethod
{
   [TypeMapper(typeof(SourceHandler), typeof(TargetHandler))]
   internal partial class HandlerMapper
   {
      public HandlerMapper()
      {
         throw new AssertFailedException("Should never be called with true");
      }

      public HandlerMapper(bool flag)
      {
      }
   }

   internal class SourceHandler
   {
      public int Age { get; set; }
   }

   internal class TargetHandler
   {
      public int Age { get; set; }
   }

   static partial class HandlerMapperExtensions
   {
      internal static HandlerMapper Mapper => new HandlerMapper(true);
   }
}
