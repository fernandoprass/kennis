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

         var logService = serviceProvider.GetService<ILogService>();

         if (logService.LoadMessages(language))
         {
            var builderService = serviceProvider.GetService<IBuilderService>();

            builderService.Build(projectName, regenerateAllSite);
         }
         else
         {
            Console.WriteLine("Log message file not found. Reinstall the application");
         }
      }
   }
}