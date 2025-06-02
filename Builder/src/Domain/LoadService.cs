using Kennis.Builder.Constants;
using Kennis.Domain.Models;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using Myce.Extensions;
using Myce.Wrappers.Contracts;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kennis.Domain
{
   public interface ILoadService {
      void Configure(ProjectFolder projectFolder);
      string[] ContentFiles(string contentBasePath);
      ContentHeader ContentHeader(string yaml);
      List<Content> ContentList(string path);
      Dictionary<string, Dictionary<string, string>> LogMessages(string language);
      Template Template();
      Dictionary<string, string> TemplateTranslationData(string language);
      Project Project(string filename);
      string YamlContentHeader(string filename);
   }

   public class LoadService : ILoadService {
      private readonly IDirectoryWrapper _directoryWrapper;
      private readonly IFileWrapper _fileWrapper;
      private readonly IPathWrapper _pathWrapper;
      private readonly ILogService _logService;

      private ProjectFolder? _projectFolder;

      public LoadService(IDirectoryWrapper directoryWrapper,
         IFileWrapper fileWrapper,
         ILogService logService,
         IPathWrapper pathWrapper)
      {
         _directoryWrapper = directoryWrapper;
         _fileWrapper = fileWrapper;
         _logService = logService;
         _pathWrapper = pathWrapper; 
      }

      public void Configure(ProjectFolder projectFolder)
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
            _logService.LogError(ex, "Failed to read Content Header. Yaml {yaml}", yaml);
         }

         return null;
      }

      public List<Content> ContentList(string path)
      {
         var filename = _pathWrapper.Combine(path, Const.File.ContentList);

         var list = ReadJsonFile<List<Content>>(filename);

         return list.IsNotNull() ? list : new List<Content>();
      }

      public string[] ContentFiles(string contentBasePath)
      {
         var criteria = "*" + Const.Extension.Content;
         var files = _directoryWrapper.GetFiles(contentBasePath, criteria, SearchOption.AllDirectories);
         return files;
      }

      #region Load Template
      public Template Template()
      {
         var filename = _pathWrapper.Combine(_projectFolder.Template, Const.File.Template);

         _logService.LogInfo(Const.Log.Category.Template, Const.Log.Action.LoadStarting, _projectFolder.Template);

         var templateFile = ReadJsonFile<Template>(filename);

         if (templateFile.IsNotNull())
         {
            var template = LoadMainTemplates(templateFile, _projectFolder.Template);

            template.Loops = LoadLoopTemplates(templateFile.Loops, _projectFolder.Template);

            _logService.LogInfo(Const.Log.Category.Template, Const.Log.Action.LoadStarting);

            return template;
         }
         else
         {
            _logService.LogCritical("Failed to load template {filename}", filename);
         }

         return null;
      }

      private Template LoadMainTemplates(Template template, string folder)
      {
         var layout = new Template
         {
            Index = ReadTextFile(folder, template.Index),
            Page = ReadTextFile(folder, template.Page),
            Blog = ReadTextFile(folder, template.Blog),
            BlogArchive = ReadTextFile(folder, template.BlogArchive),
            BlogCategories = ReadTextFile(folder, template.BlogCategories),
            BlogPost = ReadTextFile(folder, template.BlogPost),
            BlogTags = ReadTextFile(folder, template.BlogTags),
            Languages = template.Languages
         };

         return layout;
      }

      private TemplateLoop LoadLoopTemplates(TemplateLoop loops, string folder)
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

      public Dictionary<string, string> TemplateTranslationData(string language)
      {
         _logService.LogInfo(Const.Log.Category.TranslationFile, Const.Log.Action.LoadStarting, language, _projectFolder.TemplateTranslations);
         var filename = _pathWrapper.Combine(_projectFolder.TemplateTranslations, $"{language}{Const.Extension.I18n}");

         var i18nData = ReadJsonFile<Dictionary<string, string>>(filename)!;

         if (i18nData.IsNotNull())
         {
            _logService.LogInfo(Const.Log.Category.TranslationFile, Const.Log.Action.LoadFinishedSuccessfully, language);
         }

         return i18nData;
      }
      #endregion


      public Dictionary<string, Dictionary<string, string>> LogMessages(string language)
      {
         var filename = _pathWrapper.Combine(Const.Folder.LogMessages, $"{language}{Const.Extension.I18n}");
         return ReadJsonFile<Dictionary<string, Dictionary<string, string>>>(filename);
      }

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
            if (mdFile.IsNotNull())
            {
               var document = Markdown.Parse(mdFile, pipeline);
               var yamlHeader = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
               var yaml = yamlHeader.Lines.ToString();
               return yaml; 
            }

            return null;
         }
         catch (Exception ex)
         {
            _logService.LogError(ex, "Failed to read YAML Content Header file {0}", filename);
            return null;
         }
      }

      #region File Read
      private bool FileExists(string filename)
      {
         if (_fileWrapper.Exists(filename))
         {
            return true;
         }

         _logService.LogError("File not found: {filename}", filename);
         return false;
      }

      private string ReadTextFile(string folder, string filename)
      {
         if (!string.IsNullOrEmpty(filename))
         {
            try
            {
               string _filename = _pathWrapper.Combine(folder, filename);
               if (FileExists(_filename))
               {
                  return _fileWrapper.ReadAllText(_filename);
               }           
            }
            catch (Exception ex)
            {
               _logService.LogError(ex, "Failed to read file {0} at {1}", filename, folder);
            }
         }

         return null;
      }

      private T ReadJsonFile<T>(string filename)
      {
         string json = string.Empty;
         try
         {
            json = ReadTextFile(string.Empty, filename);

            if (!json.IsNullOrEmpty())
            {
               return JsonSerializer.Deserialize<T>(json)!;
            }

         }
         catch (Exception ex)
         {
            _logService.LogError(ex, Const.Log.Category.JsonFile, Const.Log.Action.DeserializeFailed, json);
         }

         return default;
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
               _logService.LogError(ex, Const.Log.Category.YamlFile, Const.Log.Action.DeserializeFailed, yaml);
            }
         }

         return default;
      }
      #endregion
   }
}
