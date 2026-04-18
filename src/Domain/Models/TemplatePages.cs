namespace Kennis.Domain.Models
{
   public class TemplatePages
   {
      public string Index { get; set; }
      public string Page { get; set; }
      public string Blog { get; set; }
      public string BlogPost { get; set; }
      public string BlogArchive { get; set; }
      public string BlogCategories { get; set; }
      public string BlogTags { get; set; }

      public TemplatePagesPartials Partials { get; set; }
      public TemplatePagesLoops Loops { get; set; }
   }
}
