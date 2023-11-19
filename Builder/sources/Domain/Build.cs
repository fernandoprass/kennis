using Builder.Domain.Models;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Builder.Domain
{
   public interface IBuild
   {
      void Builder(Project project, bool rebuildAll);
   }

   public class Build : IBuild
   {
      private readonly IBuildSetup _setup;
      private readonly IBuildSite _site;
      private readonly ILogger<Build> _logger;

      private Project Project { get; set; }
      private Layout Layout { get; set; }

      public Build(
         ILogger<Build> logger,
         IBuildSetup setup,
         IBuildSite site,
         IProjectService projectService)
      {
         _logger = logger;
         _setup = setup;
         _site = site;
      }

      public void Builder(Project project, bool rebuildAll)
      {

         if (Project.IsNotNull() && Layout.IsNotNull())
         {
            foreach (var projectSite in Project.Sites)
            {
               _logger.LogInformation("Starting create site in {0}", projectSite.Language.Label);

               _site.Build(Project.DefaultLanguageCode, Project.Folders, projectSite);

               _logger.LogInformation("Ending create site in {0}", projectSite.Language.Label);
            }
         }
      }
   }
}
