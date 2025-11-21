using Kennis.Builder.Constants;
using Kennis.Domain.Models;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Myce.Extensions;
using Myce.Wrappers.Contracts;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kennis.Domain
{
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

   public class LoadService(IDirectoryWrapper directoryWrapper,
                            IFileWrapper fileWrapper,
                            ILogService logService,
                            IPathWrapper pathWrapper) : ILoadService
   {
      private readonly IDirectoryWrapper _directoryWrapper = directoryWrapper;
      private readonly IFileWrapper _fileWrapper = fileWrapper;
      private readonly IPathWrapper _pathWrapper = pathWrapper;
      private readonly ILogService _logService = logService;

      private ProjectFolder? _projectFolder;

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
            _logService.LogError(ex, LogCategory.Content, LogAction.ReadFail, yaml);
         }

         return null;
      }

      public List<Content> ContentList(string path)
      {
         var filename = _pathWrapper.Combine(path, Const.File.ContentList);

         if (!_fileWrapper.Exists(filename))
         {
            _logService.LogWarning(LogCategory.Content, LogAction.FileMissing, filename);
            return new List<Content>();
         }

         return ReadJsonFile<List<Content>>(filename);
      }

      public string[] ContentFileList(string contentBasePath)
      {
         var criteria = "*" + Const.Extension.Content;
         var fileList = _directoryWrapper.GetFiles(contentBasePath, criteria, SearchOption.AllDirectories);
         return fileList;
      }

      #region Load Template
      public Template Template(string name)
      {
         var filename = _pathWrapper.Combine(_projectFolder.Template, Const.File.Template);

         _logService.LogInfo(LogCategory.Template, LogAction.LoadStart, name);

         var templateFile = ReadJsonFile<Template>(filename);

         if (templateFile.IsNotNull())
         {
            var template = LoadMainTemplates(templateFile, _projectFolder.Template);

            template.Loops = LoadLoopTemplates(templateFile.Loops, _projectFolder.Template);

            _logService.LogInfo(LogCategory.Template, LogAction.LoadFinishedSuccess);

            return template;
         }
         else
         {
            _logService.LogCritical(LogCategory.Template, LogAction.LoadFinishedFail, name);
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
            Assets = template.Assets,
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
         _logService.LogInfo(LogCategory.TranslationFile, LogAction.LoadStart, language);
         var filename = _pathWrapper.Combine(_projectFolder.TemplateTranslations, $"{language}{Const.Extension.I18n}");

         var i18nData = ReadJsonFile<Dictionary<string, string>>(filename)!;

         if (i18nData.IsNull())
         {
            _logService.LogInfo(LogCategory.TranslationFile, LogAction.LoadFinishedFail, language);
            return null;
         }

         _logService.LogInfo(LogCategory.TranslationFile, LogAction.LoadFinishedSuccess, language);

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
            _logService.LogError(ex, LogCategory.YamlFile, LogAction.ReadFail, filename);
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

         _logService.LogError(LogCategory.File, LogAction.FileMissing, filename);
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
                  var content = _fileWrapper.ReadAllText(_filename);
                  _logService.LogTrace(LogCategory.File, LogAction.ReadSuccess, filename);
                  return content;
               }
            }
            catch (Exception ex)
            {
               _logService.LogError(ex, LogCategory.File, LogAction.ReadFail, filename, folder);
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
               var options = new JsonSerializerOptions
               {
                  PropertyNameCaseInsensitive = true
               };

               var content = JsonSerializer.Deserialize<T>(json, options)!;
               _logService.LogTrace(LogCategory.JsonFile, LogAction.DeserializeSuccess, filename);
               return content;
            }

         }
         catch (Exception ex)
         {
            _logService.LogError(ex, LogCategory.JsonFile, LogAction.DeserializeFail, json);
            _logService.LogTrace(LogCategory.JsonFile, LogAction.ContentDeserializeFail, json);
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

               var content = deserializer.Deserialize<T>(yaml);

               return content;
            }
            catch (Exception ex)
            {
               _logService.LogError(ex, LogCategory.YamlFile, LogAction.DeserializeFail, yaml);
            }
         }

         return default;
      }
      #endregion
   }
}
