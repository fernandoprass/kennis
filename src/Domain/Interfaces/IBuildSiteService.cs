using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces;

public interface IBuildSiteService
{
   Task BuildAsync(string defaultLanguage, string projectFolder, ProjectSite projectSite, Template template);
}
