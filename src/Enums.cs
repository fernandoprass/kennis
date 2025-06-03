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
      FileDeserializeSuccessfully,
      FileNotFound,
      FileSaveFailed,
      FileSaveSuccessfully,
      FileReadFailed,
      FileReadSuccessfully,
      LoadStarting,
      LoadFinishedFailed,
      LoadFinishedSuccessfully,
      ParseStarting,
      ParseFinished,
      TranslateFinished,
   }
}
