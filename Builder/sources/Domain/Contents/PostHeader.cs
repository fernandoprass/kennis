using YamlDotNet.Serialization;

namespace Builder.Domain.Contents
{
   public class PostHeader 
   {
      [YamlMember(Alias = "title")]
      public string Title { get; set; }

      [YamlMember(Alias = "description")]
      public string Description { get; set; }

      [YamlMember(Alias = "keywords")]
      public string Keywords { get; set; }

      [YamlMember(Alias = "slug")]
      public string Slug { get; set; }

      [YamlMember(Alias = "reference")]
      public string Reference { get; set; }

      [YamlMember(Alias = "categories")]
      public IEnumerable<string> Categories { get; set; }
      public IEnumerable<string> Tags { get; set; }
      public DateTime Created { get; set; }
      public DateTime? Modified { get; set; }
      public bool Draft { get; set; }
   }
}
