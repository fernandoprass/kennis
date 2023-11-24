using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;

namespace Builder.Domain
{
    public interface IBuildSiteService
   {
      void Build(string defaultLanguage, ProjectFolder projectFolder, ProjectSite projectSite);
   }

   public class BuilderSiteService : IBuildSiteService
   {
      private readonly ILogger<BuilderService> _logger;
      private readonly IData _data;
      private readonly ISave _save;
      private readonly IBuildLoop _loop;
      private readonly IBuildTag _tag;

      private ProjectFolder ProjectFolder { get; set; }
      private Layout Layout { get; set; }
      private string DefaultLanguageCode { get; set; }
      private string LoopLanguagesParsed { get; set; }
      private string LoopSocialMediaParsed { get; set; }
      private string LoopMenuParsed { get; set; }
      private string BlogPostsLast10Parsed { get; set; }
      private string BlogPostsLast5Parsed { get; set; }
      private string BlogPostsLast3Parsed { get; set; }

      public BuilderSiteService(
         ILogger<BuilderService> logger,
         IData data,
         ISave save,
         IBuildLoop loop,
         IBuildTag tag)
      {
         _logger = logger;
         _data = data;
         _save = save;
         _loop = loop;
         _tag = tag;
      }

      public void Build(string defaultLanguageCode, ProjectFolder projectFolder, ProjectSite projectSite)
      {
         DefaultLanguageCode = defaultLanguageCode;
         ProjectFolder = projectFolder;
         _save.Configure(projectFolder.Destination, Path.Combine(projectFolder.Project, projectSite.Language.Code));
         _data.GetContentList(projectFolder, projectSite.Language.Code, projectSite.Folders.Pages, projectSite.Folders.BlogPosts);
         _data.UpdateContentList();

         var lastModified = _data.ContentList.Max(x => x.Updated.HasValue ? x.Updated.Value : x.Created);

         _data.UpdateProjectSiteModified(lastModified, projectSite);

         _data.SaveContentList();

         ParseLoopLayouts(projectSite, _data.ContentList);
         ParseIndexFile(projectSite);

         ParseBlogIndexFile(projectSite);
         ParseContentFile(projectSite, ContentType.Page, _data.ContentList);

         ParseContentFile(projectSite, ContentType.Post, _data.ContentList);
      }

      private void ParseIndexFile(ProjectSite site)
      {
         string layout = ParseHtmlFile(Layout.Index);

         //layout = _translate.To(site.Language.Code, ProjectFolder.Template, layout);

         layout = _tag.Index(layout, site);

         _logger.LogInformation("Index html page parsed - " + site.Language.Label);

         _save.ToHtmlFile(site.Language.IndexFileName, layout);
      }

      private void ParseBlogIndexFile(ProjectSite site)
      {
         string layout = ParseHtmlFile(Layout.Blog);

         //layout = _translate.To(site.Language.Code, ProjectFolder.Template, layout);

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
                         ? ParseHtmlFile(Layout.Page)
                         : ParseHtmlFile(Layout.BlogPost);

         //layout = _translate.To(site.Language.Code, ProjectFolder.Template, layout);

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

         //todo two options here: 1 add Project as a parameter; 2 move loop languages out side of build site
         var languages = new List<Language>
         {
            site.Language
         };

         LoopLanguagesParsed = _loop.Languages(languages, DefaultLanguageCode, Layout.Loops.Languages);

         LoopSocialMediaParsed = _loop.SocialMedia(site.Author.SocialMedia, Layout.Loops.SocialMedia);

         var menuList = contentList.Where(content => content.Type == ContentType.Page && content.Menu);
         LoopMenuParsed = _loop.Menu(menuList, Layout.Loops.Menu);

         var posts = contentList.Where(content => content.Type == ContentType.Post);
         BlogPostsLast10Parsed = _loop.BlogPostsLastX(posts, Layout.Loops.BlogPostLast10, 10);
         BlogPostsLast5Parsed = _loop.BlogPostsLastX(posts, Layout.Loops.BlogPostLast5, 5);
         BlogPostsLast3Parsed = _loop.BlogPostsLastX(posts, Layout.Loops.BlogPostLast3, 3);
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
