using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Myce.Extensions;

namespace Builder.Domain.Mappers
{
   public static class ProjectLanguageMapper
   {
      public static IEnumerable<Loop> ToLoop(this IEnumerable<ProjectLanguage> languages, string defaultLanguage)
      {
         if (languages.IsNull())
         {
            return null;
         }

         var loopItems = new List<Loop>();

         foreach (var language in languages)
         {
            var item = new Loop
            {
               Icon = language.Icon,
               Title = language.Label,
               Link = defaultLanguage == language.Code 
                      ? string.Concat("index", Const.Extension.WebPages)
                      : string.Concat("index", "-", language.Code, Const.Extension.WebPages)
            };

            loopItems.Add(item);
         }

         return loopItems;
      }
   }
}
