using Builder.Domain.Models;
using Microsoft.Extensions.Logging;
using Myce.Extensions;

namespace Builder.Domain
{
   public interface IBuild
   {
      void Builder(string projectName);
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
         IBuildSite site)
      {
         _logger = logger;
         _setup = setup;
         _site = site;
      }

      public void Builder(string projectName)
      {
         Setup(projectName);

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

      private void Setup(string projectName)
      {
         Project = _setup.ProjectGet(projectName);

         //todo add validate here
         if (Project.IsNotNull())
         {
            _setup.ProjectSiteUpdateLanguageData(Project.DefaultLanguageCode, Project.Sites);

            Layout = _setup.Layout(Project.Folders.Template);
         }
      }
   }
}
