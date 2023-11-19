﻿using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Myce.Extensions;
using Myce.Response;
using Myce.Validation;
using Myce.Validation.ErrorMessages;
using Myce.Wrappers.Contracts;

namespace Builder.Domain
{
   public interface IProjectService
   {
      Project? Load(string projectName);

      void ProjectSiteUpdateLanguageData(string defaulfLanguageCode, IEnumerable<ProjectSite> projectSites);

      Result Validate(Project project);
   }

   public class ProjectService : IProjectService
   {
      private readonly ILoad _load;
      private readonly IPathWrapper _path;

      public ProjectService(ILoad load,
         IPathWrapper pathWrapper)
      {
         _load = load;
         _path = pathWrapper;
      }

      public Project? Load(string projectName)
      {
         var filename = GetProjectFilename(projectName);

         var project = _load.Project(filename);

         if (project.IsNotNull())
         {
            project.Folders = GetProjectFolders(project.Name, project.Template);

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

      public Result Validate(Project project)
      {
         var result = ValidateProject(project);

         if (result.IsValid)
         {

         }

         return result;
      }

      private static Result ValidateProject(Project project)
      {
         var entityValidator = new EntityValidator()
                     .IsMandatory(project.Template, new ErrorIsMandatory(nameof(project.Template)))
                     .IsMandatory(project.Sites, new ErrorIsMandatory(nameof(project.Sites)));

         

         return entityValidator.Validate();
      }

      private static Result ValidateProjectSites(IEnumerable<ProjectSite> projectSites)
      {
         var entityValidator = new EntityValidator();
         entityValidator.If(projectSites.Any(), new ErrorIsMandatory("Project should have at least one site"));

         if (projectSites.Any())
         {
            foreach (var site in projectSites)
            {
               entityValidator.IsMandatory(site.Title, new ErrorIsMandatory(nameof(site.Title)))
                              .IsMandatory(site.Language, new ErrorIsMandatory(nameof(site.Language)))
                              .IsMandatoryIf(site.Language.Code, site.Language.IsNotNull(), new ErrorIsMandatory(nameof(site.Language.Code)));
            }
         }

         return entityValidator.Validate();
      }

      private string GetProjectFilename(string projectName)
      {
         var applicationPath = AppContext.BaseDirectory;

         return _path.Combine(applicationPath, Const.Folder.Projects, projectName, Const.File.Project);
      }

      private ProjectFolder GetProjectFolders( string projectName, string templateName)
      {
         var applicationPath = AppContext.BaseDirectory;

         string template = _path.Combine(applicationPath, Const.Folder.Templates, templateName);

         return new ProjectFolder
         {
            Application = applicationPath,
            Project = _path.Combine(applicationPath, Const.Folder.Projects, projectName),
            Template = template,
            TemplateTranslations = _path.Combine(template, Const.Folder.TemplatesTranslations),
            Destination = _path.Combine(applicationPath, Const.Folder.Sites, projectName)
         };
      }
   }
}
