using Kennis.Domain;
using Kennis.Domain.Models;
using Kennis.Domain.Interfaces;

namespace Kennis.Application.Services;

public class BuilderSiteService(ILogService logService,
                                IDataService dataService,
                                ISaveService saveService,
                                IBuildLoop loop,
                                IBuildTag tag) : IBuildSiteService
{
   private readonly ILogService _logService = logService;
   private readonly IDataService _dataService = dataService;
   private readonly ISaveService _saveService = saveService;
   private readonly IBuildLoop _loop = loop;
   private readonly IBuildTag _tag = tag;

   private Template Template { get; set; }
   private string DefaultLanguageCode { get; set; } = string.Empty;
   private string LoopLanguagesParsed { get; set; } = string.Empty;
   private string LoopSocialMediaParsed { get; set; } = string.Empty;
   private string LoopMenuParsed { get; set; } = string.Empty;
   private string BlogPostsLast10Parsed { get; set; } = string.Empty;
   private string BlogPostsLast5Parsed { get; set; } = string.Empty;
   private string BlogPostsLast3Parsed { get; set; } = string.Empty;

   public void Build(string defaultLanguageCode, string projectFolder, ProjectSite projectSite, Template template)
   {
      DefaultLanguageCode = defaultLanguageCode;
      Template = template;

      GetContentList(projectFolder, projectSite);

      ParseLoopLayouts(projectSite, _dataService.ContentList);
      ParseIndexFile(projectSite);

      ParseBlogIndexFile(projectSite);
      ParseContentFile(projectSite, ContentType.Page, _dataService.ContentList);

      ParseContentFile(projectSite, ContentType.Post, _dataService.ContentList);

      projectSite.LastSuccessfulCreation = DateTime.UtcNow;
   }

   private void GetContentList(string projectFolder, ProjectSite projectSite)
   {
      _dataService.GetContentList(projectFolder, projectSite.Language.Code, projectSite.Folders.Pages, projectSite.Folders.BlogPosts);
      _dataService.UpdateContentListData();

      var lastModified = _dataService.ContentList.Max(x => x.Updated.HasValue ? x.Updated.Value : x.Created);

      _dataService.UpdateProjectSiteModified(lastModified, projectSite);

      _dataService.SaveContentList(projectSite.Language.Code);
   }

   private void ParseIndexFile(ProjectSite site)
   {
      string template = ParseHtmlFile(Template.Index);

      template = _tag.Index(template, site);

      _logService.LogInfo(LogCategory.HtmlFile, LogAction.ParseSuccess,"index");

      _saveService.ToHtmlFile(site.Language.IndexFileName, template);
   }

   private void ParseBlogIndexFile(ProjectSite site)
   {
      string template = ParseHtmlFile(Template.Blog);

      template = _tag.Index(template, site);

      _logService.LogInfo(LogCategory.HtmlFile, LogAction.ParseSuccess, "blog index");

      _saveService.ToHtmlFile(site.Folders.Blog + Const.File.Index, template);
   }

   private void ParseContentFile(ProjectSite site, ContentType contentType, IEnumerable<Content> contentList)
   {
      var type = contentType == ContentType.Page ? "pages" : "posts";

      var contents = contentType == ContentType.Page
                     ? contentList.Where(x => x.Type.Equals(ContentType.Page))
                     : contentList.Where(x => x.Type.Equals(ContentType.Post));

      string template = contentType == ContentType.Page
                      ? ParseHtmlFile(Template.Page)
                      : ParseHtmlFile(Template.BlogPost);

      //template = _translate.To(site.Language.Code, ProjectFolder.Template, template);

      var folder = contentType == ContentType.Page
                      ? site.Folders.Pages
                      : site.Folders.BlogPosts;

      _logService.LogInfo(LogCategory.Content, LogAction.ParseStart, type);

      foreach (var content in contents)
      {
         string post = _tag.Content(template, content, site.DateTimeFormat);
         _logService.LogInfo(LogCategory.Content, LogAction.ParseSuccess, content.Title);
         _saveService.ToHtmlFile(folder + content.Filename, post);
      }

      _logService.LogInfo(LogCategory.Content, LogAction.ParseSuccess, type);
   }

   private void ParseLoopLayouts(ProjectSite site, IEnumerable<Content> contentList)
   {
      //todo => this methods should be async

      //todo two options here: 1 add Project as a parameter; 2 move loop languages out side of build site
      var languages = new List<Language>
      {
         site.Language
      };

      LoopLanguagesParsed = _loop.Languages(languages, DefaultLanguageCode, Template.Loops.Languages);

      LoopSocialMediaParsed = _loop.SocialMedia(site.Author.SocialMedia, Template.Loops.SocialMedia);

      var menuList = contentList.Where(content => content.Type == ContentType.Page && content.Menu);
      LoopMenuParsed = _loop.Menu(menuList, Template.Loops.Menu);

      var posts = contentList.Where(content => content.Type == ContentType.Post);
      BlogPostsLast10Parsed = _loop.BlogPostsLastX(posts, Template.Loops.BlogPostLast10, 10);
      BlogPostsLast5Parsed = _loop.BlogPostsLastX(posts, Template.Loops.BlogPostLast5, 5);
      BlogPostsLast3Parsed = _loop.BlogPostsLastX(posts, Template.Loops.BlogPostLast3, 3);
   }

   private string ParseHtmlFile(string template)
   {
      template = template.Replace(Const.Tag.Site.Loop.Languages, LoopLanguagesParsed);
      template = template.Replace(Const.Tag.Site.Loop.Menu, LoopMenuParsed);
      template = template.Replace(Const.Tag.Site.Loop.SocialMedia, LoopSocialMediaParsed);
      template = template.Replace(Const.Tag.Site.Loop.BlogPostLast10, BlogPostsLast10Parsed);
      template = template.Replace(Const.Tag.Site.Loop.BlogPostLast5, BlogPostsLast5Parsed);
      template = template.Replace(Const.Tag.Site.Loop.BlogPostLast3, BlogPostsLast3Parsed);

      return template;
   }

}
