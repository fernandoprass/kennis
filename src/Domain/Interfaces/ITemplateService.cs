using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces
{
   public interface ITemplateService
   {
      Task CopyAssetsAsync(string templapeFolder, IEnumerable<string> assets, string siteDestination);
      Task<Template> LoadAsync(string name, string projectDefaultLanguage);
   }
}
