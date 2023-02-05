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
				""Index"": ""index.html"",
				""Page"": null,
				""Blog"": null,
				""BlogArchive"": null,
				""BlogCategories"": null,
				""BlogPost"": null,
				""BlogTags"": null,
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
      public void Get_ReceiveJsonFileWithEmptyTemplateAttribuite_ShouldReturnNullForThisAttribute()
      {
         MockDeserializeJsonFile();
         MockLoadFromFile();

         _layoutBase.Get(_JsonExtension);

			Assert.Null(_layoutBase.Page);
		}

      [Fact]
      public void Get_ReceiveJsonFileWithTemplateAttribuite_ShouldReturnThisAttribute()
      {
         MockDeserializeJsonFile();
         MockLoadFromFile();

         _layoutBase.Get(_JsonExtension);

         Assert.NotNull(_layoutBase.Index);
         Assert.Equal(_templateHtmlFile, _layoutBase.Index);
      }

      [Fact]
      public void Get_ReceiveJsonFileWithEmptyLoopAttribuite_ShouldReturnNullForThisAttribute()
      {
         MockDeserializeJsonFile();
         MockLoadFromFile();

         _layoutBase.Get(_JsonExtension);

         Assert.NotNull(_layoutBase.Loops.BlogArchive);
         Assert.Equal(_templateHtmlFile, _layoutBase.Loops.BlogArchive);
      }

      [Fact]
      public void Get_ReceiveJsonFileWithLoopAttribuite_ShouldReturnThisAttribute()
      {
         MockDeserializeJsonFile();
         MockLoadFromFile();

         _layoutBase.Get(_JsonExtension);

         Assert.NotNull(_layoutBase.Loops);
         Assert.Equal(_templateHtmlFile, _layoutBase.Index);
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
