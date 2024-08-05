using Builder.Domain.Mappers;
using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Extensions;
using System.Text;

namespace Builder.Domain
{
   public interface IBuildLoop
   {
      string BlogPostsLastX(IEnumerable<Content> posts, string templateBase, int numberOfPosts);

      string Languages(IEnumerable<Language> languages, string defaultLanguage, string templateBase);

      string Menu(IEnumerable<Content> menu, string templateBase);

      string SocialMedia(IEnumerable<AuthorSocialMedia> socialMedia, string templateBase);

   }
   public class BuildLoop : IBuildLoop
   {
      private readonly ILogger<BuilderService> _logger;

      public BuildLoop(ILogger<BuilderService> logger)
      {
         _logger = logger;
      }
      public string BlogPostsLastX(IEnumerable<Content> posts, string templateBase, int numberOfPosts)
      {
         var postLastX = posts.OrderByDescending(x => x.Created).Take(numberOfPosts);
         var loopItems = posts.ToLoop();

         return ParseLoop(templateBase, loopItems);
      }

      public string Languages(IEnumerable<Language> languages, string defaultLanguage, string templateBase)
      {
         var loopItems = languages.ToLoop(defaultLanguage);

         return ParseLoop(templateBase, loopItems);
      }

      public string Menu(IEnumerable<Content> menu, string templateBase)
      {
         var loopItems = menu.ToLoop();

         return ParseLoop(templateBase, loopItems);
      }

      public string SocialMedia(IEnumerable<AuthorSocialMedia> socialMedia, string templateBase)
      {
         var loopItems = socialMedia.ToLoop();

         return ParseLoop(templateBase, loopItems);
      }

      private static string ParseLoop(string templateBase, IEnumerable<Loop> loopItems)
      {
         if (templateBase.IsNull())
         {
            return string.Empty;
         } 

         var result = new StringBuilder();

         foreach (var item in loopItems)
         {
            string template = templateBase;
            template = template.Replace(Const.Tag.Loop.Icon, item.Icon.EmptyIfIsNull());
            template = template.Replace(Const.Tag.Loop.Link, item.Link);
            template = template.Replace(Const.Tag.Loop.Title, item.Title);
            template = template.Replace(Const.Tag.Loop.Description, item.Description.EmptyIfIsNull());

            result.Append(template);
         }

         return result.ToString();
      }
   }
}
