using Builder.Domain.Models;
using Myce.Extensions;

namespace Builder.Domain {
   public interface ITemplateService {
      void Load(string defaultLanguage);
      bool IsTemplateLoaded();
      Template TranslateTo(string language);

   }
   public class TemplateService : ITemplateService {
      private readonly ILoadService _loadService;
      private Template? _template;
      private string _defaultLanguage;

      public TemplateService(ILoadService loadService)
      {
         _loadService = loadService;
      }

      public void Load(string defaultLanguage)
      {
         _template = _loadService.Template();
         SetDefaultLanguage(defaultLanguage);
      }

      private void SetDefaultLanguage(string defaultLanguage)
      {
         //if defaultLanguage is not a language suported by the template, so the defaultLanguage is the first language of the template
         _defaultLanguage = _template.Languages.Contains(defaultLanguage) ? defaultLanguage : _template.Languages.First();
      }

      public Template TranslateTo(string language)
      {
         var i18nData = LoadI18Data(language);

         var translatedTemplate = new Template
         {
            Index = Translate(_template.Index, i18nData),
            Blog = Translate(_template.Blog, i18nData),
            BlogArchive = Translate(_template.BlogArchive, i18nData),
            BlogCategories = Translate(_template.BlogCategories, i18nData),
            BlogPost = Translate(_template.BlogPost, i18nData),
            BlogTags = Translate(_template.BlogTags, i18nData),
            Page = Translate(_template.Page, i18nData),
         };

         translatedTemplate.Loops = new TemplateLoop
         {
            BlogArchive = Translate(_template.Loops.BlogArchive, i18nData),
            BlogCategories = Translate(_template.Loops.BlogCategories, i18nData),
            BlogPostLast3 = Translate(_template.Loops.BlogPostLast3, i18nData),
            BlogPostLast5 = Translate(_template.Loops.BlogPostLast5, i18nData),
            BlogPostLast10 = Translate(_template.Loops.BlogPostLast10, i18nData),
            BlogPosts = Translate(_template.Loops.BlogPosts, i18nData),
            BlogTags = Translate(_template.Loops.BlogTags, i18nData),
            Languages = Translate(_template.Loops.Languages, i18nData),
            Menu = Translate(_template.Loops.Menu, i18nData),
            SocialMedia = Translate(_template.Loops.SocialMedia, i18nData)
         };

         return translatedTemplate;
      }

      private Dictionary<string, string> LoadI18Data(string language)
      {
         if (!_template.Languages.Contains(language))
         {
            language = _defaultLanguage;
         }
         
         return _loadService.TemplateTranslationData(language);
      }

      private static string Translate(string template, Dictionary<string, string> i18nData)
      {

         if (string.IsNullOrEmpty(template))
         {
            return null;
         }

         foreach (var i18m in i18nData)
         {
            template = template.Replace($"{{:{i18m.Key}}}", i18m.Value);
         }

         return template;
      }

      public bool IsTemplateLoaded()
      {
         return _template.IsNotNull();
      }
   }
}
