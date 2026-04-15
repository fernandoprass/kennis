using Kennis.Domain.Models;

namespace Kennis.Domain.Mappers;

public static class LoopMapper
{
   //todo => change IEnumerable<Language> for IEnumerable<ProjectSite>
   public static IEnumerable<Loop> ToLoop(this IEnumerable<Language> languages, string defaultLanguage)
   {
      if (languages == null )
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
            Link = language.IndexFileName
         };

         loopItems.Add(item);
      }

      return loopItems;
   }

   public static IEnumerable<Loop> ToLoop(this IEnumerable<AuthorSocialMedia> socialMedia)
   {
      if (socialMedia == null )
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
      if (contentList == null)
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
