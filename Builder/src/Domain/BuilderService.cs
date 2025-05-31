using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Kennis.Domain
{
   public interface IBuilderService
   {
      void Build(Dictionary<string, Dictionary<string, string>> logMessages, string projectName, bool rebuildAll);
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

      public static string GetMessage(Dictionary<string, Dictionary<string, string>> logMessages, string category, string key)
      {
         return logMessages.ContainsKey(category) && logMessages[category].ContainsKey(key)
             ? logMessages[category][key]
             : $"[{key}]";
      }

      public void Build(Dictionary<string, Dictionary<string, string>> logMessages, string projectName, bool rebuildAll)
      {
         string message = GetMessage(logMessages, Const.Log.Category.Project, Const.Log.Action.BuildStarting);
         _logger.LogInformation(message, projectName);
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
                     _logger.LogInformation(GetMessage(logMessages, Const.Log.Category.Site, Const.Log.Action.BuildStarting), projectSite.Language.Label);

                     _buildSiteService.Build(project.DefaultLanguageCode, projectSite, template);

                     _logger.LogInformation(GetMessage(logMessages, Const.Log.Category.Site, Const.Log.Action.BuildFinished), projectSite.Language.Label);
                  }
               }
            }
            _logger.LogInformation(GetMessage(logMessages, Const.Log.Category.Project, Const.Log.Action.BuildFinished), projectName);
         }
      }
   }
}
