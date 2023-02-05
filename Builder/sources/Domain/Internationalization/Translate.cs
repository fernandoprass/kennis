using Builder.Domain.Layouts;
using Builder.Domain.Wrappers;
using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Extensions;
using System.Text.Json;

namespace Builder.Domain.Internationalization
{
   public interface ITranslate
   {
      Layout To(string language, ILayoutBase layoutBase);
   }
   public class Translate : ITranslate
   {
      private readonly IFileWrapper _file;
      private readonly ILogger<Build> _logger;

      public Translate(IFileWrapper file, ILogger<Build> logger)
      {
         _file = file;
         _logger = logger;
      }

      public Layout To(string language, ILayoutBase layoutBase)
      {
         var i18nData = LoadI18nData(language, layoutBase.FilePath);

         var layout = new Layout();

         layout.Index = TranslateLayoutTemplate(layoutBase.Index, i18nData);
         layout.Blog = TranslateLayoutTemplate(layoutBase.Blog, i18nData);
         layout.Post = TranslateLayoutTemplate(layoutBase.Post, i18nData);
         layout.Page = TranslateLayoutTemplate(layoutBase.Page, i18nData);

         return layout;
      }

      private static LayoutTemplate TranslateLayoutTemplate(LayoutTemplate layoutTemplate, Dictionary<string, string> i18nData)
      {
         if (layoutTemplate.IsNull())
         {
            return null;
         }

         var layout = new LayoutTemplate();
         layout.Template = layoutTemplate.Template;
         foreach (var i18m in i18nData)
         {
            layout.Template = layout.Template.Replace($"{{:{i18m.Key}}}", i18m.Value);
         }

         if (layoutTemplate.TemplatesPreprocessed.HasData())
         {
            foreach (var templatePreprocessed in layoutTemplate.TemplatesPreprocessed)
            {
               foreach (var i18m in i18nData)
               {
                  templatePreprocessed.Template = templatePreprocessed.Template.Replace($"{{:{i18m.Key}}}", i18m.Value);
               }
            }
         }

         return layout;
      }

      private Dictionary<string, string> LoadI18nData(string language, string templatePath)
      {
         var filename = Path.Combine(templatePath, LocalEnvironment.Folder.I18n, language + LocalEnvironment.Extensions.I18n);

         if (_file.Exists(filename))
         {
            string jsonString = _file.ReadAllText(filename);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString)!;
         }

         return null;
      }
   }
}
