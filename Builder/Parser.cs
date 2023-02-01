namespace Kennis.Builder
{
   public static class Parser
    {
        public static string Do()
        {
            string templateFile = @"index.html";
            string contentFile = @"content.html";


            string data = File.ReadAllText(templateFile);
            string content = File.ReadAllText(contentFile);

            string title = "This is the title";


            string author = "Fernando Prass";


            data = data.Replace("{@page.title}", title);
            data = data.Replace("{@page.content}", content);
            data = data.Replace("{@page.meta.author}", author);

            File.WriteAllText(@"index_new.html", data);

            return data;

        }
    }
}
