using Kennis.Application.Validators;
using Kennis.Domain;
using Kennis.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Kennis;

public class Kennis
{
   static async Task Main(string[] args)
   {
      string projectName = "KennisDemo";

      var serviceProvider = Service.Configure(projectName);

      var loadService = serviceProvider.GetService<ILoadService>()!;
      var logService = serviceProvider.GetService<ILogService>()!;

      var appSettings = await loadService.AppSettingsAsync();

      appSettings.ProjectName = projectName;

      var result = appSettings.Validate();

      if (await logService.LoadMessagesAsync(appSettings.Language))
      {
         logService.LogInfo(LogCategory.AppSettings, LogAction.LoadFinishedSuccess);
         if (result.IsSuccess)
         {
            var builderService = serviceProvider.GetService<IBuilderService>()!;

            await builderService.BuildAsync(appSettings);
         }
         else
         {
            logService.LogMessagesFromValidator(nameof(appSettings), result.Messages);
         }
      }
      else
      {
         logService.LogCritical(Const.LogMessageError, [appSettings.Language]);
      }
   }
}
