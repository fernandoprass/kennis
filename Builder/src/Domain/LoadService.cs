using Builder.Domain.Models;
using Kennis.Builder.Constants;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using Myce.Extensions;
using Myce.Wrappers.Contracts;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Builder.Domain {
   public interface ILoadService {
      void ConfigureFolder(ProjectFolder projectFolder);
      ContentHeader ContentHeader(string yaml);
      List<Content> ContentList(string path);
      Template Template();
      Project Project(string filename);
      string YamlContentHeader(string filename);
   }

   public class LoadService : ILoadService {
      private readonly IFileWrapper _file;
      private readonly IPathWrapper _path;
      private readonly ILogger<BuilderService> _logger;

      private ProjectFolder _projectFolder;

      public LoadService(IFileWrapper fileWrapper,
         ILogger<BuilderService> logger,
         IPathWrapper pathWrapper)
      {
         _file = fileWrapper;
         _logger = logger;
         _path = pathWrapper;
      }

      public void ConfigureFolder(ProjectFolder projectFolder)
      {
         _projectFolder = projectFolder;
      }

      public ContentHeader ContentHeader(string yaml)
      {
         try
         {
            return ReadYamlFile<ContentHeader>(yaml);
         }
         catch (Exception ex)
         {
            _logger.LogError(ex, "Falling when try to read content header. Yaml {yaml}", yaml);
         }

         return null;
      }

      public List<Content> ContentList(string path)
      {
         var filename = _path.Combine(path, Const.File.ContentList);

         var list = ReadJsonFile<List<Content>>(filename);

         return list.IsNotNull() ? list : new List<Content>();
      }

      #region Load Layout
      public Template Template()
      {
         var filename = _path.Combine(_projectFolder.Template, Const.File.Template);

         _logger.LogInformation("Load templates at {template}", _projectFolder.Template);

         var templateFile = ReadJsonFile<Template>(filename);

         if (templateFile.IsNotNull())
         {
            var template = LoadLayoutMainTemplates(templateFile, _projectFolder.Template);

            template.Loops = LoadLayoutLoopTemplates(templateFile.Loops, _projectFolder.Template);

            return template;
         }
         else
         {
            _logger.LogError("Falling when try to load template {filename}", filename);
         }

         return null;
      }

      private Template LoadLayoutMainTemplates(Template template, string folder)
      {
         var layout = new Template
         {
            Index = ReadTextFile(folder, template.Index),
            Page = ReadTextFile(folder, template.Page),
            Blog = ReadTextFile(folder, template.Blog),
            BlogArchive = ReadTextFile(folder, template.BlogArchive),
            BlogCategories = ReadTextFile(folder, template.BlogCategories),
            BlogPost = ReadTextFile(folder, template.BlogPost),
            BlogTags = ReadTextFile(folder, template.BlogTags)
         };

         return layout;
      }

      private TemplateLoop LoadLayoutLoopTemplates(TemplateLoop loops, string folder)
      {
         if (loops.IsNull())
         {
            return null;
         }

         var templateLoop = new TemplateLoop
         {
            BlogArchive = ReadTextFile(folder, loops.BlogArchive),
            BlogCategories = ReadTextFile(folder, loops.BlogCategories),
            BlogPostLast10 = ReadTextFile(folder, loops.BlogPostLast10),
            BlogPostLast5 = ReadTextFile(folder, loops.BlogPostLast5),
            BlogPostLast3 = ReadTextFile(folder, loops.BlogPostLast3),
            BlogPosts = ReadTextFile(folder, loops.BlogPosts),
            BlogTags = ReadTextFile(folder, loops.BlogTags),
            Languages = ReadTextFile(folder, loops.Languages),
            Menu = ReadTextFile(folder, loops.Menu),
            SocialMedia = ReadTextFile(folder, loops.SocialMedia)
         };

         return templateLoop;
      }
      #endregion


      public Project Project(string filename)
      {
         return ReadJsonFile<Project>(filename);
      }

      public string YamlContentHeader(string filename)
      {
         try
         {
            var pipeline = new MarkdownPipelineBuilder()
                .UseYamlFrontMatter()
                .Build();

            var mdFile = ReadTextFile(string.Empty, filename);
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
      private string ReadTextFile(string folder, string filename)
      {
         if (!string.IsNullOrEmpty(filename))
         {
            try
            {
               filename = _path.Combine(folder, filename);
               return _file.ReadAllText(filename);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Falling when try read file {0} at {1}", filename, folder);
            }
         }

         return null;
      }

      private T ReadJsonFile<T>(string filename)
      {
         if (_file.Exists(filename))
         {
            string jsonString = string.Empty;
            try
            {
               jsonString = ReadTextFile(string.Empty, filename);

               return JsonSerializer.Deserialize<T>(jsonString)!;
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Falling when try deserialize JSON file. Content {jsonString}", jsonString);
            }
         }

         return default(T);
      }

      private T? ReadYamlFile<T>(string yaml)
      {
         if (yaml.IsNotNull())
         {
            try
            {
               var deserializer = new DeserializerBuilder()
                                      .WithNamingConvention(LowerCaseNamingConvention.Instance)
                                      .Build();

               return deserializer.Deserialize<T>(yaml);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex, "Falling when try deserialize YAML file. Content {yaml}", yaml);
            }
         }

         return default;
      }
      #endregion
   }
}
