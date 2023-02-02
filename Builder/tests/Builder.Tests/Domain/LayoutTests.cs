using Builder.Domain.Wrappers;
using Kennis.Builder.Constants;
using Kennis.Builder.Domain;
using System.Linq;

namespace Builder.Tests.Domain
{
   public class LayoutTests
   {
      private readonly Mock<IFileWrapper> _fileMock;
		private readonly ILayout _layout;

		private readonly string _JsonExtension = ".json";
      private readonly string _HtmlExtension = ".html";
      private readonly string _template = "HTML Code";
      private readonly string _templateJsonFile = @"{
				""Languages"": [ ""en"", ""pt-br"" ],
				""Index"": [
					{
						""Order"": 1,
						""FileName"": ""index.html"",
						""ProcessOnlyOnce"": false
					},
					{
						""Order"": 2,
						""FileName"": ""index.footer.html"",
						""ProcessOnlyOnce"": false
					}
				],
				""Blog"": [
					{
						""Order"": 1,
						""FileName"": ""blog.html"",
						""ProcessOnlyOnce"": false
					},
					{
						""Order"": 2,
						""FileName"": ""blog.footer.html"",
						""ProcessOnlyOnce"": true
					}
				],
				""Post"": null,
				""Loops"": {
					""BlogArchive"": ""loop.blog.archive.html"",
					""BlogCategories"": ""loop.blog.categories.html"",
					""BlogPostLast10"": ""loop.blog.posts.last10.html"",
					""BlogPostLast5"": null,
					""BlogPostLast3"": null,
					""BlogPosts"": null,
					""BlogTags"": ""loop.blog.tags.html"",
					""Menu"": ""loop.menu.html"",
					""SocialMedia"": ""loop.socialMedia.html""
				}
			}";

      public LayoutTests() { 
         _fileMock= new Mock<IFileWrapper>();
         _layout = new Layout(_fileMock.Object);
      }

      [Fact]
      public void Get_ReceiveJsonFileWithListOfLanguages_ShouldReturnLanguages()
		{
			MockDeserializeJsonFile();

         _layout.Get(_JsonExtension);

         Assert.Equal(2, _layout.Languages.Count());
			Assert.Equal("en", _layout.Languages.First());
			Assert.Equal("pt-br", _layout.Languages.Last());
		}

      [Fact]
      public void Get_ReceiveJsonFileWithoutLayoutTemplatePreprocessed_ShouldNotReturnLayoutTemplatePreprocessedList()
      {
         MockDeserializeJsonFile();
			MockLoadFromFile();

         _layout.Get(_JsonExtension);

         Assert.NotNull(_layout.Index.Template);
         Assert.Null(_layout.Index.TemplatesPreprocessed);
      }

      [Fact]
      public void Get_ReceiveJsonFileWithLayoutTemplatePreprocessed_ShouldReturnLayoutTemplatePreprocessedList()
      {
         MockDeserializeJsonFile();
         MockLoadFromFile();

         _layout.Get(_JsonExtension);

         Assert.Equal(1, _layout.Blog.TemplatesPreprocessed.Count());
         Assert.Contains(_layout.Blog.TemplatesPreprocessed.First().Id.ToString(), _layout.Blog.Template);
      }

      private void MockDeserializeJsonFile()
		{
         _fileMock.Setup(x => x.Exists(It.Is<string>(s => s.Contains(_JsonExtension)))).Returns(true);
			_fileMock.Setup(x => x.ReadAllText(It.Is<string>(s => s.Contains(_JsonExtension)))).Returns(_templateJsonFile);
		}

      private void MockLoadFromFile()
      {
         _fileMock.Setup(x => x.Exists(It.Is<string>(s => s.Contains(_HtmlExtension)))).Returns(true);
         _fileMock.Setup(x => x.ReadAllText(It.Is<string>(s => s.Contains(_HtmlExtension)))).Returns(_template);
      }
   }
}
