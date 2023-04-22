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
      private readonly IBuildSetup _setup;
      private readonly IBuildTag _tag;
      private readonly ILogger<Build> _logger;
      private readonly ITranslate _translate;
      private readonly IData _data;
      private readonly ISave _save;

      private Project Project { get; set; }
      private Layout LayoutBase { get; set; }

      private string LoopLanguagesParsed { get; set; }
      private string LoopSocialMediaParsed { get; set; }
      private string LoopMenuParsed { get; set; }
      private string BlogPostsLast10Parsed { get; set; }
      private string BlogPostsLast5Parsed { get; set; }
      private string BlogPostsLast3Parsed { get; set; }

      public Build(ILoad load,
         IBuildLoop loop,
         IBuildSetup setup,
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
         _setup= setup;
         _translate = translate;
      }

      public void Builder(string projectName)
      {
         Setup(projectName);

         if (Project.IsNotNull())
         {
            foreach (var site in Project.Sites)
            {
               _save.Configure(Project.Folders.Destination, Path.Combine(Project.Folders.Project, site.Language.Code));

               _logger.LogInformation("Starting create site in {0}", site.Language.Label);

               _data.GetContentList(Project.Folders, site.Language.Code, site.Folders.Pages, site.Folders.BlogPosts);

               _data.UpdateContentList();

               var lastModified = _data.ContentList.Max(x => x.Updated.HasValue ? x.Updated.Value : x.Created);

               _data.UpdateProjectSiteModified(lastModified, site);

               _data.SaveContentList();

               ParseLoopLayouts(site, _data.ContentList);

               ParseIndexFile(site);

               ParseBlogIndexFile(site);

               ParseContentFile(site, ContentType.Page, _data.ContentList);

               ParseContentFile(site, ContentType.Post, _data.ContentList);

               _logger.LogInformation("Ending create site in {0}", site.Language.Label);
            }
         }
      }

      private void Setup(string projectName)
      {
         Project = _setup.ProjectGet(projectName);

         //todo add validate here
         if (Project.IsNotNull())
         {
            _setup.ProjectUpdateLanguageIndexFileName(Project.DefaultLanguageCode, Project.Sites);

            LayoutBase = _load.Layout(Project.Folders.Template);
         }
      }

      private void ParseIndexFile(ProjectSite site)
      {
         string layout = ParseHtmlFile(LayoutBase.Index);

         layout = _translate.To(site.Language.Code, Project.Folders.Template, layout);

         layout = _tag.Index(layout, site);

         _logger.LogInformation("Index html page parsed - " + site.Language.Label);

         _save.ToHtmlFile(site.Language.IndexFileName, layout);
      }

      private void ParseBlogIndexFile(ProjectSite site)
      {
         string layout = ParseHtmlFile(LayoutBase.Blog);

         layout = _translate.To(site.Language.Code, Project.Folders.Template, layout);

         layout = _tag.Index(layout, site);

         _logger.LogInformation("Blog index html page parsed");

         _save.ToHtmlFile(site.Folders.Blog + Const.File.Index, layout);
      }

      private void ParseContentFile(ProjectSite site, ContentType contentType, IEnumerable<Content> contentList)
      {
         var type = contentType == ContentType.Page ? "pages" : "posts";

         var contents = contentType == ContentType.Page
                        ? contentList.Where(x => x.Type.Equals(ContentType.Page))
                        : contentList.Where(x => x.Type.Equals(ContentType.Post));

         string layout = contentType == ContentType.Page
                         ? ParseHtmlFile(LayoutBase.Page)
                         : ParseHtmlFile(LayoutBase.BlogPost);

         layout = _translate.To(site.Language.Code, Project.Folders.Template, layout);

         var folder = contentType == ContentType.Page
                         ? site.Folders.Pages
                         : site.Folders.BlogPosts;

         _logger.LogInformation("Start to parsed {0}", type);

         foreach (var content in contents)
         {
            string post = _tag.Content(layout, content, site.DateTimeFormat);
            _logger.LogInformation("Content parsed: " + content.Title);
            _save.ToHtmlFile(folder + content.Filename, post);
         }

         _logger.LogInformation("Finish to parsed {0}", type);
      }

      private void ParseLoopLayouts(ProjectSite site, IEnumerable<Content> contentList)
      {
         //todo => this methods should be async
         var languages = Project.Sites.Select(x => x.Language);

         LoopLanguagesParsed = _loop.Languages(languages, Project.DefaultLanguageCode, LayoutBase.Loops.Languages);

         LoopSocialMediaParsed = _loop.SocialMedia(site.Author.SocialMedia, LayoutBase.Loops.SocialMedia);

         var menuList = contentList.Where(content => content.Type == ContentType.Page && content.Menu);
         LoopMenuParsed = _loop.Menu(menuList, LayoutBase.Loops.Menu);

         var posts = contentList.Where(content => content.Type == ContentType.Post);
         BlogPostsLast10Parsed = _loop.BlogPostsLastX(posts, LayoutBase.Loops.BlogPostLast10, 10);
         BlogPostsLast5Parsed = _loop.BlogPostsLastX(posts, LayoutBase.Loops.BlogPostLast5, 5);
         BlogPostsLast3Parsed = _loop.BlogPostsLastX(posts, LayoutBase.Loops.BlogPostLast3, 3);
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
