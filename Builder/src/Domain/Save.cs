using Microsoft.Extensions.Logging;
using Myce.Wrappers.Contracts;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Builder.Domain
{
   public interface ISave
   {
      void Configure(string htmlFolder, string jsonFolder);
      void ToJsonFile<T>(string filename, T contentList);
      void ToHtmlFile(string filename, string webPage);
   }

   public class Save : ISave
   {
      private string HtmlFolder { get; set; }
      private string JsonFolder { get; set; }


      private readonly IFileWrapper _file;
      private readonly ILogger<BuilderService> _logger;

      public Save(IFileWrapper fileWrapper,
         ILogger<BuilderService> logger)
      {
         _file = fileWrapper;
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

            filename = Path.Combine(JsonFolder, filename);

            ToHtmlFile(filename, json);

            _logger.LogInformation("Json file serialized: {0}", filename);
         }
         catch(Exception ex)
         { 
            _logger.LogError(ex, "Error when try to serialize to Json {0}", filename);
         }
      }

      public void ToHtmlFile(string filename, string webPage)
      {
         filename = Path.Combine(HtmlFolder, filename);

         ToTxtFile(filename, webPage);
      }

      private void ToTxtFile(string filename, string content)
      {
         try
         {
            _file.WriteAllText(filename, content);

            _logger.LogInformation("File saved: {0}", filename);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Error when try to save file {0}", filename);
         }
      }
   }
}
