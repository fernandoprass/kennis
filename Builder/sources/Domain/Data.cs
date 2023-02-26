using Builder.Domain.Mappers;
using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Builder.Domain
{
   public interface IData
   {
      List<Content> GetContentList(ProjectFolder projectFolders, string languageCode, string htmlPagePath, string htmlPostPath);

   }
   public class Data : IData
   {
      private readonly ILoad _load;
      private readonly ISave _save;
      private readonly ILogger<Build> _logger;
      private string ContentBasePath { get; set; }
      private string ContentPagesPath { get; set; }
      private string ContentPostsPath { get; set; }
      private string HtmlPagesPath { get; set; }
      private string HtmlPostsPath { get; set; }
      private List<Content> ContentList { get; set; }

      public Data(ILoad load, ISave save, ILogger<Build> logger)
      {
         _load = load;
         _save =  save;
         _logger = logger;
      }

      public List<Content> GetContentList(ProjectFolder projectFolders, string languageCode, string htmlPagePath, string htmlPostPath)
      {
         InitializeContentPaths(projectFolders.Project, languageCode, htmlPagePath, htmlPostPath);

         var files = GetFiles();

         InitializeContentList();

         foreach (var file in files)
         {
            _logger.LogInformation("Reading {0}", file);

            string yaml = _load.YamlHeader(file);

            if (yaml.IsNotNull())
            {
               var header = _load.ContentHeader(yaml);

               if (header.IsNotNull())
               {
                  var (filename, contentType) = GetFilenameAndContentType(file);

                  AddToContentList(contentType, filename, header);
               }
            }
         }

         UpdateContentField();

         SortContentList();

         _save.ContentListToJson(ContentList, ContentBasePath);

         return ContentList;
      }

      private void InitializeContentPaths(string projectPath, string languageCode, string htmlPagePath, string htmlPostPath)
      {
         ContentBasePath = Path.Combine(projectPath, languageCode);
         ContentPagesPath = Path.Combine(ContentBasePath, Const.Folder.Pages);
         ContentPostsPath = Path.Combine(ContentBasePath, Const.Folder.Posts);
         HtmlPagesPath = htmlPagePath;
         HtmlPostsPath = htmlPostPath;
      }

      private void UpdateContentField()
      {
         foreach(var content in ContentList.Where(x => x.Published.IsNull() || x.Published < x.Updated))
         {
            content.Keywords = GetKeywords(content.Categories, content.Tags);
            content.Url = GetUrl(content.Type, content.Title, content.Categories.FirstOrDefault(), content.Created.Year);
         }
      }

      private string GetKeywords(IEnumerable<string> categories, IEnumerable<string> tags)
      {
         var keywords = categories.HasData() ? categories : new List<string>();

         if (tags.HasData()) { 
            keywords = keywords.Union(tags);
         }
         
         return string.Join(",", keywords);
      }

      private string GetSlug(string title)
      {
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
         return baseUrl + slug + ".html";
      }

      private void SortContentList()
      {
         ContentList.OrderBy(x => x.Type).OrderBy(x => x.Created);
      }

      private (string, ContentType) GetFilenameAndContentType(string file)
      {
         var contentType = file.Contains(ContentPagesPath) ? ContentType.Page : ContentType.Post;

         var filename = contentType == ContentType.Page
            ? file.Replace(ContentPagesPath, string.Empty)
            : file.Replace(ContentPostsPath, string.Empty);

         return (filename, contentType);
      }

      private void AddToContentList(ContentType contentType, string filename, ContentHeader contentHeader)
      {
         var content = ContentList.SingleOrDefault(x => x.Filename == filename && x.Type == contentType);

         if (content.IsNull())
         {
            content = contentHeader.ToContent();
            content.Type= contentType;
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
         ContentList = _load.ContentList(ContentBasePath);
         ContentList.ForEach(page => { page.Delete = true; });
      }

      private string[] GetFiles()
      {
         var criteria = "*" + Const.Extension.Content;
         var files = Directory.GetFiles(ContentBasePath, criteria, SearchOption.AllDirectories);
         return files;
      }
   }
}
