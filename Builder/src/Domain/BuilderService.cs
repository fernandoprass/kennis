using Builder.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Builder.Domain {
   public interface IBuilderService
   {
      void Build(Project project, bool rebuildAll);
   }

   public class BuilderService : IBuilderService
   {
      private readonly ILayoutService _layoutService;
      private readonly IBuildSiteService _buildSiteService;
      private readonly ILogger<BuilderService> _logger;

      public BuilderService(
         ILogger<BuilderService> logger,
         ILayoutService layoutService,
         IBuildSiteService site)
      {
         _logger = logger;
         _layoutService = layoutService;
         _buildSiteService = site;
      }

      public void Build(Project project, bool rebuildAll)
      {
         var layout = _layoutService.Load(project.Folders.Template);
         if (layout)
         {
            foreach (var projectSite in project.Sites)
            {
               _layoutService.Translate(projectSite.Language.Code);

               _logger.LogInformation("Starting create site in {0}", projectSite.Language.Label);

               _buildSiteService.Build(project.DefaultLanguageCode, project.Folders, projectSite);

               _logger.LogInformation("Ending create site in {0}", projectSite.Language.Label);
            }
         }
      }
   }
}
