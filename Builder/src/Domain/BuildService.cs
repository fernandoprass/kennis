using Builder.Domain.Models;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Builder.Domain {
   public interface IBuildService
   {
      void Builder(Project project, bool rebuildAll);
   }

   public class BuildService : IBuildService
   {
      private readonly ILayoutService _layoutService;
      private readonly IBuildSite _site;
      private readonly ILogger<BuildService> _logger;

      public BuildService(
         ILogger<BuildService> logger,
         ILayoutService layoutService,
         IBuildSite site)
      {
         _logger = logger;
         _layoutService = layoutService;
         _site = site;
      }

      public void Builder(Project project, bool rebuildAll)
      {
         var layout = _layoutService.Load(project.Folders.Template);
         if (layout.IsNotNull())
         {
            foreach (var projectSite in project.Sites)
            {
               _logger.LogInformation("Starting create site in {0}", projectSite.Language.Label);

               _site.Build(project.DefaultLanguageCode, project.Folders, projectSite);

               _logger.LogInformation("Ending create site in {0}", projectSite.Language.Label);
            }
         }
      }
   }
}
