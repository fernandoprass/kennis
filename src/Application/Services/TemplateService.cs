using Kennis.Domain;
using Kennis.Domain.Interfaces;
using Kennis.Domain.Models;
using Myce.Wrappers.Contracts;

namespace Kennis.Application.Services;

public class TemplateService(IDirectoryWrapper directoryWrapper,
                         IFileWrapper fileWrapper, 
                         IPathWrapper pathWrapper,
                         ILogService logService, 
                         ILoadService loadService) : ITemplateService
{
   private readonly IDirectoryWrapper _directoryWrapper = directoryWrapper;
   private readonly IFileWrapper _fileWrapper = fileWrapper;
   private readonly IPathWrapper _pathWrapper = pathWrapper;
   private readonly ILogService _logService = logService;
   private readonly ILoadService _loadService = loadService;

   public async Task CopyAssetsAsync(string templapeFolder, IEnumerable<string> assets, string siteDestination)
   {
      _logService.LogInfo(LogCategory.Template, LogAction.FileCopy, siteDestination);
      try
      {
         foreach (string asset in assets)
         {
            string destination = _pathWrapper.Combine(siteDestination, asset);

            if (!await _directoryWrapper.ExistsAsync(destination))
            {
               await _directoryWrapper.CreateDirectoryAsync(destination);
            }

            string source = _pathWrapper.Combine(templapeFolder, asset);

            // Copy all files and subfolders
            var files = await _directoryWrapper.GetFilesAsync(source, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
               // Get the relative path from the asset root
               var relativePath = file.Substring(source.Length).TrimStart(Path.DirectorySeparatorChar);
               var destFile = _pathWrapper.Combine(destination, relativePath);

               var fileDestinationFolder = _pathWrapper.GetDirectoryName(destFile);

               if (!await _directoryWrapper.ExistsAsync(fileDestinationFolder))
               {
                  await _directoryWrapper.CreateDirectoryAsync(fileDestinationFolder);
               }

               await _fileWrapper.CopyAsync(file, destFile, true);
            }
         }
      }
      catch (Exception ex)
      {
         _logService.LogError(LogCategory.Template, LogAction.FileCopyFail, ex.Message);
      }
      
   }

   public async Task<Template> LoadAsync(string name, string projectDefaultLanguage)
   {
      var template = await _loadService.TemplateAsync(name);

      template.DefaultLanguage = template.Languages.Contains(projectDefaultLanguage) ? projectDefaultLanguage : template.DefaultLanguage;

      if (!template.Languages.Contains(projectDefaultLanguage))
      {
         _logService.LogWarning(LogCategory.Template, LogAction.NotSupported, projectDefaultLanguage, template.DefaultLanguage);
      }

      return template;
   }
}
