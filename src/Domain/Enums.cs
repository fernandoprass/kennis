namespace Kennis.Domain;

public enum LogCategory
{
   AppSettings,
   Content,
   File,
   Project,
   Site,
   Template,
   HtmlFile,
   JsonFile,
   YamlFile,
   TranslationFile,
   Validator
}

public enum LogAction
{
   BuildStart,
   BuildSuccess,
   ContentNotFound,
   ContentDeserializeFail,
   DataMissing,
   DeserializeFail,
   DeserializeSuccess,
   FileCopy,
   FileCopyFail,
   FileMissing,
   LoadStart,
   LoadFinishedFail,
   LoadFinishedSuccess,
   Message,
   NotSupported,
   ParseStart,
   ParseSuccess,
   ReadFail,
   ReadSuccess,
   RuleBroken,
   SaveFail,
   SaveSuccess,
   TranslateSuccess,
}

enum TemplateEngine
{
   Liquid,
   Scriban
}