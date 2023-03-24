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

      private string LoopLanguagesParsed { get; set; }
      private string LoopSocialMediaParsed { get; set; }
      private string LoopMenuParsed { get; set; }
      private string BlogPostsLast10Parsed { get; set; }
      private string BlogPostsLast5Parsed { get; set; }
      private string BlogPostsLast3Parsed { get; set; }



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

            foreach (var site in project.Sites)
            {
               _logger.LogInformation("Starting create site in {0}", site.Language.Label);

               site.SetIndexFileName(project.DefaultLanguageCode);

               var contentList = _data.GetContentList(project.Folders, site.Language.Code, site.Folders.Pages, site.Folders.BlogPosts);

               ParseLoopLayouts(site, contentList);

               site.Modified = contentList.Max(x => x.Updated.HasValue ? x.Updated.Value : x.Created);

               ParseIndexFile(site, contentList);

               _logger.LogInformation("Ending create site in {0}", site.Language.Label);
            }
         }
      }

      private void ParseIndexFile(ProjectSite site, List<Content> contentList)
      {
         var layout = ParseHtmlFile(layoutBase.Index);

         layout = _translate.To(site.Language.Code, project.Folders.Template, layout);

         layout = _tag.Index(layout, site);

         _logger.LogInformation("Index html page parsed", site.Language.Label);

         _data.SaveContentList(Path.Combine(project.Folders.Project, site.Language.Code), contentList);

         _save.ToHtmlFile(site.IndexFileName, layout);
      }

      private void ParseLoopLayouts(ProjectSite site, List<Content> contentList)
      {
         //todo => this methods should be async
         var languages = project.Sites.Select(x => x.Language);

         LoopLanguagesParsed = _loop.Languages(languages, project.DefaultLanguageCode, layoutBase.Loops.Languages);

         LoopSocialMediaParsed = _loop.SocialMedia(site.Author.SocialMedia, layoutBase.Loops.SocialMedia);

         var menuList = contentList.Where(content => content.Type == ContentType.Page && content.Menu);
         LoopMenuParsed = _loop.Menu(menuList, layoutBase.Loops.Menu);

         var posts = contentList.Where(content => content.Type == ContentType.Post);
         BlogPostsLast10Parsed = _loop.BlogPostsLastX(posts, layoutBase.Loops.BlogPostLast10, 10);
         BlogPostsLast5Parsed = _loop.BlogPostsLastX(posts, layoutBase.Loops.BlogPostLast5, 5);
         BlogPostsLast3Parsed = _loop.BlogPostsLastX(posts, layoutBase.Loops.BlogPostLast3, 3);
      }

      private string ParseHtmlFile(string layout)
      {
         layout = layout.Replace(Const.Tag.Site.Loop.Languages, LoopLanguagesParsed);
         layout = layout.Replace(Const.Tag.Site.Loop.Menu, LoopMenuParsed);
         layout = layout.Replace(Const.Tag.Site.Loop.SocialMedia, LoopSocialMediaParsed);
         layout = layout.Replace(Const.Tag.Site.Loop.BlogPostLast10, BlogPostsLast10Parsed);
         layout = layout.Replace(Const.Tag.Site.Loop.BlogPostLast5, BlogPostsLast5Parsed);
         layout = layout.Replace(Const.Tag.Site.Loop.BlogPostLast3, BlogPostsLast3Parsed);

         return layout;
      }
   }
}
