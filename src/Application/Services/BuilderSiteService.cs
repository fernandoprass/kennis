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

   public async Task BuildAsync(string defaultLanguageCode, string projectFolder, ProjectSite projectSite, Template template)
   {
      DefaultLanguageCode = defaultLanguageCode;
      Template = template;

      await GetContentListAsync(projectFolder, projectSite);

      ParseLoopLayouts(projectSite, _dataService.ContentList);
      await ParseIndexFileAsync(projectSite);

      await ParseBlogIndexFileAsync(projectSite);
      await ParseContentFileAsync(projectSite, ContentType.Page, _dataService.ContentList);

      await ParseContentFileAsync(projectSite, ContentType.Post, _dataService.ContentList);

      projectSite.LastSuccessfulCreation = DateTime.UtcNow;
   }

   private async Task GetContentListAsync(string projectFolder, ProjectSite projectSite)
   {
      await _dataService.GetContentListAsync(projectFolder, projectSite.Language.Code, projectSite.Folders.Pages, projectSite.Folders.BlogPosts);
      _dataService.UpdateContentListData();

      var lastModified = _dataService.ContentList.Max(x => x.Updated.HasValue ? x.Updated.Value : x.Created);

      _dataService.UpdateProjectSiteModified(lastModified, projectSite);

      await _dataService.SaveContentListAsync(projectSite.Language.Code);
   }

   private async Task ParseIndexFileAsync(ProjectSite site)
   {
      string template = ParseHtmlFile(Template.Pages.Index);

      template = _tag.Index(template, site);

      _logService.LogInfo(LogCategory.HtmlFile, LogAction.ParseSuccess, "index");

      await _saveService.ToHtmlFileAsync(site.Language.IndexFileName, template);
   }

   private async Task ParseBlogIndexFileAsync(ProjectSite site)
   {
      string template = ParseHtmlFile(Template.Pages.Blog);

      template = _tag.Index(template, site);

      _logService.LogInfo(LogCategory.HtmlFile, LogAction.ParseSuccess, "blog index");

      await _saveService.ToHtmlFileAsync(site.Folders.Blog + Const.File.Index, template);
   }

   private async Task ParseContentFileAsync(ProjectSite site, ContentType contentType, IEnumerable<Content> contentList)
   {
      var type = contentType == ContentType.Page ? "pages" : "posts";

      var contents = contentType == ContentType.Page
                     ? contentList.Where(x => x.Type.Equals(ContentType.Page))
                     : contentList.Where(x => x.Type.Equals(ContentType.Post));

      string template = contentType == ContentType.Page
                      ? ParseHtmlFile(Template.Pages.Page)
                      : ParseHtmlFile(Template.Pages.BlogPost);

      var folder = contentType == ContentType.Page
                      ? site.Folders.Pages
                      : site.Folders.BlogPosts;

      _logService.LogInfo(LogCategory.Content, LogAction.ParseStart, type);

      var tasks = contents.Select(async content =>
      {
         string post = _tag.Content(template, content, site.DateTimeFormat);
         _logService.LogInfo(LogCategory.Content, LogAction.ParseSuccess, content.Title);
         await _saveService.ToHtmlFileAsync(folder + content.Filename, post);
      });

      await Task.WhenAll(tasks);

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

      LoopLanguagesParsed = _loop.Languages(languages, DefaultLanguageCode, Template.Pages.Loops.Languages);

      LoopSocialMediaParsed = _loop.SocialMedia(site.Author.SocialMedia, Template.Pages.Loops.SocialMedia);

      var menuList = contentList.Where(content => content.Type == ContentType.Page && content.Menu);
      LoopMenuParsed = _loop.Menu(menuList, Template.Pages.Loops.Menu);

      var posts = contentList.Where(content => content.Type == ContentType.Post);
      BlogPostsLast10Parsed = _loop.BlogPostsLastX(posts, Template.Pages.Loops.BlogPosts, 10);
      BlogPostsLast5Parsed = _loop.BlogPostsLastX(posts, Template.Pages.Loops.BlogPosts, 5);
      BlogPostsLast3Parsed = _loop.BlogPostsLastX(posts, Template.Pages.Loops.BlogPosts, 3);
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
