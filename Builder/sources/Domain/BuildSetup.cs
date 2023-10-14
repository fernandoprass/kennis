using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Builder.Domain
{
   public interface IBuildSetup
   {
      Layout Layout(string templateFolder);

      Project ProjectGet(string projectFile);

      void ProjectSiteUpdateLanguageData(string defaulfLanguageCode, IEnumerable<ProjectSite> projectSites);
   }
   public class BuildSetup : IBuildSetup
   {
      private readonly ILoad _load;
      private readonly ILogger<Build> _logger;
      private readonly ISave _save;

      public BuildSetup(ILoad load,
         ILogger<Build> logger,
         ISave save)
      {
         _load = load;
         _logger = logger;
         _save = save;
      }

      #region Project
      public Project ProjectGet(string projectFile)
      {
         var projectPath = GetProjectPath(projectFile);

         var fileName = Path.Combine(projectPath, Const.File.Project);

         var project = _load.Project(fileName);

         if (project.IsNotNull())
         {
            project.Folders = GetProjectFolders(projectPath, project.Name, project.Template);

            return project;
         }

         return null;
      }

      public void ProjectSiteUpdateLanguageData(string defaulfLanguageCode, IEnumerable<ProjectSite> projectSites)
      {
         foreach (var site in projectSites)
         {
            site.Language.IndexFileName = site.Language.Code.Equals(defaulfLanguageCode)
                                          ? string.Concat("index", Const.Extension.WebPages)
                                          : string.Concat("index", "-", site.Language.Code, Const.Extension.WebPages);
         }
      }

      private static ProjectFolder GetProjectFolders(string projectPath, string projectName, string templateName)
      {
         var applicationPath = AppContext.BaseDirectory;

         string template = Path.Combine(applicationPath, Const.Folder.Templates, templateName);

         return new ProjectFolder
         {
            Application = applicationPath,
            Project = projectPath,
            Template = template,
            TemplateTranslations = Path.Combine(template, Const.Folder.TemplatesTranslations),
            Destination = Path.Combine(applicationPath, Const.Folder.Sites, projectName)
         };
      }

      private static string GetProjectPath(string projectName)
      {
         var applicationPath = AppContext.BaseDirectory;

         return Path.Combine(applicationPath, Const.Folder.Projects, projectName);
      }

      #endregion

      #region Load Layout
      public Layout Layout(string templateFolder)
      {
         return _load.Layout(templateFolder);
      }
      #endregion
   }
}
