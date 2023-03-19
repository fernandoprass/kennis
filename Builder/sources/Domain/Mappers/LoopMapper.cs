using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Myce.Extensions;

namespace Builder.Domain.Mappers
{
   public static class LoopMapper
   {
      public static IEnumerable<Loop> ToLoop(this IEnumerable<Language> languages, string defaultLanguage)
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
               Link = ProjectSite.GetIndexFileName(defaultLanguage, language.Code)
            };

            loopItems.Add(item);
         }

         return loopItems;
      }

      public static IEnumerable<Loop> ToLoop(this IEnumerable<AuthorSocialMedia> socialMedia)
      {
         if (socialMedia.IsNull())
         {
            return null;
         }

         var loopItems = new List<Loop>();

         foreach (var media in socialMedia)
         {
            var item = new Loop
            {
               Icon = media.Icon,
               Title = media.Name,
               Link = media.Link
            };

            loopItems.Add(item);
         }

         return loopItems;
      }

      public static IEnumerable<Loop> ToLoop(this IEnumerable<Content> contentList)
      {
         if (contentList.IsNull())
         {
            return null;
         }

         var loopItems = new List<Loop>();

         foreach (var content in contentList)
         {
            var item = new Loop
            {
               Icon = content.Icon,
               Title = content.Title,
               Link = content.Url,
               Description= content.Description
            };

            loopItems.Add(item);
         }

         return loopItems;
      }
   }
}
