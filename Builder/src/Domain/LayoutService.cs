using Builder.Domain.Models;

namespace Builder.Domain
{
   public interface ILayoutService
   {
      Layout Load(string templateFolder);
      
   }
   public class LayoutService : ILayoutService
   {
      private readonly ILoad _load;

      public LayoutService(ILoad load)
      {
         _load = load;
      }

      #region Load Layout
      public Layout Load(string templateFolder)
      {
         return _load.Layout(templateFolder);
      }
      #endregion
   }
}
