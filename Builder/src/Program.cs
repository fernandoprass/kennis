using Builder.Domain;
using Builder.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Myce.Extensions;

namespace Builder {
   public class KennisBuilder {
      static void Main(string[] args)
      {
         var projectName = "KennisDemo";
         bool regenerateAllSite = true;

         var serviceProvider = Service.Configure(projectName);

         var project = LoadProject(serviceProvider, projectName);

         if (project.IsNotNull())
         {
            var builder = serviceProvider.GetService<IBuilderService>();

            builder.Build(project, regenerateAllSite);
         }
      }


      private static Project LoadProject(ServiceProvider serviceProvider, string projectName)
      {
         var projectService = serviceProvider.GetService<IProjectService>();
         var project = projectService.Load(projectName);

         //todo add validate here
         if (project.IsNotNull())
         {
            projectService.ProjectSiteUpdateLanguageData(project.DefaultLanguageCode, project.Sites);
         }

         return project;
      }
   }
}