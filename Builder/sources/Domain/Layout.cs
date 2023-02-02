using Builder.Domain;
using Builder.Domain.Configuration;
using Builder.Domain.Wrappers;
using Kennis.Builder.Constants;
using Myce.Extensions;

namespace Kennis.Builder.Domain
{
   public interface ILayout {
      void Get(string templateName);
   }

   public class Layout : ILayout
   {
      private readonly IFileWrapper _file;
      private static string FilePath { get; set; }
      public LayoutTemplate Index { get; set; }
      public LayoutTemplate Blog { get; set; }
      public LayoutTemplate Post { get; set; }

      public LayoutLoop Loops;

      public Layout(IFileWrapper file)
      {
         _file= file;
      }

      public void Get(string templateName)
      {
         FilePath = Path.Combine(AppContext.BaseDirectory, LocalEnvironment.Folder.Templates, templateName);

         var filename = Path.Combine(FilePath, LocalEnvironment.File.Template);

         var template = Template.Read(filename);

         LoadMainTemplates(template);

         LoadLoopTemplates(template.Loops);
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
            Index = new LayoutTemplate();
            LoadFromFiles(template.Blog, Blog);
         };

         if (template.Post.HasData())
         {
            Index = new LayoutTemplate();
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