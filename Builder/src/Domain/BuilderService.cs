using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Builder.Domain {
   public interface IBuilderService {
      void Build(string projectName, bool rebuildAll);
   }

   public class BuilderService : IBuilderService {
      private readonly ITemplateService _templateService;
      private readonly IBuildSiteService _buildSiteService;
      private readonly ILogger<BuilderService> _logger;
      private readonly IProjectService _projectService;

      public BuilderService(
         ILogger<BuilderService> logger,
         ITemplateService templateService,
         IBuildSiteService site,
         IProjectService projectService)
      {
         _logger = logger;
         _templateService = templateService;
         _buildSiteService = site;
         _projectService = projectService;
      }

      public void Build(string projectName, bool rebuildAll)
      {
         _logger.LogInformation("Starting to build Project {projectName}", projectName);
         var project = _projectService.Load(projectName);

         if (project.IsNotNull())
         {
            _templateService.Load(project.DefaultLanguageCode);

            if (_templateService.IsTemplateLoaded())
            {
               foreach (var projectSite in project.Sites)
               {
                  var template = _templateService.TranslateTo(projectSite.Language.Code);

                  _logger.LogInformation("Starting to build site in {languageLabel}", projectSite.Language.Label);

                  _buildSiteService.Build(project.DefaultLanguageCode, project.Folders, projectSite);

                  _logger.LogInformation("Finished to build site in {languageLabel}", projectSite.Language.Label);
               }
            }
         }
         _logger.LogInformation("Finished to build Project {projectName}", projectName);
      }
   }
}
