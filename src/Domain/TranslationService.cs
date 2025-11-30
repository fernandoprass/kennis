using Kennis.Domain.Models;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Kennis.Domain
{
   public interface ITranslationService
   {
      Template Translate(Template template, string language);
   }

   public class TranslationService(ILoadService loadService, ILogService logService) : ITranslationService
   {
      private readonly ILoadService _loadService = loadService;
      private readonly ILogService _logService = logService;

      public Template Translate(Template template, string language)
      {
         if (!template.Languages.Contains(language))
         {
            _logService.LogError(LogCategory.Template, LogAction.NotSupported, language);
            return null;
         }

         var i18nData = _loadService.TemplateTranslationData(language);

         var translatedTemplate = new Template
         {
            Index = Translate(template.Index, i18nData),
            Blog = Translate(template.Blog, i18nData),
            BlogArchive = Translate(template.BlogArchive, i18nData),
            BlogCategories = Translate(template.BlogCategories, i18nData),
            BlogPost = Translate(template.BlogPost, i18nData),
            BlogTags = Translate(template.BlogTags, i18nData),
            Page = Translate(template.Page, i18nData),
            Assets = template.Assets,
            Loops = new TemplateLoop
            {
               BlogArchive = Translate(template.Loops.BlogArchive, i18nData),
               BlogCategories = Translate(template.Loops.BlogCategories, i18nData),
               BlogPostLast3 = Translate(template.Loops.BlogPostLast3, i18nData),
               BlogPostLast5 = Translate(template.Loops.BlogPostLast5, i18nData),
               BlogPostLast10 = Translate(template.Loops.BlogPostLast10, i18nData),
               BlogPosts = Translate(template.Loops.BlogPosts, i18nData),
               BlogTags = Translate(template.Loops.BlogTags, i18nData),
               Languages = Translate(template.Loops.Languages, i18nData),
               Menu = Translate(template.Loops.Menu, i18nData),
               SocialMedia = Translate(template.Loops.SocialMedia, i18nData)
            }
         };
         
         _logService.LogInfo(LogCategory.Template, LogAction.TranslateSuccess, language);

         return translatedTemplate;
      }

      private string Translate(string template, Dictionary<string, string> i18nData)
      {

         if (string.IsNullOrEmpty(template))
         {
            return null;
         }

         if (!i18nData.IsNullOrEmpty())
         {
            foreach (var i18m in i18nData)
            {
               template = template.Replace($"{{:{i18m.Key}}}", i18m.Value);
            }         
         }

         return template;
      }
   }
}
