using Builder.Domain;
using Builder.Domain.Layouts;
using Builder.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Builder
{
    public class KennisBuilder
   {
      static void Main(string[] args)
      {
         var projectName = "KennisDemo";

         var serviceProvider = IoC.Configure(projectName);

         //todo create a service to read project and layoutBase
         var project = Project.Get(projectName);

         var layoutBase = serviceProvider.GetService<ILayoutBase>();

         var build = serviceProvider.GetService<IBuild>();

         layoutBase.Get(project.Template);

         build.Builder(project, layoutBase);
      }

   }
}