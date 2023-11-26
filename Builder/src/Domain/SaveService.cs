using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using Myce.Wrappers.Contracts;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Builder.Domain
{
   public interface ISaveService
   {
      void Configure(string htmlFolder, string jsonFolder);
      void ToJsonFile<T>(string filename, T contentList);
      void ToHtmlFile(string filename, string webPage);
   }

   public class SaveService : ISaveService {
      private string HtmlFolder { get; set; }
      private string JsonFolder { get; set; }

      private readonly IFileWrapper _file;
      private readonly IPathWrapper _path;
      private readonly ILogger<BuilderService> _logger;

      public SaveService(IFileWrapper fileWrapper,
         IPathWrapper pathWrapper,
         ILogger<BuilderService> logger)
      {
         _file = fileWrapper;
         _path = pathWrapper;
         _logger = logger;
      }

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

            _logger.LogInformation("Json file saved: {filename}", filename);
         }
         catch(Exception ex)
         { 
            _logger.LogError(ex, "Error when try to save to Json file {filename}", filename);
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

            _logger.LogInformation("Html File saved: {filename}", filename);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Error when try to save Html file {filename}", filename);
         }
      }
   }
}
