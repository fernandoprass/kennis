using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Kennis.Domain
{
   public interface IBuilderService
   {
      void Build(string projectName, bool rebuildAll);
   }

   internal class BuilderService : IBuilderService
   {
      private readonly ILogService _logService;
      private readonly IBuildSiteService _buildSiteService;
      private readonly IProjectService _projectService;
      private readonly ITemplateService _templateService;
      private readonly ITranslationService _translationService;

      public BuilderService(
         ILogService logService,
         IBuildSiteService site,
         IProjectService projectService,
         ITemplateService templateService,
         ITranslationService translationService)
      {
         _logService = logService;
         _buildSiteService = site;
         _projectService = projectService;
         _templateService = templateService;
         _translationService = translationService;
      }

      public void Build(string projectName, bool rebuildAll)
      {
         _logService.LogInfo(Const.Log.Category.Project, Const.Log.Action.BuildStarting, projectName);
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
                     _logService.LogInfo(Const.Log.Category.Site, Const.Log.Action.BuildStarting, projectSite.Language.Label);

                     _buildSiteService.Build(project.DefaultLanguageCode, projectSite, template);

                     _logService.LogInfo(Const.Log.Category.Site, Const.Log.Action.BuildFinished, projectSite.Language.Label);
                  }
               }
            }
            _logService.LogInfo(Const.Log.Category.Project, Const.Log.Action.BuildFinished, projectName);
         }
      }
   }
}
