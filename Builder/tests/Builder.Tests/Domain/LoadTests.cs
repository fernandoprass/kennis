using Builder.Domain;
using Builder.Domain.Models;
using Microsoft.Extensions.Logging;
using Myce.Wrappers.Contracts;

namespace Builder.Tests.Domain
{
   public class LoadTests
   {
      private readonly Mock<IFileWrapper> _fileMock;
      private readonly Mock<ILogger<BuilderService>> _loggerMock;
      private readonly ILoad _load;

		private readonly string _mdExtension = ".md";
      private readonly string _jsonExtension = ".json";
      private readonly string _htmlExtension = ".html";
      private readonly string _templateHtmlFile = "HTML Code";


      public LoadTests() { 
         _fileMock= new Mock<IFileWrapper>();
         _loggerMock= new Mock<ILogger<BuilderService>>();
       //  _load = new Load(_fileMock.Object, _loggerMock.Object);
      }

      #region Load Layout Tests
      [Fact]
      public void Layout_ReceiveJsonFileWithEmptyTemplateAttribuite_ShouldReturnNullForThisAttribute()
      {
         var template = GetLayoutFromMockData();

         Assert.Null(template.Page);
		}

      [Fact]
      public void Layout_ReceiveJsonFileWithTemplateAttribuite_ShouldReturnThisAttribute()
      {
         var template = GetLayoutFromMockData();

         Assert.NotNull(template.Index);
         Assert.Equal(_templateHtmlFile, template.Index);
      }

      [Fact]
      public void Layout_ReceiveJsonFileWithEmptyLoopAttribuite_ShouldReturnNullForThisAttribute()
      {
         var template = GetLayoutFromMockData();

         Assert.NotNull(template.Loops.BlogArchive);
         Assert.Equal(_templateHtmlFile, template.Loops.BlogArchive);
      }

      [Fact]
      public void Layout_ReceiveJsonFileWithLoopAttribuite_ShouldReturnThisAttribute()
      {
         var template = GetLayoutFromMockData();

         Assert.NotNull(template.Loops);
         Assert.Equal(_templateHtmlFile, template.Index);
      }

      private Template GetLayoutFromMockData()
      {
         MockLoadTextFile(_jsonExtension, LoadTestsMockData.TemplateJsonFile());

         MockLoadTextFile(_htmlExtension, _templateHtmlFile);

         return _load.Template(_jsonExtension);
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
      public void ContentHeader_ReceiveValidYamlFile_ShouldParse()
      {
         var contentHeader = _load.ContentHeader(LoadTestsMockData.ContentHeaderYamlFile());

         Assert.Equal("content title", contentHeader.Title);
         Assert.Equal("content description", contentHeader.Description);
         Assert.Equal("2023-01-02 15:05:00", contentHeader.Created.ToString("yyyy-MM-dd HH:mm:ss"));
         Assert.Equal("hello-world", contentHeader.Reference);
         Assert.False(contentHeader.Draft);
         Assert.Single(contentHeader.Categories);
         Assert.Equal(2, contentHeader.Tags.Count());
         Assert.Null(contentHeader.Updated);
         Assert.Null(contentHeader.Icon);
      }
      #endregion

      #region YamlHeader Load
      [Fact]
      public void YamlHeader_ReceiveMdFileWithValidYamlHeader_ShouldParse()
      {
         MockLoadTextFile(_mdExtension, LoadTestsMockData.MdFileWithYamlHeader());
         var yamlFile = _load.YamlContentHeader(_mdExtension);

         Assert.Equal("field: value", yamlFile);
      }
      #endregion

      private void MockLoadTextFile(string extension, string textFile)
      {
         //Mock read text file
         _fileMock.Setup(x => x.Exists(It.Is<string>(s => s.Contains(extension)))).Returns(true);
         _fileMock.Setup(x => x.ReadAllText(It.Is<string>(s => s.Contains(extension)))).Returns(textFile);
      }
   }
}
