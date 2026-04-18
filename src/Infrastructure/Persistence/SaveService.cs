using Kennis.Domain;
using Kennis.Domain.Interfaces;
using Myce.Wrappers.Contracts;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Kennis.Infrastructure.Persistence;

public class SaveService(IFileWrapper fileWrapper,
                         IDirectoryWrapper directoryWrapper,
                         IPathWrapper pathWrapper,
                         ILogService logService) : ISaveService {

   private readonly IDirectoryWrapper _directory = directoryWrapper;
   private readonly IFileWrapper _file = fileWrapper;
   private readonly IPathWrapper _path = pathWrapper;
   private readonly ILogService _logService = logService;

   private string HtmlFolder { get; set; } = string.Empty;
   private string JsonFolder { get; set; } = string.Empty;

   public void Configure(string htmlFolder, string jsonFolder)
   {
      HtmlFolder = htmlFolder;
      JsonFolder = jsonFolder;
   }

   public async Task ToJsonFileAsync<T>(string languageCode, string filename, T contentList)
   {
      var options = new JsonSerializerOptions
      {
         Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
         WriteIndented = true
      };

      var json = JsonSerializer.Serialize(contentList, options)!;

      filename = _path.Combine(JsonFolder, languageCode, filename);

      await SaveFileAsync(LogCategory.JsonFile, filename, json);
   }

   public async Task ToHtmlFileAsync(string filename, string webPage)
   {
      filename = _path.Combine(HtmlFolder, filename);

      await SaveFileAsync(LogCategory.HtmlFile, filename, webPage);
   }

   private async Task SaveFileAsync(LogCategory logCategory, string filename, string content)
   {
      try
      {
         var path = _path.GetDirectoryName(filename);
         if (! _directory.Exists(path))
         {
            _directory.CreateDirectory(path);
         }

         await _file.WriteAllTextAsync(filename, content);

         _logService.LogInfo(logCategory, LogAction.SaveSuccess, filename);
      }
      catch (Exception ex)
      {
         _logService.LogError(ex, logCategory, LogAction.SaveFail, filename);
      }
   }
}
