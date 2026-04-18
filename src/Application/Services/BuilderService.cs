using Kennis.Domain;
using Kennis.Domain.Interfaces;

namespace Kennis.Application.Services;

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

   public async Task BuildAsync(string projectName, bool rebuildAll)
   {
      _logService.LogInfo(LogCategory.Project, LogAction.BuildStart, projectName);
      var project = await _projectService.LoadAsync(projectName);

      if (project != null )
      {
         var baseTemplate = await _templateService.LoadAsync(project.Template, project.DefaultLanguageCode);

         if (baseTemplate != null )
         {
            var tasks = project.Sites.Select(async projectSite =>
            {
               var template = await _translationService.TranslateAsync(baseTemplate, projectSite.Language.Code);

               if (template != null )
               {
                  _logService.LogInfo(LogCategory.Site, LogAction.BuildStart, projectSite.Language.Code);

                  await _templateService.CopyAssets(project.Folders.Template, template.Assets, project.Folders.SiteDestination[projectSite.Language.Code]);

                  await _buildSiteService.BuildAsync(project.DefaultLanguageCode, project.Folders.Project, projectSite, template);

                  _logService.LogInfo(LogCategory.Site, LogAction.BuildSuccess, projectSite.Language.Code);
               }
            });

            await Task.WhenAll(tasks);
         }
         _logService.LogInfo(LogCategory.Project, LogAction.BuildSuccess, projectName);
      }
   }
}
