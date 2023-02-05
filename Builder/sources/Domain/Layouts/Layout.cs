namespace Builder.Domain.Layouts
{
   public interface ILayout
   {
      string Index { get; set; }
      string Page { get; set; }
      string Blog { get; set; }
      string BlogArchive { get; set; }
      string BlogCategories { get; set; }
      string BlogPost { get; set; }
      string BlogTags { get; set; }
      LayoutLoop Loops { get; set; }
   }

   public class Layout : ILayout
   {
      public string Index { get; set; }
      public string Page { get; set; }
      public string Blog { get; set; }
      public string BlogArchive { get; set; }
      public string BlogCategories { get; set; }
      public string BlogPost { get; set; }
      public string BlogTags { get; set; }
      public LayoutLoop Loops { get; set; }
   }
}