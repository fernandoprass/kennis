namespace Builder.Domain.Wrappers
{
   public interface IDirectoryWrapper
   {
      string[] GetFiles(string path, string seacrhPattern, SearchOption searchOption);
   }

   public class DirectoryWrapper : IDirectoryWrapper
   {
      public string[] GetFiles(string path, string seacrhPattern, SearchOption searchOption)
      {
         return Directory.GetFiles(path, seacrhPattern, searchOption);
      }  
   }
}
