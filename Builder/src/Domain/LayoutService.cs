using Builder.Domain.Models;
using Myce.Extensions;

namespace Builder.Domain
{
   public interface ILayoutService
   {
      bool Load(string templateFolder);
      bool Translate(string language);

   }
   public class LayoutService : ILayoutService
   {
      private readonly ILoad _load;
      private readonly ILayoutTranslateService _layoutTranslateService;

      private Layout _layout;

      public LayoutService(ILoad load, ILayoutTranslateService layoutTranslateService)
      {
         _load = load;
         _layoutTranslateService = layoutTranslateService;
      }

      public bool Load(string templateFolder)
      {
         _layout = _load.Layout(templateFolder);

         return _layout.IsNotNull();
      }

      public bool Translate(string language)
      { 
         return true;
      }
   }
}
