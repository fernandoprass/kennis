using Myce.Wrappers.Contracts;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Kennis.Domain
{
   public interface ISaveService
   {
      void Configure(string htmlFolder, string jsonFolder);
      void ToJsonFile<T>(string languageCode,string filename, T contentList);
      void ToHtmlFile(string filename, string webPage);
   }

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

      public void ToJsonFile<T>(string languageCode, string filename, T contentList)
      {
         var options = new JsonSerializerOptions
         {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
         };

         var json = JsonSerializer.Serialize(contentList, options)!;

         filename = _path.Combine(JsonFolder, languageCode, filename);

         SaveFile(LogCategory.JsonFile, filename, json);
      }

      public void ToHtmlFile(string filename, string webPage)
      {
         filename = _path.Combine(HtmlFolder, filename);

         SaveFile(LogCategory.HtmlFile,filename, webPage);
      }

      private void SaveFile(LogCategory logCategory, string filename, string content)
      {
         try
         {
            var path = _path.GetDirectoryName(filename);
            if (!_directory.Exists(path))
            {
               _directory.CreateDirectory(path);
            }

            _file.WriteAllText(filename, content);

            _logService.LogInfo(logCategory, LogAction.SaveSuccess, filename);
         }
         catch (Exception ex)
         {
            _logService.LogError(ex, logCategory, LogAction.SaveFail, filename);
         }
      }
   }
}
