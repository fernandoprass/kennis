namespace Kennis.Domain.Interfaces;

public interface ISaveService
{
   void Configure(string htmlFolder, string jsonFolder);
   void ToJsonFile<T>(string languageCode,string filename, T contentList);
   void ToHtmlFile(string filename, string webPage);
}
