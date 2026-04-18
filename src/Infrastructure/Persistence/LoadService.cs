using Kennis.Domain;
using Kennis.Domain.Interfaces;
using Kennis.Domain.Models;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Myce.Extensions;
using Myce.Wrappers.Contracts;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kennis.Infrastructure.Persistence;

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

   public async Task<List<Content>> ContentListAsync(string path)
   {
      var filename = _pathWrapper.Combine(path, Const.File.ContentList);

      if (!FileExists(filename))
      {
         _logService.LogWarning(LogCategory.Content, LogAction.FileMissing, filename);
         return new List<Content>();
      }

      return await ReadJsonFileAsync<List<Content>>(filename);
   }

   public async Task<string[]> ContentFileListAsync(string contentBasePath)
   {
      var criteria = "*" + Const.Extension.Content;
      var fileList = await Task.Run(() => _directoryWrapper.GetFiles(contentBasePath, criteria, SearchOption.AllDirectories));
      return fileList;
   }

   #region Load Template
   public async Task<Template> TemplateAsync(string name)
   {
      var filename = _pathWrapper.Combine(_projectFolder.Template, Const.File.Template);

      _logService.LogInfo(LogCategory.Template, LogAction.LoadStart, name);

      var templateFile = await ReadJsonFileAsync<Template>(filename);

      if (templateFile != null )
      {
         var template = await LoadMainTemplatesAsync(templateFile, _projectFolder.Template);

         template.Pages.Loops = await LoadLoopTemplatesAsync(templateFile.Pages.Loops, _projectFolder.Template);

         _logService.LogInfo(LogCategory.Template, LogAction.LoadFinishedSuccess);

         return template;
      }
      else
      {
         _logService.LogCritical(LogCategory.Template, LogAction.LoadFinishedFail, name);
      }

      return null;
   }

   private async Task<Template> LoadMainTemplatesAsync(Template template, string folder)
   {
      var layout = new Template
      {
         Assets = template.Assets,
         Languages = template.Languages,
         Pages = new TemplatePages
         {
            Index = await ReadTextFileAsync(folder, template.Pages.Index),
            Page = await ReadTextFileAsync(folder, template.Pages.Page),
            Blog = await ReadTextFileAsync(folder, template.Pages.Blog),
            //BlogArchive = await ReadTextFileAsync(folder, template.Pages.BlogArchive),
            //BlogCategories = await ReadTextFileAsync(folder, template.Pages.BlogCategories),
            BlogPost = await ReadTextFileAsync(folder, template.Pages.BlogPost),
            //BlogTags = await ReadTextFileAsync(folder, template.BlogTags),
         }
      };

      return layout;
   }

   private async Task<TemplatePagesLoops> LoadLoopTemplatesAsync(TemplatePagesLoops loops, string folder)
   {
      if (loops == null )
      {
         return null;
      }

      var templateLoop = new TemplatePagesLoops
      {
         BlogArchive = await ReadTextFileAsync(folder, loops.BlogArchive),
         BlogCategories = await ReadTextFileAsync(folder, loops.BlogCategories),
         BlogPosts = await ReadTextFileAsync(folder, loops.BlogPosts),
         BlogTags = await ReadTextFileAsync(folder, loops.BlogTags),
         Languages = await ReadTextFileAsync(folder, loops.Languages),
         Menu = await ReadTextFileAsync(folder, loops.Menu),
         SocialMedia = await ReadTextFileAsync(folder, loops.SocialMedia)
      };

      return templateLoop;
   }

   public async Task<Dictionary<string, string>> TemplateTranslationDataAsync(string language)
   {
      _logService.LogInfo(LogCategory.TranslationFile, LogAction.LoadStart, language);
      var filename = _pathWrapper.Combine(_projectFolder.TemplateTranslations, $"{language}{Const.Extension.I18n}");

      var i18nData = await ReadJsonFileAsync<Dictionary<string, string>>(filename)!;

      if (i18nData == null )
      {
         _logService.LogInfo(LogCategory.TranslationFile, LogAction.LoadFinishedFail, language);
         return null;
      }

      _logService.LogInfo(LogCategory.TranslationFile, LogAction.LoadFinishedSuccess, language);

      return i18nData;
   }
   #endregion


   public async Task<Dictionary<string, Dictionary<string, string>>> LogMessagesAsync(string language)
   {
      var filename = _pathWrapper.Combine(Const.Folder.LogMessages, $"{language}{Const.Extension.I18n}");
      return await ReadJsonFileAsync<Dictionary<string, Dictionary<string, string>>>(filename);
   }

   public async Task<Project> ProjectAsync(string filename)
   {
      return await ReadJsonFileAsync<Project>(filename);
   }

   public async Task<string> YamlContentHeaderAsync(string filename)
   {
      try
      {
         var pipeline = new MarkdownPipelineBuilder()
             .UseYamlFrontMatter()
             .Build();

         var mdFile = await ReadTextFileAsync(string.Empty, filename);
         if (mdFile != null )
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

   private async Task<string> ReadTextFileAsync(string folder, string filename)
   {
      if (!string.IsNullOrEmpty(filename))
      {
         try
         {
            string _filename = _pathWrapper.Combine(folder, filename);
            if (FileExists(_filename))
            {
               var content = await _fileWrapper.ReadAllTextAsync(_filename);
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

   private async Task<T> ReadJsonFileAsync<T>(string filename)
   {
      string json = string.Empty;
      try
      {
         json = await ReadTextFileAsync(string.Empty, filename);

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
      if (yaml != null )
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
