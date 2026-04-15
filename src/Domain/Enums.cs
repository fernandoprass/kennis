namespace Kennis.Domain;

public enum LogCategory
{
   Content,
   File,
   Project,
   Site,
   Template,
   HtmlFile,
   JsonFile,
   YamlFile,
   TranslationFile
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
   NotSupported,
   ParseStart,
   ParseSuccess,
   ReadFail,
   ReadSuccess,
   SaveFail,
   SaveSuccess,
   TranslateSuccess,
}
