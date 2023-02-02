namespace Builder.Domain.Configuration
{
    public class TemplateHtmlFile
    {
        public int Order { get; set; }
        public string FileName { get; set; }
        public bool ProcessOnlyOnce { get; set; }
    }
}
