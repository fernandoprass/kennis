namespace Builder.Domain.Models
{
    public class Template
    {
        public IEnumerable<string> Languages { get; set; }
        public string Index { get; set; }
        public string Page { get; set; }
        public string Blog { get; set; }
        public string BlogArchive { get; set; }
        public string BlogCategories { get; set; }
        public string BlogPost { get; set; }
        public string BlogTags { get; set; }
        public TemplateLoop Loops { get; set; }
    }
}