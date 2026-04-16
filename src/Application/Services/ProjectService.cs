using Myce.Wrappers.Contracts;
using Kennis.Domain;
using Kennis.Domain.Interfaces;
using Myce.FluentValidator;
using Kennis.Domain.Models;
using Myce.Response;
using Myce.Response.Messages;

namespace Kennis.Application.Services;

public class ProjectService (ILoadService loadService,
      ILogService logService,
      IPathWrapper pathWrapper,
      ISaveService saveService) : IProjectService
{
   private readonly ILoadService _loadService = loadService;
   private readonly ILogService _logService = logService;
   private readonly IPathWrapper _path = pathWrapper;
   private readonly ISaveService _saveService = saveService;

   public async Task<Project?> LoadAsync(string projectName)
   {
      var filename = GetProjectFilename(projectName);

      var project = await _loadService.ProjectAsync(filename);

      if (project != null )
      {
         _logService.LogTrace(LogCategory.Project, LogAction.ReadSuccess, projectName);

         ProjectSiteUpdateLanguageData(project.DefaultLanguageCode, project.Sites);

         var languages = project.Sites.Select(s => s.Language.Code).Distinct().ToList();

         project.Folders = GetProjectFolders(project.Name, project.Template, languages);

         _loadService.Configure(project.Folders);
         _saveService.Configure(project.Folders.SiteDestination[project.DefaultLanguageCode], project.Folders.Project);

         return project;
      }
      
      _logService.LogCritical(LogCategory.Project, LogAction.LoadFinishedFail, projectName);

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
      var validator = new FluentValidator<Project>()
            .RuleFor(p => p.Template).IsRequired()
            .RuleFor(p => p.Sites).IsRequired();

      var result = validator.Validate(project);

      if (result)
      {
         return Result.Success();
      }
      return Result.Failure(validator.Messages);
   }

   private static Result ValidateProjectSites(IEnumerable<ProjectSite> projectSites)
   {
      var validator = new FluentValidator<ProjectSite>()
            .RuleForValue(projectSites.Any()).IsTrue(new ErrorMessage("Project should have at least one site"));

      //todo => validate each site with a loop and return all messages in case of failure
      //if (projectSites.Any())
      //{
      //   foreach (var site in projectSites)
      //   {
      //      validator.IsMandatory(site.Title, new ErrorIsMandatory(nameof(site.Title)))
      //                     .IsMandatory(site.Language, new ErrorIsMandatory(nameof(site.Language)))
      //                     .IsMandatoryIf(site.Language.Code, site.Language != null , new ErrorIsMandatory(nameof(site.Language.Code)));
      //   }
      //}

      return Result.Success();
   }

   private string GetProjectFilename(string projectName)
   {
      var applicationPath = AppContext.BaseDirectory;

      return _path.Combine(applicationPath, Const.Folder.Projects, projectName, Const.File.Project);
   }

   private ProjectFolder GetProjectFolders(string projectName, string templateName, List<string> languages)
   {
      var applicationPath = AppContext.BaseDirectory;

      string template = _path.Combine(applicationPath, Const.Folder.Templates, templateName);

      string baseSiteDestination = _path.Combine(applicationPath, Const.Folder.Sites, projectName);
      var siteDestination = new Dictionary<string, string>();

      foreach(string language in languages) {
         siteDestination.Add(language, _path.Combine(baseSiteDestination, language));
      }

      return new ProjectFolder
      {
         Application = applicationPath,
         Project = _path.Combine(applicationPath, Const.Folder.Projects, projectName),
         Template = template,
         TemplateTranslations = _path.Combine(template, Const.Folder.TemplatesTranslations),
         SiteDestination = siteDestination
      };
   }

   public async Task SaveAsync()
   {
     // await _saveService.ToJsonFileAsync<Project>(_project)
   }
}
