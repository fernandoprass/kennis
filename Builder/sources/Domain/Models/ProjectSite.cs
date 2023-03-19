using Kennis.Builder.Constants;

namespace Builder.Domain.Models
{
   public class ProjectSite
   {
      public Language Language { get; set; }
      public string Title { get; set; }
      public string Subtitle { get; set; }
      public string Description { get; set; }
      public string Keywords { get; set; }
      public string DateTimeFormat { get; set; }
      public string IndexFileName { get; private set; }
      public DateTime Modified { get; set; }
      public string GoogleAnalyticTrackingId { get; set; }
      public Author Author { get; set; }
      public ProjectSiteFolders Folders { get; set; }

      public static string GetIndexFileName(string defaultLanguage, string language)
      {
         return defaultLanguage == language
                                   ? string.Concat("index", Const.Extension.WebPages)
                                   : string.Concat("index", "-", language, Const.Extension.WebPages);
      }

      public void SetIndexFileName(string defaultLanguage)
      {
         IndexFileName = GetIndexFileName(defaultLanguage, Language.Code);
      }
   }
}
