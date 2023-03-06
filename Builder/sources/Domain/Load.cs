using Builder.Domain.Models;
using Builder.Domain.Wrappers;
using Kennis.Builder.Constants;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using Myce.Extensions;
using System.Text.Encodings.Web;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Builder.Domain
{
    public interface ILoad
   {
      ContentHeader ContentHeader(string yaml);
      List<Content> ContentList(string path);
      Layout Layout(string templateFolder);
      Project Project(string projectName);
      string YamlHeader(string filename);
   }

   public class Load : ILoad
   {
      private readonly IFileWrapper _file;
      private readonly ILogger<Build> _logger;

      public Load(IFileWrapper fileWrapper,
         ILogger<Build> logger)
      {
         _file = fileWrapper;
         _logger = logger;
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

      public List<Content> ContentList(string path)
      {
         var filename = Path.Combine(path, Const.File.ContentList);
         var list = ReadJsonFile<List<Content>>(filename);

         return list.IsNotNull() ? list : new List<Content>();
      }

      #region Load Layout
      public Layout Layout(string templateFolder)
      {
         var filename = Path.Combine(templateFolder, Const.File.Template);
         var template = ReadJsonFile<Template>(filename);

         var layout = new Layout
         {
            Languages = template.Languages
         };

         LoadLayoutMainTemplates(layout, template, templateFolder);

         layout.Loops = LoadLayoutLoopTemplates(template.Loops, templateFolder);

         return layout;
      }

      private void LoadLayoutMainTemplates(Layout layout, Template template, string folder)
      {
         layout.Index = ReadHtmlFile(folder, template.Index);
         layout.Page = ReadHtmlFile(folder, template.Page);
         layout.Blog = ReadHtmlFile(folder, template.Blog);
         layout.BlogArchive = ReadHtmlFile(folder, template.BlogArchive);
         layout.BlogCategories = ReadHtmlFile(folder, template.BlogCategories);
         layout.BlogPost = ReadHtmlFile(folder, template.BlogPost);
         layout.BlogTags = ReadHtmlFile(folder, template.BlogTags);
      }

      private LayoutLoop LoadLayoutLoopTemplates(TemplateLoop loops, string folder)
      {
         if (loops.IsNull())
         {
            return null;
         }
         var layoutLoop = new LayoutLoop
         {
            BlogArchive = ReadHtmlFile(folder, loops.BlogArchive),
            BlogCategories = ReadHtmlFile(folder, loops.BlogCategories),
            BlogPostLast10 = ReadHtmlFile(folder, loops.BlogPostLast10),
            BlogPostLast5 = ReadHtmlFile(folder, loops.BlogPostLast5),
            BlogPostLast3 = ReadHtmlFile(folder, loops.BlogPostLast3),
            BlogPosts = ReadHtmlFile(folder, loops.BlogPosts),
            BlogTags = ReadHtmlFile(folder, loops.BlogTags),
            Languages = ReadHtmlFile(folder, loops.Languages),
            Menu = ReadHtmlFile(folder, loops.Menu),
            SocialMedia = ReadHtmlFile(folder, loops.SocialMedia)
         };
         return layoutLoop;
      }
      #endregion

      #region Load Project
      public Project Project(string projectName)
      {
         var projectPath = GetProjectPath(projectName);

         var fileName = Path.Combine(projectPath, Const.File.Project);

         var project = ReadJsonFile<Project>(fileName);

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

         return Path.Combine(applicationPath, Const.Folder.Projects, projectName);
      }

      private static ProjectFolder GetProjectFolders(string projectName, string templateName)
      {
         var applicationPath = AppContext.BaseDirectory;

         string projectPath = GetProjectPath(projectName);
         string template = Path.Combine(applicationPath, Const.Folder.Templates, templateName);

         return new ProjectFolder
         {
            Application = applicationPath,
            Project = projectPath,
            Template = template,
            TemplateTranslations = Path.Combine(template, Const.Folder.TemplatesTranslations),
            Destination = Path.Combine(applicationPath, Const.Folder.Sites, projectName)
         };
      }
      #endregion

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

      #region File Read
      private string ReadHtmlFile(string folder, string filename)
      {
         if (!string.IsNullOrEmpty(filename))
         {
            filename = Path.Combine(folder, filename);
            return _file.ReadAllText(filename);
         }

         return null;
      }

      private T ReadJsonFile<T>(string filename)
      {
         if (_file.Exists(filename))
         {
            string jsonString = _file.ReadAllText(filename);
            return JsonSerializer.Deserialize<T>(jsonString)!;
         }

         return default(T);
      }
      #endregion
   }
}
