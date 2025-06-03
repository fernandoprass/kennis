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
      BuildStarting,
      BuildFinished,
      ContentNotFound,
      ContentDeserializeFailed,
      FileDeserializeFailed,
      FileNotFound,
      FileSaveFailed,
      FileSaveSuccessfully,
      FileReadFailed,
      LoadStarting,
      LoadFinishedFailed,
      LoadFinishedSuccessfully,
      ParseStarting,
      ParseFinished,
   }
}
