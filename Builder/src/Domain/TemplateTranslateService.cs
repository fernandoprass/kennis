using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Wrappers.Contracts;
using System.Text.Json;

namespace Builder.Domain {
   public interface ITemplateTranslateService
    {
        string To(string language, string templatePath, string template);
    }

    public class TemplateTranslateService : ITemplateTranslateService
    {
        private readonly IFileWrapper _file;
        private readonly ILoad _load;
        private readonly ILogger<BuilderService> _logger;

        public TemplateTranslateService(IFileWrapper file, ILoad load, ILogger<BuilderService> logger)
        {
            _file = file;
            _load = load;
            _logger = logger;
        }

        public string To(string language, string templatePath, string templateBase)
        {
            var i18nData = LoadI18nData(language, templatePath);

            return TranslateTemplate(templateBase, i18nData);
        }

        private static string TranslateTemplate(string template, Dictionary<string, string> i18nData)
        {
            if (string.IsNullOrEmpty(template))
            {
                return null;
            }

            var tralatedTemplate = template;

            foreach (var i18m in i18nData)
            {
                tralatedTemplate = tralatedTemplate.Replace($"{{:{i18m.Key}}}", i18m.Value);
            }

            return tralatedTemplate;
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
