using Builder.Domain.Layouts;
using Builder.Domain.Wrappers;

namespace Builder.Tests.Domain
{
	public class LayoutBaseTests
   {
      private readonly Mock<IFileWrapper> _fileMock;
		private readonly ILayoutBase _layoutBase;

		private readonly string _JsonExtension = ".json";
      private readonly string _HtmlExtension = ".html";
      private readonly string _templateHtmlFile = "HTML Code";
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
				""Page"": null,
				""Post"": [
					{
						""Order"": 1,
						""FileName"": ""index.html"",
						""ProcessOnlyOnce"": false
					}
				],
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

      public LayoutBaseTests() { 
         _fileMock= new Mock<IFileWrapper>();
         _layoutBase = new LayoutBase(_fileMock.Object);
      }

      [Fact]
      public void Get_ReceiveJsonFileWithListOfLanguages_ShouldReturnLanguages()
		{
			MockDeserializeJsonFile();

         _layoutBase.Get(_JsonExtension);

         Assert.Equal(2, _layoutBase.Languages.Count());
			Assert.Equal("en", _layoutBase.Languages.First());
			Assert.Equal("pt-br", _layoutBase.Languages.Last());
		}

      [Fact]
      public void Get_ReceiveJsonFileWithoutLayoutTemplatePreprocessed_ShouldNotReturnLayoutTemplatePreprocessedList()
      {
         MockDeserializeJsonFile();
			MockLoadFromFile();

         _layoutBase.Get(_JsonExtension);

         Assert.NotNull(_layoutBase.Index.Template);
         Assert.Null(_layoutBase.Index.TemplatesPreprocessed);
      }

      [Fact]
      public void Get_ReceiveJsonFileWithLayoutTemplatePreprocessed_ShouldReturnLayoutTemplatePreprocessedList()
      {
         MockDeserializeJsonFile();
         MockLoadFromFile();

         _layoutBase.Get(_JsonExtension);

         Assert.Single(_layoutBase.Blog.TemplatesPreprocessed);
         Assert.Contains(_layoutBase.Blog.TemplatesPreprocessed.First().Id.ToString(), _layoutBase.Blog.Template);
      }

      [Fact]
      public void Get_ReceiveJsonFileWithEmptyLayoutTemplateAttribuite_ShouldReturnNullForThisAttribute()
      {
         MockDeserializeJsonFile();
         MockLoadFromFile();

         _layoutBase.Get(_JsonExtension);

			Assert.Null(_layoutBase.Page);
		}


      [Fact]
      public void Get_ReceiveJsonFileWithLayoutTemplateAttribuiteWithOnlyOneFile_ShouldPaserItRight()
      {
         MockDeserializeJsonFile();
         MockLoadFromFile();

         _layoutBase.Get(_JsonExtension);

         Assert.NotNull(_layoutBase.Post);
         Assert.Null(_layoutBase.Post.TemplatesPreprocessed);
         Assert.Equal(_templateHtmlFile, _layoutBase.Post.Template);
      }

      private void MockDeserializeJsonFile()
		{
         _fileMock.Setup(x => x.Exists(It.Is<string>(s => s.Contains(_JsonExtension)))).Returns(true);
			_fileMock.Setup(x => x.ReadAllText(It.Is<string>(s => s.Contains(_JsonExtension)))).Returns(_templateJsonFile);
		}

      private void MockLoadFromFile()
      {
         _fileMock.Setup(x => x.Exists(It.Is<string>(s => s.Contains(_HtmlExtension)))).Returns(true);
         _fileMock.Setup(x => x.ReadAllText(It.Is<string>(s => s.Contains(_HtmlExtension)))).Returns(_templateHtmlFile);
      }
   }
}
