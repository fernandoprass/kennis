using Builder.Domain;
using Builder.Domain.Configuration;
using Kennis.Builder.Constants;
using Myce.Extensions;

namespace Kennis.Builder.Domain
{
    public class Layout
   {
      private static string FilePath { get; set; }
      private Template Configuration { get; set; }
      public string Index { get; set; }
      public IEnumerable<LayoutPreprocessedTemplate> IndexPreprocessedTemplates { get; set; }
      public string Blog { get; set; }
      public string Post { get; set; }

      public LayoutLoop Loops;

      public Layout Get(string templateName)
      {
         FilePath = Path.Combine(AppContext.BaseDirectory, LocalEnvironment.Folder.Templates, templateName);

         var filename = Path.Combine(FilePath, LocalEnvironment.File.Template);
         var template = new Layout
         {
            Configuration = Template.Read(filename)
         };

         template.Index = LoadFromFiles(template.Configuration.Index, IndexPreprocessedTemplates);
         template.Loops = LoadLoopFromFiles(template.Configuration.Loops);

         return template;
      }

      private static LayoutLoop LoadLoopFromFiles(TemplateLoopHtmlFile loops)
      {
         if (loops.IsNotNull())
         {
            return new LayoutLoop
            {
               BlogArchive = LoadFromFile(loops.BlogArchive),
               BlogCategories = LoadFromFile(loops.BlogCategories),
               BlogPostLast10 = LoadFromFile(loops.BlogPostLast10),
               BlogPostLast5 = LoadFromFile(loops.BlogPostLast5),
               BlogPostLast3 = LoadFromFile(loops.BlogPostLast3),
               BlogPosts = LoadFromFile(loops.BlogPosts),
               BlogTags = LoadFromFile(loops.BlogTags),
               SocialMedia = LoadFromFile(loops.SocialMedia)
            };

            //todo implement menus
            //loops.Menus = LoadLoopFromFile(Configuration.Loops.);
         }
         return null;
      }

      private static string LoadFromFile(string filename)
      {
         if (!string.IsNullOrEmpty(filename))
         {
            filename = Path.Combine(FilePath, filename);
            return File.ReadAllText(filename);
         }

         return null;
      }

      private static string LoadFromFiles(IEnumerable<TemplateHtmlFile> files, 
         IEnumerable<LayoutPreprocessedTemplate> layoutPreprocessedTemplate)
      {
         if (files.IsNotNull())
         {
            var preprocessedTemplates = files.Any(x => x.ProcessOnlyOnce) ? new List<LayoutPreprocessedTemplate>() : null;
            string template = string.Empty;
            foreach (var file in files.OrderBy(x => x.Order))
            {
               var filename = Path.Combine(FilePath, file.FileName);

               var templatePart = LoadFromFile(filename);

               if (file.ProcessOnlyOnce)
               {
                  var preprocessed = new LayoutPreprocessedTemplate
                  {
                     Id = Guid.NewGuid(),
                     Template = templatePart
                  };

                  preprocessedTemplates.Add(preprocessed);

                  templatePart = string.Concat("{@", preprocessed.Id, "}");
               }
               
               template += templatePart;
            }
            return template;
         }

         return null;
      }
   }
}