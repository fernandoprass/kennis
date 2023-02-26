using Builder.Domain.Models;
using Builder.Domain.Wrappers;
using Kennis.Builder.Constants;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Builder.Domain
{
   public interface ISave
   {
      void ContentListToJson(List<Content> contentList, string contentPath);
   }

   public class Save : ISave
   {
      private readonly IFileWrapper _file;
      private readonly ILogger<Build> _logger;

      public Save(IFileWrapper fileWrapper,
         ILogger<Build> logger)
      {
         _file = fileWrapper;
         _logger = logger;
      }

      public void ContentListToJson(List<Content> contentList, string contentPath)
      {
         try
         {
            var filename = Path.Combine(contentPath, Const.File.ContentList);
            var options = new JsonSerializerOptions
            {
               Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
               WriteIndented = true
            };

            var json = JsonSerializer.Serialize(contentList, options)!;

            File.WriteAllText(filename, json);

            _logger.LogInformation("Content list save: {0}", filename);
         }
         catch(Exception ex)
         { 
            _logger.LogError(ex, "Error when try to save content list at {0}", contentPath);
         }
      }
   }
}
