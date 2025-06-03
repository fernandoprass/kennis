using Kennis.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kennis
{
   public class Kennis {
      static void Main(string[] args)
      {
         string projectName = "KennisDemo";
         bool regenerateAllSite = true;

         var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("config.json").Build();

         string language = config["language"];
         string logLevel = config["logLevel"];

         var serviceProvider = Service.Configure(projectName, logLevel);

         var logService = serviceProvider.GetService<ILogService>();

         if (logService.LoadMessages(language))
         {
            var builderService = serviceProvider.GetService<IBuilderService>();

            builderService.Build(projectName, regenerateAllSite);
         }
         else
         {
            logService.LogCritical("Log message file not found for {language}. Reinstall the application", [language]);
         }
      }
   }
}
