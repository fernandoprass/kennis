using Builder.Domain.Layouts;
using Builder.Domain.Models;
using Builder.Domain.Wrappers;
using Kennis.Builder.Constants;
using Markdig.Extensions.Yaml;
using Markdig;
using Microsoft.Extensions.Logging;
using Myce.Extensions;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Markdig.Syntax;
using System.IO;

namespace Builder.Domain
{
   public interface ILoad
   {
      Project Project(string projectName);
      ILayoutBase LayoutBase(string templateFolder);
      ContentHeader ContentHeader(string yaml);
      List<Content> ContentList(string path, string type);
      string YamlHeader(string filename);

      void SaveContentListToJson(List<Content> contentList, string filename);
   }

   public class Load : ILoad
   {
      private readonly IFileWrapper _file;
      private readonly ILayoutBase _layoutBase;
      private readonly ILogger<Build> _logger;

      public Load(IFileWrapper fileWrapper,
         ILayoutBase layoutBase,
         ILogger<Build> logger)
      {
         _file = fileWrapper;
         _layoutBase = layoutBase;
         _logger = logger;
      }

      public Project Project(string projectName)
      {
         var projectPath = GetProjectPath(projectName);

         var fileName = Path.Combine(projectPath, LocalEnvironment.File.Project);

         var project = ReadJson<Project>(fileName);

         if (project.IsNotNull())
         {
            project.Folders = GetProjectFolders(projectName, project.Template);

            return project;
         }

         return null;
      }

      private static string GetProjectPath(string projectName)
      {
         var applicationPath = AppContext.BaseDirectory;

         return Path.Combine(applicationPath, LocalEnvironment.Folder.Projects, projectName);
      }

      private static ProjectFolder GetProjectFolders(string projectName, string templateName)
      {
         var applicationPath = AppContext.BaseDirectory;

         string projectPath = GetProjectPath(projectName);
         string template = Path.Combine(applicationPath, LocalEnvironment.Folder.Templates, templateName);

         return new ProjectFolder
         {
            Application = applicationPath,
            Project = projectPath,
            Template = template,
            TemplateTranslations = Path.Combine(template, LocalEnvironment.Folder.TemplatesTranslations),
            Destination = Path.Combine(applicationPath, LocalEnvironment.Folder.Sites, projectName)
         };
      }

      public ILayoutBase LayoutBase(string templateFolder)
      {
         var filename = Path.Combine(templateFolder, LocalEnvironment.File.Template);
         var template = ReadJson<Template>(filename);
         _layoutBase.Get(templateFolder, template);
         return _layoutBase;
      }

      private T ReadJson<T>(string filename)
      {
         if (_file.Exists(filename))
         {
            string jsonString = _file.ReadAllText(filename);
            return JsonSerializer.Deserialize<T>(jsonString)!;
         }

         return default(T);
      }

      //todo move to a server specific to sava data
      public void SaveContentListToJson(List<Content> contentList, string filename)
      {
         var options = new JsonSerializerOptions { WriteIndented = true };
         var json = JsonSerializer.Serialize(contentList, options)!;

         File.WriteAllText(filename, json);
      }

      public ContentHeader ContentHeader(string yaml)
      {
         try
         {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(LowerCaseNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<ContentHeader>(yaml);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Falling when try to read content header. Yaml {0}", yaml);
            return null;
         }
      }

      public string YamlHeader(string filename)
      {
         try
         {
            var pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .Build();

            var mdFile = File.ReadAllText(filename);
            var document = Markdown.Parse(mdFile, pipeline);
            var yamlHeader = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
            var yaml = yamlHeader.Lines.ToString();
            return yaml;

         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Falling when try to read file {0}", filename);
            return null;
         }

      }

      public List<Content> ContentList(string path, string file)
      {
         var filename = Path.Combine(path, file);
         var list = ReadJson<List<Content>>(filename);

         return list.IsNotNull() ? list : new List<Content>();
      }
   }
}
