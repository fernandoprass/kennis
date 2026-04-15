using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces;

public interface IDataService
{
   List<Content> ContentList { get; set; }

   void GetContentList(string projectFolder, string languageCode, string htmlPagePath, string htmlPostPath);

   void SaveContentList(string languageCode);

   void UpdateContentListData();

   void UpdateProjectSiteModified(DateTime lastModified, ProjectSite projectSite);
}
