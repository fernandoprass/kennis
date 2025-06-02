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
      LoadStarting,
      LoadFinishedFailed,
      LoadFinishedSuccessfully,
      DeserializeFailed,
      SaveFailed,
      SaveSuccessfully,
      ParseStarting,
      ParseFinished
   }
}
