namespace Kennis
{
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
      BuildFinished,
      ContentNotFound,
      ContentDeserializeFail,
      DataMissing,
      DeserializeFail,
      DeserializeSuccess,
      FileCopy,
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
}
