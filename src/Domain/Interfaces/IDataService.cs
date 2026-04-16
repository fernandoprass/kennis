using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces;

public interface IDataService
{
   List<Content> ContentList { get; set; }

   Task GetContentListAsync(string projectFolder, string languageCode, string htmlPagePath, string htmlPostPath);

   Task SaveContentListAsync(string languageCode);

   void UpdateContentListData();

   void UpdateProjectSiteModified(DateTime lastModified, ProjectSite projectSite);
}
