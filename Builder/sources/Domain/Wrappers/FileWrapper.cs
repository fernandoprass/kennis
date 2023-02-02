namespace Builder.Domain.Wrappers
{
   public interface IFileWrapper
   {
      bool Exists(string path);
      string ReadAllText(string path);
      void WriteAllText(string path, string text);
   }

   public class FileWrapper : IFileWrapper
   {
      public bool Exists(string path)
      {
         return File.Exists(path);
      }
      public string ReadAllText(string path)
      {
         return File.ReadAllText(path);
      }
      public void WriteAllText(string path, string text)
      {
         File.WriteAllText(path, text);
      }
   }
}
