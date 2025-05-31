using Kennis.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kennis {
   public class Kennis {
      static void Main(string[] args)
      {
         var projectName = "KennisDemo";
         bool regenerateAllSite = true;

         var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("config.json").Build();

         var language = config["language"];

         var serviceProvider = Service.Configure(projectName);

         var loadService = serviceProvider.GetService<ILoadService>();

         var logMessages = loadService.LogMessages(language);

         var builderService = serviceProvider.GetService<IBuilderService>();

         builderService.Build(logMessages, projectName, regenerateAllSite);

      }
   }
}