namespace Builder.Domain.Models
{
    public class Content : ContentHeader
    {
        public string Filename { get; set; }
        public string Keywords { get; set; }
        public DateTime? Published { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }
        public bool Delete { get; set; }
    }
}
