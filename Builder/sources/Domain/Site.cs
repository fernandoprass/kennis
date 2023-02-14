using Builder.Domain.Contents;
using Kennis.Builder.Constants;
using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Myce.Extensions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Builder.Domain
{
   public interface ISite
   {
      void Load();

   }
   public class Site : ISite
   {
      public string Language { get; set; }
      List<Content> Pages { get; set; }
      List<Content> Posts { get; set; }

      public void Load()
      {
         var path = @"D:\Fernando\Professional\kennis\Builder\sources\projects\KennisDemo\en\";
         var folder = Path.Combine(path, LocalEnvironment.Folder.Posts);
         var criteria = "*" + LocalEnvironment.Extension.Content;
         var files = Directory.GetFiles(folder, criteria, SearchOption.AllDirectories);

         var pipeline = new MarkdownPipelineBuilder()
            .UseYamlFrontMatter()
            .Build();

         Pages = new List<Content>();
         foreach(var filename in files)
         {
            var file = File.ReadAllText(filename);
            var document = Markdown.Parse(file, pipeline);
            var yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
            var yaml = yamlBlock.Lines.ToString();

            var deserializer = new DeserializerBuilder()
               .WithNamingConvention(LowerCaseNamingConvention.Instance)
               .Build();

            var post = deserializer.Deserialize<PostHeader>(yaml);

            post.Slug = GetSlug(post.Title);

            var serializer = new SerializerBuilder()
            .WithNamingConvention(LowerCaseNamingConvention.Instance)
            .Build();

            var yamlFinal = serializer.Serialize(post);
         }
      }

      private string GetSlug(string title)
      {
         var slug = title.ToLower().Replace(" ", "-");
         slug = slug.RemoveAccents();
         return slug;
      }
   }
}
