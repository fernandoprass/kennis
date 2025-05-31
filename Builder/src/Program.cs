using Kennis.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Kennis {
   public class Kennis {
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