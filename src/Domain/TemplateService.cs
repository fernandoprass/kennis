using Kennis.Domain.Models;

namespace Kennis.Domain
{
   public interface ITemplateService
   {
      Template Load(string name, string projectDefaultLanguage);
   }

   public class TemplateService : ITemplateService
   {
      private readonly ILoadService _loadService;

      public TemplateService(ILoadService loadService)
      {
         _loadService = loadService;
      }

      public Template Load(string name, string projectDefaultLanguage)
      {
         var template = _loadService.Template(name);

         template.DefaultLanguage = template.Languages.Contains(projectDefaultLanguage) ? projectDefaultLanguage : template.Languages.First();

         return template;
      }
   }
}
