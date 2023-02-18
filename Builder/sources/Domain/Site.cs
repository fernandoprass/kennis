using Builder.Domain.Mappers;
using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Builder.Domain
{
   public interface ISite
   {
      void Load(ProjectFolder projectFolders, string languageCode);

   }
   public class Site : ISite
   {
      private readonly ILoad _load;
      private readonly ILogger<Build> _logger;
      private string Language { get; set; }
      private string ContentBasePath { get; set; }
      private string ContentPagesPath { get; set; }
      private string ContentPostsPath { get; set; }
      private List<Content> Pages { get; set; }
      private List<Content> Posts { get; set; }

      public Site(ILoad load, ILogger<Build> logger)
      {
         _load = load;
         _logger = logger;
      }

      public void Load(ProjectFolder projectFolders, string languageCode)
      {
         InitializeContentPaths(projectFolders.Project, languageCode);

         var files = GetFiles();

         InitializeContentLists();

         foreach (var file in files)
         {
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

         DeleteNonExistentContent();

         SaveContentLists();
      }

      private void InitializeContentPaths(string projectPath, string languageCode)
      {
         ContentBasePath = Path.Combine(projectPath, languageCode);
         ContentPagesPath = Path.Combine(ContentBasePath, LocalEnvironment.Folder.Pages);
         ContentPostsPath = Path.Combine(ContentBasePath, LocalEnvironment.Folder.Posts);
      }

      private void DeleteNonExistentContent()
      {
         Pages.RemoveAll(x => x.Delete);
         Posts.RemoveAll(x => x.Delete);
      }

      private void SaveContentLists(string projectPath, string languageCode)
      {
         ContentBasePath = Path.Combine(projectPath, languageCode);
         ContentPagesPath = Path.Combine(ContentBasePath, LocalEnvironment.Folder.Pages);
         ContentPostsPath = Path.Combine(ContentBasePath, LocalEnvironment.Folder.Posts);
      }

      private (string, ContentType) GetFilenameAndContentType(string file)
      {
         var contentType = file.Contains(ContentPagesPath) ? ContentType.Page : ContentType.BlogPost;

         var filename = contentType == ContentType.Page
            ? file.Replace(ContentPagesPath, string.Empty)
            : file.Replace(ContentPostsPath, string.Empty);

         return (filename, contentType);
      }

      private void AddToContentList(ContentType contentType, string filename, ContentHeader contentHeader)
      {
         if (contentType == ContentType.Page)
         {
            AddToContentList(Pages, filename, contentHeader);
         }
         else
         {
            AddToContentList(Posts, filename, contentHeader);
         }
      }

      private void AddToContentList(List<Content> contentList, string filename, ContentHeader contentHeader)
      {
         var content = contentList.SingleOrDefault(x => x.Filename == filename);

         if (content.IsNull())
         {
            content = contentHeader.ToContent();
            content.Filename = filename;

            contentList.Add(content);
         }
         else
         {
            content.Merge(contentHeader);
            content.Delete = false;
         }
      }


      private void InitializeContentLists()
      {
         Pages = _load.ContentList(ContentBasePath, LocalEnvironment.File.Pages);
         Pages.ForEach(page => { page.Delete = true; });

         Posts = _load.ContentList(ContentBasePath, LocalEnvironment.File.Posts);
         Posts.ForEach(page => { page.Delete = true; });
      }

      private string[] GetFiles()
      {
         var criteria = "*" + LocalEnvironment.Extension.Content;
         var files = Directory.GetFiles(ContentBasePath, criteria, SearchOption.AllDirectories);
         return files;
      }

      private string GetSlug(string title)
      {
         var slug = title.ToLower().Replace(" ", "-");
         slug = slug.RemoveAccents();
         return slug;
      }
   }
}
