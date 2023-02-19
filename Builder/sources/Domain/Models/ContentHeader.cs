namespace Builder.Domain.Models
{
   public class ContentHeader
   {
      public string Icon { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }
      public string Reference { get; set; }
      public bool Menu { get; set; }
      public IEnumerable<string> Categories { get; set; }
      public IEnumerable<string> Tags { get; set; }
      public DateTime Created { get; set; }
      public DateTime? Updated { get; set; }
      public bool Draft { get; set; }
   }
}
