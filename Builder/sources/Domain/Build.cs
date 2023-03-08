using Builder.Domain.Internationalization;
using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Builder.Domain
{
    public interface IBuild
   {
      void Builder(string projectName);
   }

   public class Build : IBuild
   {
      private readonly ILoad _load;
      private readonly IBuildLoop _loop;
      private readonly IBuildTag _tag;
      private readonly ILogger<Build> _logger;
      private readonly ITranslate _translate;
      private readonly IData _data;
      private readonly ISave _save;

      private Project project;
      private Layout layoutBase;

      public Build(ILoad load,
         IBuildLoop loop,
         IBuildTag tag,
         ILogger<Build> logger,
         IData data,
         ISave save,
         ITranslate translate)
      {
         _load = load;
         _loop = loop;
         _tag = tag;
         _logger = logger;
         _data = data;
         _save = save;
         _translate = translate;
      }

      public void Builder(string projectName)
      {
         project = _load.Project(projectName);

         if (project.IsNotNull())
         {
            //todo add validate here

            layoutBase = _load.Layout(project.Folders.Template);

            foreach (var language in project.Languages)
            {
               _logger.LogInformation("Starting create site in {0}", language.Label);

               var site = project.Sites.First(s => s.Language == language.Code);

               site.SetIndex(project.DefaultLanguage);

               var contentList = _data.GetContentList(project.Folders, language.Code, site.Folders.Pages, site.Folders.BlogPosts);

               string layout = ParseLoops(site, contentList);

               site.Modified = contentList.Max(x => x.Updated.HasValue ? x.Updated.Value : x.Created);

               layout = _translate.To(language.Code, project.Folders.Template, layout);

               layout = _tag.Index(layout, site);

               _save.ContentListToJson(contentList, Path.Combine(project.Folders.Project, site.Language));
               _save.WebPage("test.html", layout);

               _logger.LogInformation("Ending create site in {0}", language.Label);
            }
         }
      }

      private string ParseLoops(ProjectSite site, List<Content> contentList)
      {
         //todo => this methods should be async
         var loopLanguages = _loop.Languages(project.Languages, project.DefaultLanguage, layoutBase.Loops.Languages);

         var loopSocialMedia = _loop.SocialMedia(site.Author.SocialMedia, layoutBase.Loops.SocialMedia);

         var menuList = contentList.Where(content => content.Type == ContentType.Page && content.Menu);
         var loopMenu = _loop.Menu(menuList, layoutBase.Loops.Menu);

         var posts = contentList.Where(content => content.Type == ContentType.Post);
         var blogPostsLast10 = _loop.BlogPostsLastX(posts, layoutBase.Loops.BlogPostLast10, 10);
         var blogPostsLast5 = _loop.BlogPostsLastX(posts, layoutBase.Loops.BlogPostLast5, 5);
         var blogPostsLast3 = _loop.BlogPostsLastX(posts, layoutBase.Loops.BlogPostLast3, 3);

         var layout = layoutBase.Index;

         layout = layout.Replace(Const.Tag.Site.Loop.Languages, loopLanguages);
         layout = layout.Replace(Const.Tag.Site.Loop.Menu, loopMenu);
         layout = layout.Replace(Const.Tag.Site.Loop.SocialMedia, loopSocialMedia);
         layout = layout.Replace(Const.Tag.Site.Loop.BlogPostLast10, blogPostsLast10);
         layout = layout.Replace(Const.Tag.Site.Loop.BlogPostLast5, blogPostsLast5);
         layout = layout.Replace(Const.Tag.Site.Loop.BlogPostLast3, blogPostsLast3);

         return layout;
      }
   }
}
