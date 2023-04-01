using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Wrappers.Contracts;
using System.Text.Json;

namespace Builder.Domain.Internationalization
{
   public interface ITranslate
   {
      string To(string language, string templatePath, string layout);
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

      public string To(string language, string templatePath, string layoutBase)
      {
          var i18nData = LoadI18nData(language, templatePath);

         return TranslateLayoutTemplate(layoutBase, i18nData);
      }

      private string TranslateLayoutTemplate(string layoutTemplate, Dictionary<string, string> i18nData)
      {
         if (string.IsNullOrEmpty(layoutTemplate))
         {
            return null;
         }

         var layout = layoutTemplate;

         foreach (var i18m in i18nData)
         {
            layout = layout.Replace($"{{:{i18m.Key}}}", i18m.Value);
         }

         return layout;
      }

      private Dictionary<string, string> LoadI18nData(string language, string templatePath)
      {
         _logger.LogInformation($"Loading i18n data for '{language}' on {templatePath}");
         var filename = Path.Combine(templatePath, Const.Folder.TemplatesTranslations, language + Const.Extension.I18n);

         if (_file.Exists(filename))
         {
            string jsonString = _file.ReadAllText(filename);
            var i18nData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString)!;
            _logger.LogInformation($"I18n data for '{language}' loaded successfully");
            return i18nData;
         }

         _logger.LogWarning($"I18n data for '{language}' not found");

         return null;
      }
   }
}
