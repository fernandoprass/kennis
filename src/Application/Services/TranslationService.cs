using Kennis.Domain;
using Kennis.Domain.Interfaces;
using Kennis.Domain.Models;
using Myce.Extensions;

namespace Kennis.Application.Services;

public class TranslationService(
   ILoadService loadService, 
   ILogService logService) : ITranslationService
{
   private readonly ILoadService _loadService = loadService;
   private readonly ILogService _logService = logService;

   public async Task<Template> TranslateAsync(Template template, string language)
   {
      if (!template.Languages.Contains(language))
      {
         _logService.LogError(LogCategory.Template, LogAction.NotSupported, language);
         return null;
      }

      var i18nData = await _loadService.TemplateTranslationDataAsync(language);

      var translatedTemplate = new Template
      {
         Assets = template.Assets,
         Pages = new TemplatePages
         {
            Index = Translate(template.Pages.Index, i18nData),
            Blog = Translate(template.Pages.Blog, i18nData),
            BlogArchive = Translate(template.Pages.BlogArchive, i18nData),
            BlogCategories = Translate(template.Pages.BlogCategories, i18nData),
            BlogTags = Translate(template.Pages.BlogTags, i18nData),
            BlogPost = Translate(template.Pages.BlogPost, i18nData),
            Page = Translate(template.Pages.Page, i18nData), 
            Partials = new TemplatePagesPartials
            {
               Footer = Translate(template.Pages.Partials.Footer, i18nData),
               Header = Translate(template.Pages.Partials.Header, i18nData),
               Sidebar = Translate(template.Pages.Partials.Sidebar, i18nData)
            },
            Loops = new TemplatePagesLoops
            {
               BlogArchive = Translate(template.Pages.Loops.BlogArchive, i18nData),
               BlogCategories = Translate(template.Pages.Loops.BlogCategories, i18nData),
               BlogPosts = Translate(template.Pages.Loops.BlogPosts, i18nData),
               BlogTags = Translate(template.Pages.Loops.BlogTags, i18nData),
               Languages = Translate(template.Pages.Loops.Languages, i18nData),
               Menu = Translate(template.Pages.Loops.Menu, i18nData),
               SocialMedia = Translate(template.Pages.Loops.SocialMedia, i18nData)
            }
         },
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
