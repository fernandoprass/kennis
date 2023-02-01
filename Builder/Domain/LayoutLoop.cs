using Builder.Domain.Configuration;

namespace Kennis.Builder.Domain
{
    public class LayoutLoop
   {
      public string BlogArchive { get; set; }
      public string BlogCategories { get; set; }
      public string BlogPostLast10 { get; set; }
      public string BlogPostLast5 { get; set; }
      public string BlogPostLast3 { get; set; }
      public string BlogPosts { get; set; }
      public string BlogTags { get; set; }
      public string SocialMedia { get; set; }
      public IEnumerable<TemplateMenuHtmlFile> Menus { get; set; }

   }
}
