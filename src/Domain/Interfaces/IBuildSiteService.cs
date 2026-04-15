using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces;

public interface IBuildSiteService
{
   void Build(string defaultLanguage, string projectFolder, ProjectSite projectSite, Template template);
}
