using Builder.Domain.Models;

namespace Builder.Domain
{
   public interface IBuildSetup
   {
      Layout Layout(string templateFolder);
      
   }
   public class BuildSetup : IBuildSetup
   {
      private readonly ILoad _load;

      public BuildSetup(ILoad load)
      {
         _load = load;
      }

      #region Load Layout
      public Layout Layout(string templateFolder)
      {
         return _load.Layout(templateFolder);
      }
      #endregion
   }
}
