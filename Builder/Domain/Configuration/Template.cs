using System.Text.Json;

namespace Builder.Domain.Configuration
{
    public class Template
    {
        public IEnumerable<string> Languages { get; set; }
        public IEnumerable<TemplateHtmlFile> Index { get; set; }
        public IEnumerable<TemplateHtmlFile> Blog { get; set; }
        public IEnumerable<TemplateHtmlFile> Post { get; set; }
        public TemplateLoopHtmlFile Loops { get; set; }

        public static Template Read(string fileName)
        {
            if (File.Exists(fileName))
            {
                string jsonString = File.ReadAllText(fileName);
                var templateConfig = JsonSerializer.Deserialize<Template>(jsonString)!;
                return templateConfig;
            }

            return null;
        }
    }
}