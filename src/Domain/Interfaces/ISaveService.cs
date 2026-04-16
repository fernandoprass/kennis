namespace Kennis.Domain.Interfaces;

public interface ISaveService
{
   void Configure(string htmlFolder, string jsonFolder);
   Task ToJsonFileAsync<T>(string languageCode,string filename, T contentList);
   Task ToHtmlFileAsync(string filename, string webPage);
}
