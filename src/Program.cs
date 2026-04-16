using Kennis.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kennis;

public class Kennis {
   static async Task Main(string[] args)
   {
      string projectName = "KennisDemo";
      bool rebuildAllSite = true;

      var config = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("config.json").Build();

      string language = config["language"];
      string logLevel = config["logLevel"];

      var serviceProvider = Service.Configure(projectName, logLevel);

      var logService = serviceProvider.GetService<ILogService>()!;

      if (await logService.LoadMessagesAsync(language))
      {
         var builderService = serviceProvider.GetService<IBuilderService>()!;

         await builderService.BuildAsync(projectName, rebuildAllSite);
      }
      else
      {
         logService.LogCritical("Log message file not found for {language}. Reinstall the application", [language]);
      }
   }
}
