using Kennis.Domain.Models;

namespace Kennis.Domain
{
   public interface ITemplateService
   {
      Template Load(string name, string projectDefaultLanguage);
   }

   public class TemplateService(ILogService logService, ILoadService loadService) : ITemplateService
   {
      private readonly ILogService _logService = logService;
      private readonly ILoadService _loadService = loadService;

      public Template Load(string name, string projectDefaultLanguage)
      {
         var template = _loadService.Template(name);

         template.DefaultLanguage = template.Languages.Contains(projectDefaultLanguage) ? projectDefaultLanguage : template.DefaultLanguage;

         if (!template.Languages.Contains(projectDefaultLanguage))
         {
            _logService.LogWarning(LogCategory.Template, LogAction.LanguageNotSupported, projectDefaultLanguage, template.DefaultLanguage);
         }

         return template;
      }
   }
}
