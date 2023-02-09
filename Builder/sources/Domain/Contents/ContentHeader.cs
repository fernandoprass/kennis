namespace Builder.Domain.Contents
{
   public abstract class ContentHeader
   {
      public string Title { get; set; }
      public string Description { get; set; }
      public string Slug { get; set; }
      public string Reference { get; set; }
      public IEnumerable<string> Categories { get; set; }
      public IEnumerable<string> Tags { get; set; }
      public DateTime Created { get; set; }
      public DateTime? Modified { get; set; }
      public bool Draft { get; set; }
   }
}
