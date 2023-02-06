using Builder.Domain.Layouts;
using Builder.Domain.Models;
using Builder.Domain.Wrappers;
using Kennis.Builder.Constants;
using System.Text.Json;

namespace Builder.Domain
{
   public interface ILoad
   {
      Project Project(string projectName);
      ILayoutBase LayoutBase(string templateFolder);
   }

   public class Load : ILoad
   {
      private readonly IFileWrapper _file;
      private readonly ILayoutBase _layoutBase;

      public Load(IFileWrapper fileWrapper, ILayoutBase layoutBase)
      {
         _file = fileWrapper;
         _layoutBase = layoutBase;
      }

      public Project Project(string projectName)
      {
         var projectPaths = CreateProjectFolders(projectName);

         var fileName = Path.Combine(projectPaths.Project, LocalEnvironment.File.Project);

         if (File.Exists(fileName))
         {
            string jsonString = _file.ReadAllText(fileName);
            var project = JsonSerializer.Deserialize<Project>(jsonString)!;

            project.Folders = projectPaths;

            UpdateProjectFolders(project);

            return project;
         }

         return null;
      }

      private static ProjectFolder CreateProjectFolders(string projectName)
      {
         var applicationPath = AppContext.BaseDirectory;

         var paths = new ProjectFolder
         {
            Application = applicationPath,
            Project = Path.Combine(applicationPath, LocalEnvironment.Folder.Projects, projectName)
         };
         return paths;
      }

      private static void UpdateProjectFolders(Project project)
      {
         project.Folders.Template = Path.Combine(project.Folders.Application, LocalEnvironment.Folder.Templates, project.Template);
         project.Folders.TemplateTranslations = Path.Combine(project.Folders.Template, LocalEnvironment.Folder.TemplatesTranslations);
         project.Folders.Destination = Path.Combine(project.Folders.Application, LocalEnvironment.Folder.Sites, project.Name);
      }

      public ILayoutBase LayoutBase(string templateFolder)
      {
         var filename = Path.Combine(templateFolder, LocalEnvironment.File.Template);
         var template = ReadTemplate(filename);
         _layoutBase.Get(templateFolder, template);
         return _layoutBase;
      }

      private Template ReadTemplate(string fileName)
      {
         if (_file.Exists(fileName))
         {
            string jsonString = _file.ReadAllText(fileName);
            return JsonSerializer.Deserialize<Template>(jsonString)!;
         }

         return null;
      }
   }
}
