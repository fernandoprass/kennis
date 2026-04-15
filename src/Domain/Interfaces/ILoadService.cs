using Kennis.Domain.Models;

namespace Kennis.Domain.Interfaces;

public interface ILoadService
{
   void Configure(ProjectFolder projectFolder);
   string[] ContentFileList(string contentBasePath);
   ContentHeader ContentHeader(string yaml);
   List<Content> ContentList(string path);
   Dictionary<string, Dictionary<string, string>> LogMessages(string language);
   Template Template(string name);
   Dictionary<string, string> TemplateTranslationData(string language);
   Project Project(string filename);
   string YamlContentHeader(string filename);
}
