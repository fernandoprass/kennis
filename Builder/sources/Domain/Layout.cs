using Builder.Domain;
using Builder.Domain.Configuration;
using Builder.Domain.Wrappers;
using Kennis.Builder.Constants;
using Myce.Extensions;
using System.Text.Json;

namespace Kennis.Builder.Domain
{
   public interface ILayout
   {
      IEnumerable<string> Languages { get; set; }
      LayoutTemplate Index { get; set; }
      LayoutTemplate Blog { get; set; }
      LayoutTemplate Post { get; set; }
      LayoutLoop Loops { get; set; }
      void Get(string templateName);
   }

   public class Layout : ILayout
   {
      private readonly IFileWrapper _file;

      private static string FilePath { get; set; }
      public IEnumerable<string> Languages { get; set; }
      public LayoutTemplate Index { get; set; }
      public LayoutTemplate Blog { get; set; }
      public LayoutTemplate Post { get; set; }
      public LayoutLoop Loops { get; set; }

      public Layout(IFileWrapper file)
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
         if (template.Index.HasData())
         {
            Index = new LayoutTemplate();
            LoadFromFiles(template.Index, Index);
         };

         if (template.Blog.HasData())
         {
            Blog = new LayoutTemplate();
            LoadFromFiles(template.Blog, Blog);
         };

         if (template.Post.HasData())
         {
            Post = new LayoutTemplate();
            LoadFromFiles(template.Post, Post);
         };
      }

      private void LoadFromFiles(IEnumerable<TemplateHtmlFile> files, LayoutTemplate layoutTemplate)
      {
         if (files.IsNotNull())
         {
            layoutTemplate.TemplatesPreprocessed = files.Any(x => x.ProcessOnlyOnce) ? new List<LayoutTemplatePreprocessed>() : null;

            foreach (var file in files.OrderBy(x => x.Order))
            {
               var filename = Path.Combine(FilePath, file.FileName);

               var templatePart = LoadFromFile(filename);

               if (file.ProcessOnlyOnce)
               {
                  var preprocessed = new LayoutTemplatePreprocessed
                  {
                     Id = Guid.NewGuid(),
                     Template = templatePart
                  };

                  layoutTemplate.TemplatesPreprocessed.Add(preprocessed);

                  templatePart = string.Concat("{@", preprocessed.Id, "}");
               }

               layoutTemplate.Template += templatePart;
            }
         }
      }

      private void LoadLoopTemplates(TemplateLoopHtmlFile loops)
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