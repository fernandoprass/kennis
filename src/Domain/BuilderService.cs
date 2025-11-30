using Myce.Extensions;

namespace Kennis.Domain
{
   public interface IBuilderService
   {
      void Build(string projectName, bool rebuildAll);
   }

   internal class BuilderService(ILogService logService,
                                 IBuildSiteService buildSiteService,
                                 IProjectService projectService,
                                 ITemplateService templateService,
                                 ITranslationService translationService) : IBuilderService
   {
      private readonly ILogService _logService = logService;
      private readonly IBuildSiteService _buildSiteService = buildSiteService;
      private readonly IProjectService _projectService = projectService;
      private readonly ITemplateService _templateService = templateService;
      private readonly ITranslationService _translationService = translationService;

      public void Build(string projectName, bool rebuildAll)
      {
         _logService.LogInfo(LogCategory.Project, LogAction.BuildStart, projectName);
         var project = _projectService.Load(projectName);

         if (project.IsNotNull())
         {
            var baseTemplate = _templateService.Load(project.Template, project.DefaultLanguageCode);

            if (baseTemplate.IsNotNull())
            {
               foreach (var projectSite in project.Sites)
               {
                  var template = _translationService.Translate(baseTemplate, projectSite.Language.Code);

                  if (template.IsNotNull())
                  {
                     _logService.LogInfo(LogCategory.Site, LogAction.BuildStart, projectSite.Language.Code);

                     _templateService.CopyAssets(project.Folders.Template, template.Assets, project.Folders.SiteDestination[projectSite.Language.Code]);

                     _buildSiteService.Build(project.DefaultLanguageCode, project.Folders.Project, projectSite, template);

                     _logService.LogInfo(LogCategory.Site, LogAction.BuildSuccess, projectSite.Language.Code);
                  }
               }
            }
            _logService.LogInfo(LogCategory.Project, LogAction.BuildSuccess, projectName);
         }
      }
   }
}
