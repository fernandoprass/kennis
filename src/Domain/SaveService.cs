using Myce.Wrappers.Contracts;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Kennis.Domain
{
   public interface ISaveService
   {
      void Configure(string htmlFolder, string jsonFolder);
      void ToJsonFile<T>(string filename, T contentList);
      void ToHtmlFile(string filename, string webPage);
   }

   public class SaveService(IFileWrapper fileWrapper,
                            IPathWrapper pathWrapper,
                            ILogService logService) : ISaveService {

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

      public void ToJsonFile<T>(string filename, T contentList)
      {
         try
         {
            var options = new JsonSerializerOptions
            {
               Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
               WriteIndented = true
            };

            var json = JsonSerializer.Serialize(contentList, options)!;

            filename = _path.Combine(JsonFolder, filename);

            ToHtmlFile(filename, json);

            _logService.LogInfo(LogCategory.JsonFile, LogAction.FileSaveSuccessfully, filename);
         }
         catch(Exception ex)
         { 
            _logService.LogError(ex, LogCategory.JsonFile, LogAction.FileSaveFailed, filename);
         }
      }

      public void ToHtmlFile(string filename, string webPage)
      {
         filename = _path.Combine(HtmlFolder, filename);

         ToTxtFile(filename, webPage);
      }

      private void ToTxtFile(string filename, string content)
      {
         try
         {
            _file.WriteAllText(filename, content);

            _logService.LogInfo(LogCategory.HtmlFile, LogAction.FileSaveSuccessfully, filename);
         }
         catch (Exception ex)
         {
            _logService.LogError(ex, LogCategory.HtmlFile, LogAction.FileSaveFailed, filename);
         }
      }
   }
}
