namespace Builder.Domain.Configuration
{
    public class ProjectSite
    {
        public string Language { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public Author Author { get; set; }
        public ProjectSiteFolders Folders { get; set; }
    }
}
