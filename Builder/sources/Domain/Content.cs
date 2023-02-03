using Builder.Domain.Configuration;

namespace Kennis.Builder.Domain
{
    public class Content
    {
        public string Language { get; set; }
        public Author Author { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Reference { get; set; }
        public string ContentFile { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<string> Tags { get; set; }               
        public DateTime PublicationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Draft { get; set; }
        public bool Published { get; set; }
    }
}
