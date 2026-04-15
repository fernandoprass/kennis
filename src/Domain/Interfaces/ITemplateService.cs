using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces
{
   public interface ITemplateService
   {
      void CopyAssets(string templapeFolder, IEnumerable<string> assets, string siteDestination);
      Template Load(string name, string projectDefaultLanguage);
   }
}
