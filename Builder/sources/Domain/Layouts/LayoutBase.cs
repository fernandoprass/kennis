using Builder.Domain.Models;
using Builder.Domain.Wrappers;
using Kennis.Builder.Constants;
using Myce.Extensions;
using System.Text.Json;

namespace Builder.Domain.Layouts
{
    public interface ILayoutBase
   {
      string FilePath { get; }
      IEnumerable<string> Languages { get; set; }
      string Index { get; set; }
      string Page { get; set; }
      string Blog { get; set; }
      string BlogArchive { get; set; }
      string BlogCategories { get; set; }
      string BlogPost { get; set; }
      string BlogTags { get; set; }
      LayoutLoop Loops { get; set; }
      void Get(string templateName);
   }

   public class LayoutBase : ILayoutBase
   {
      private readonly IFileWrapper _file;
      public string FilePath { get; private set; }
      public IEnumerable<string> Languages { get; set; }
      public string Index { get; set; }
      public string Page { get; set; }
      public string Blog { get; set; }
      public string BlogArchive { get; set; }
      public string BlogCategories { get; set; }
      public string BlogPost { get; set; }
      public string BlogTags { get; set; }
      public LayoutLoop Loops { get; set; }

      public LayoutBase(IFileWrapper file)
      {
         _file = file;
      }

      public void Get(string templateName)
      {
         FilePath = Path.Combine(AppContext.BaseDirectory, LocalEnvironment.Folder.Templates, templateName);

         var templateFilename = Path.Combine(FilePath, LocalEnvironment.File.Template);

         var template = ReadTemplate(templateFilename);

         Languages = template.Languages;

         LoadMainTemplates(template);

         LoadLoopTemplates(template.Loops);
      }

      private Template ReadTemplate(string fileName)
      {
         if (_file.Exists(fileName))
         {
            string jsonString = _file.ReadAllText(fileName);
            var templateConfig = JsonSerializer.Deserialize<Template>(jsonString)!;
            return templateConfig;
         }

         return null;
      }

      private void LoadMainTemplates(Template template)
      {
         Index = LoadFromFile(template.Index);
         Page = LoadFromFile(template.Page);
         Blog = LoadFromFile(template.Blog);
         BlogArchive = LoadFromFile(template.BlogArchive);
         BlogCategories = LoadFromFile(template.BlogCategories);
         BlogPost = LoadFromFile(template.BlogPost);
         BlogTags = LoadFromFile(template.BlogTags);
      }

      private void LoadLoopTemplates(TemplateLoop loops)
      {
         if (loops.IsNotNull())
         {
            Loops = new LayoutLoop
            {
               BlogArchive = LoadFromFile(loops.BlogArchive),
               BlogCategories = LoadFromFile(loops.BlogCategories),
               BlogPostLast10 = LoadFromFile(loops.BlogPostLast10),
               BlogPostLast5 = LoadFromFile(loops.BlogPostLast5),
               BlogPostLast3 = LoadFromFile(loops.BlogPostLast3),
               BlogPosts = LoadFromFile(loops.BlogPosts),
               BlogTags = LoadFromFile(loops.BlogTags),
               Menu = LoadFromFile(loops.Menu),
               SocialMedia = LoadFromFile(loops.SocialMedia)
            };
         }
      }

      private string LoadFromFile(string filename)
      {
         if (!string.IsNullOrEmpty(filename))
         {
            filename = Path.Combine(FilePath, filename);
            return _file.ReadAllText(filename);
         }

         return null;
      }
   }
}