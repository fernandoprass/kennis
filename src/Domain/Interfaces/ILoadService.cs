using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces;

public interface ILoadService
{
   void Configure(ProjectFolder projectFolder);
   Task<string[]> ContentFileListAsync(string contentBasePath);
   ContentHeader ContentHeader(string yaml);
   Task<List<Content>> ContentListAsync(string path);
   Task<AppSettings> AppSettingsAsync();
   Task<Template> TemplateAsync(string name);
   Task<Dictionary<string, string>> TemplateTranslationDataAsync(string language);
   Task<Project> ProjectAsync(string filename);
   Task<string> YamlContentHeaderAsync(string filename);
}
