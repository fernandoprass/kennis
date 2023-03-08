using Builder.Domain;
using Builder.Domain.Models;
using Builder.Domain.Wrappers;
using Microsoft.Extensions.Logging;

namespace Builder.Tests.Domain
{
   public class LoadTests
   {
      private readonly Mock<IFileWrapper> _fileMock;
      private readonly Mock<ILogger<Build>> _loggerMock;
      private readonly ILoad _load;

		private readonly string _jsonExtension = ".json";
      private readonly string _htmlExtension = ".html";
      private readonly string _templateHtmlFile = "HTML Code";


      public LoadTests() { 
         _fileMock= new Mock<IFileWrapper>();
         _loggerMock= new Mock<ILogger<Build>>();
         _load = new Load(_fileMock.Object, _loggerMock.Object);
      }

      #region Load Layout Tests
      [Fact]
      public void Layout_ReceiveJsonFileWithListOfLanguages_ShouldReturnLanguages()
		{
         var layout = GetLayoutFromMockData();

         Assert.Equal(2, layout.Languages.Count());
			Assert.Equal("en", layout.Languages.First());
			Assert.Equal("pt-br", layout.Languages.Last());
		}

      [Fact]
      public void Layout_ReceiveJsonFileWithEmptyTemplateAttribuite_ShouldReturnNullForThisAttribute()
      {
         var layout = GetLayoutFromMockData();

         Assert.Null(layout.Page);
		}

      [Fact]
      public void Layout_ReceiveJsonFileWithTemplateAttribuite_ShouldReturnThisAttribute()
      {
         var layout = GetLayoutFromMockData();

         Assert.NotNull(layout.Index);
         Assert.Equal(_templateHtmlFile, layout.Index);
      }

      [Fact]
      public void Layout_ReceiveJsonFileWithEmptyLoopAttribuite_ShouldReturnNullForThisAttribute()
      {
         var layout = GetLayoutFromMockData();

         Assert.NotNull(layout.Loops.BlogArchive);
         Assert.Equal(_templateHtmlFile, layout.Loops.BlogArchive);
      }

      [Fact]
      public void Layout_ReceiveJsonFileWithLoopAttribuite_ShouldReturnThisAttribute()
      {
         var layout = GetLayoutFromMockData();

         Assert.NotNull(layout.Loops);
         Assert.Equal(_templateHtmlFile, layout.Index);
      }

      private Layout GetLayoutFromMockData()
      {
         MockLoadTextFile(_jsonExtension, LoadTestsMockData.TemplateJsonFile());

         MockLoadTextFile(_htmlExtension, _templateHtmlFile);

         return _load.Layout(_jsonExtension);
      }
      #endregion

      #region ContentList Tests
      [Fact]
      public void ContentList_ReceivePathForNonExistingJsonFile_ShouldReturnEmptyList()
      {
         var filename = "content_file_name";

         _fileMock.Setup(x => x.Exists(filename)).Returns(false);

         var contentList = _load.ContentList(filename);

         Assert.Empty(contentList);
      }

      [Fact]
      public void ContentList_ReceivePathForExistingJsonFile_ShouldReturnListOfContent()
      {
         MockLoadTextFile(_jsonExtension, LoadTestsMockData.ContentListJsonFile());

         var contentList = _load.ContentList("filename");

         Assert.Equal(3, contentList.Count());
         Assert.Equal(1, contentList.Count(x => x.Type == ContentType.Page));
         Assert.Equal(2, contentList.Count(x => x.Type == ContentType.Post));
      }
      #endregion

      #region ContentHeader Load
      [Fact]
      public void ContentHeader_ReceivePathForExistingJsonFile_ShouldReturnListOfContent()
      {
         var contentHeader = _load.ContentHeader(LoadTestsMockData.ContentHeaderYamlFile());
         Assert.Equal("content title", contentHeader.Title);
         Assert.Equal("content description", contentHeader.Description);
         Assert.Single(contentHeader.Categories);
         Assert.Equal(2, contentHeader.Tags.Count());

         Assert.Null(contentHeader.Icon);
      }
      #endregion


      private void MockLoadTextFile(string extension, string jsonFile)
      {
         //Mock Deserialize Template JsonFile
         _fileMock.Setup(x => x.Exists(It.Is<string>(s => s.Contains(extension)))).Returns(true);
         _fileMock.Setup(x => x.ReadAllText(It.Is<string>(s => s.Contains(extension)))).Returns(jsonFile);
      }
   }
}
