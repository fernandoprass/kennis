using System.Text.Json;
using Kennis.Builder.Constants;

namespace Builder.Domain.Models
{
    public class Project
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string DefaultLanguage { get; set; }
        public byte Pagination { get; set; }
        public string Template { get; set; }
        public string GoogleAnalyticTrackingId { get; set; }
        public IEnumerable<ProjectLanguage> Languages { get; set; }
        public IEnumerable<ProjectSite> Sites { get; set; }

        public static Project Get(string projectName)
        {
            var app = AppContext.BaseDirectory;
            var fileName = Path.Combine(app, LocalEnvironment.Folder.Projects, projectName, LocalEnvironment.File.Project);

            if (File.Exists(fileName))
            {
                string jsonString = File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<Project>(jsonString)!;
            }

            return null;
        }
    }
}
