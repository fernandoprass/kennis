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

   public Task CopyAssets(string templapeFolder, IEnumerable<string> assets, string siteDestination)
   {
      _logService.LogInfo(LogCategory.Template, LogAction.FileCopy, siteDestination);
      try
      {
         foreach (string asset in assets)
         {
            string destination = _pathWrapper.Combine(siteDestination, asset);

            if (!_directoryWrapper.Exists(destination))
            {
               _directoryWrapper.CreateDirectory(destination);
            }

            string source = _pathWrapper.Combine(templapeFolder, asset);

            // Copy all files and subfolders
            var files = _directoryWrapper.GetFiles(source, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
               // Get the relative path from the asset root
               var relativePath = file.Substring(source.Length).TrimStart(Path.DirectorySeparatorChar);
               var destFile = _pathWrapper.Combine(destination, relativePath);

               var fileDestinationFolder = _pathWrapper.GetDirectoryName(destFile);

               if (!_directoryWrapper.Exists(fileDestinationFolder))
               {
                  _directoryWrapper.CreateDirectory(fileDestinationFolder);
               }

               _fileWrapper.Copy(file, destFile, true);
            }
         }
      }
      catch (Exception ex)
      {
         _logService.LogError(LogCategory.Template, LogAction.FileCopyFail, ex.Message);
      }
      
      return Task.CompletedTask;
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
