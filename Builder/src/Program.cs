using Builder.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Builder {
   public class KennisBuilder {
      static void Main(string[] args)
      {
         var projectName = "KennisDemo";
         bool regenerateAllSite = true;

         var serviceProvider = Service.Configure(projectName);

         var builder = serviceProvider.GetService<IBuilderService>();

         builder.Build(projectName, regenerateAllSite);

      }
   }
}