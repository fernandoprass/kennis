namespace Kennis.Domain.Models
{
   public class AppSettings
   {
      public string Language { get; set; }
      public string LogLevel { get; set; }
      public string ProjectName { get; set; }
      public bool RebuildAllSite { get; set; }
      public AppSettingsFolders Folders { get; set; }
   }

   public class AppSettingsFolders
   {
      public string Projects { get; set; }
      public string Templates { get; set; }
   }
}
