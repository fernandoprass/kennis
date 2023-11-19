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
      public DateTime Modified { get; set; }
      public string? GoogleAnalyticTrackingId { get; set; }
      public Author Author { get; set; }
      public ProjectSiteFolders Folders { get; set; }
   }
}
