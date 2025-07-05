using Kennis.Builder.Constants;
using Kennis.Domain.Mappers;
using Kennis.Domain.Models;
using Myce.Extensions;

namespace Kennis.Domain
{
   public interface IDataService
   {
      List<Content> ContentList { get; set; }

      void GetContentList(string projectFolder, string languageCode, string htmlPagePath, string htmlPostPath);

      void SaveContentList(string languageCode);

      void UpdateContentListData();

      void UpdateProjectSiteModified(DateTime lastModified, ProjectSite projectSite);
   }

   public class DataService(ILoadService loadService,
                            ISaveService saveService,
                            ILogService logService) : IDataService
   {
      private readonly ILoadService _loadService = loadService;
      private readonly ISaveService _saveService = saveService;
      private readonly ILogService _logService = logService;

      private string ContentBasePath { get; set; }
      private string ContentPagesPath { get; set; }
      private string ContentPostsPath { get; set; }
      private string HtmlPagesPath { get; set; }
      private string HtmlPostsPath { get; set; }
      public List<Content> ContentList { get; set; }


      #region Public methods
      public void GetContentList(string projectFolder, string languageCode, string htmlPagePath, string htmlPostPath)
      {
         InitializeContentPaths(projectFolder, languageCode, htmlPagePath, htmlPostPath);

         var contentFileList = _loadService.ContentFileList(ContentBasePath);

         InitializeContentList();

         foreach (var file in contentFileList)
         {
            _logService.LogInfo(LogCategory.Content, LogAction.LoadStarting, file);

            string yaml = _loadService.YamlContentHeader(file);

            if (yaml.IsNotNull())
            {
               var header = _loadService.ContentHeader(yaml);

               //Draft contents should not be added
               if (header.IsNotNull() && !header.Draft)
               {
                  var contentType = GetContentType(file);
                  var filename = Path.GetFileName(file);

                  AddToContentList(contentType, filename, header);
               }

               if (header.IsNull())
               {
                  _logService.LogError(LogCategory.Content, LogAction.ContentNotFound, file);
               }
            }
         }

         SortContentList();
      }

      public void SaveContentList(string languageCode)
      {
         _saveService.ToJsonFile(languageCode, Const.File.ContentList, ContentList);
      }

      public void UpdateContentListData()
      {
         foreach (var content in ContentList.Where(x => x.Published.IsNull() || x.Published < x.Updated))
         {
            var category = content.Categories?.FirstOrDefault();
            content.Keywords = GetKeywords(content.Categories, content.Tags);
            content.Url = GetUrl(content.Type, content.Title, category, content.Created.Year);
         }
      }

      public void UpdateProjectSiteModified(DateTime lastModified, ProjectSite projectSite)
      {
         projectSite.Modified = lastModified;
      }

      #endregion

      #region Private methods
      private void InitializeContentPaths(string projectPath, string languageCode, string htmlPagePath, string htmlPostPath)
      {
         ContentBasePath = Path.Combine(projectPath, languageCode);
         ContentPagesPath = Path.Combine(ContentBasePath, Const.Folder.Pages);
         ContentPostsPath = Path.Combine(ContentBasePath, Const.Folder.Posts);
         HtmlPagesPath = htmlPagePath;
         HtmlPostsPath = htmlPostPath;
      }

      private string GetKeywords(IEnumerable<string> categories, IEnumerable<string> tags)
      {
         var keywords = categories.HasData() ? categories : new List<string>();

         if (tags.HasData())
         {
            keywords = keywords.Union(tags);
         }

         return string.Join(",", keywords);
      }

      private string GetSlug(string title)
      {
         if (title.IsNull())
         {
            return string.Empty;
         }
         var slug = title.ToLower().Replace(" ", "-");
         slug = slug.RemoveAccents();
         return slug;
      }

      private string GetUrl(ContentType type, string title, string category, int year)
      {
         var slug = GetSlug(title);
         var baseUrl = type == ContentType.Page ? HtmlPagesPath : HtmlPostsPath;
         baseUrl = baseUrl.Replace("{category}", GetSlug(category));
         baseUrl = baseUrl.Replace("{year}", year.ToString());
         return baseUrl + slug + Const.Extension.WebPages;
      }

      private void SortContentList()
      {
         ContentList.OrderBy(x => x.Type).OrderBy(x => x.Created);
      }

      private ContentType GetContentType(string file)
      {
         return file.Contains(ContentPagesPath) ? ContentType.Page : ContentType.Post;
      }

      private void AddToContentList(ContentType contentType, string filename, ContentHeader contentHeader)
      {
         var content = ContentList.SingleOrDefault(x => x.Filename == filename && x.Type == contentType);

         if (content.IsNull())
         {
            content = contentHeader.ToContent();
            content.Type = contentType;
            content.Filename = filename;

            ContentList.Add(content);
         }
         else
         {
            content.Merge(contentHeader);
            content.Delete = false;
         }
      }

      private void InitializeContentList()
      {
         ContentList = _loadService.ContentList(ContentBasePath);
         ContentList.ForEach(page => { page.Delete = true; });
      }
      #endregion
   }
}