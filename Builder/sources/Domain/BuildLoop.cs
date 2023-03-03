using Builder.Domain.Layouts;
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
      string BlogPostsLastX(IEnumerable<Content> posts, string layoutBase, int numberOfPosts);

      string Languages(IEnumerable<ProjectLanguage> languages, string defaultLanguage, string layoutBase);

      string Menu(IEnumerable<Content> menu, string layoutBase);

      string SocialMedia(IEnumerable<AuthorSocialMedia> socialMedia, string layoutBase);

   }
   public class BuildLoop : IBuildLoop
   {
      private readonly ILogger<Build> _logger;

      public BuildLoop(ILogger<Build> logger)
      {
         _logger = logger;
      }
      public string BlogPostsLastX(IEnumerable<Content> posts, string layoutBase, int numberOfPosts)
      {
         var postLastX = posts.OrderByDescending(x => x.Created).Take(numberOfPosts);
         var loopItems = posts.ToLoop();

         return ParseLoop(layoutBase, loopItems);
      }

      public string Languages(IEnumerable<ProjectLanguage> languages, string defaultLanguage, string layoutBase)
      {
         var loopItems = languages.ToLoop(defaultLanguage);

         return ParseLoop(layoutBase, loopItems);
      }

      public string Menu(IEnumerable<Content> menu, string layoutBase)
      {
         var loopItems = menu.ToLoop();

         return ParseLoop(layoutBase, loopItems);
      }

      public string SocialMedia(IEnumerable<AuthorSocialMedia> socialMedia, string layoutBase)
      {
         var loopItems = socialMedia.ToLoop();

         return ParseLoop(layoutBase, loopItems);
      }

      private static string ParseLoop(string layoutBase, IEnumerable<Loop> loopItems)
      {
         if (layoutBase.IsNull())
         {
            return string.Empty;
         } 

         var result = new StringBuilder();

         foreach (var item in loopItems)
         {
            string layout = layoutBase;
            layout = layout.Replace(Const.Tag.Loop.Icon, item.Icon.EmptyIfIsNull());
            layout = layout.Replace(Const.Tag.Loop.Link, item.Link);
            layout = layout.Replace(Const.Tag.Loop.Title, item.Title);
            layout = layout.Replace(Const.Tag.Loop.Description, item.Description.EmptyIfIsNull());

            result.Append(layout);
         }

         return result.ToString();
      }
   }
}
