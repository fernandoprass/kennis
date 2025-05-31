using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Kennis.Domain
{
   public interface IBuilderService
   {
      void Build(string projectName, bool rebuildAll);
   }

   public class BuilderService : IBuilderService
   {
      private readonly ILogger<BuilderService> _logger;
      private readonly IBuildSiteService _buildSiteService;
      private readonly IProjectService _projectService;
      private readonly ITemplateService _templateService;
      private readonly ITranslationService _translationService;

      public BuilderService(
         ILogger<BuilderService> logger,
         IBuildSiteService site,
         IProjectService projectService,
         ITemplateService templateService,
         ITranslationService translationService)
      {
         _logger = logger;
         _buildSiteService = site;
         _projectService = projectService;
         _templateService = templateService;
         _translationService = translationService;
      }

      public void Build(string projectName, bool rebuildAll)
      {
         _logger.LogInformation(Const.Log.Project.StartBuild, projectName);
         var project = _projectService.Load(projectName);

         if (project.IsNotNull())
         {
            var baseTemplate = _templateService.Load(project.DefaultLanguageCode);

            if (baseTemplate.IsNotNull())
            {
               foreach (var projectSite in project.Sites)
               {
                  var template = _translationService.Translate(baseTemplate, projectSite.Language.Code);

                  if (template.IsNotNull())
                  {
                     _logger.LogInformation("Starting to build site in {languageLabel}", projectSite.Language.Label);

                     _buildSiteService.Build(project.DefaultLanguageCode, projectSite, template);

                     _logger.LogInformation("Finished to build site in {languageLabel}", projectSite.Language.Label);
                  }
               }
            }
            _logger.LogInformation(Const.Log.Project.FinishBuild, projectName);
         }
      }
   }
}
