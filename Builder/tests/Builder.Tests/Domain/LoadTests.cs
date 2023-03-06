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

		private readonly string _JsonExtension = ".json";
      private readonly string _HtmlExtension = ".html";
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
         //Mock Deserialize Template JsonFile
         _fileMock.Setup(x => x.Exists(It.Is<string>(s => s.Contains(_JsonExtension)))).Returns(true);
			_fileMock.Setup(x => x.ReadAllText(It.Is<string>(s => s.Contains(_JsonExtension)))).Returns(LoadTestsMockData.TemplateJsonFile);

         //Mock Load Html File
         _fileMock.Setup(x => x.Exists(It.Is<string>(s => s.Contains(_HtmlExtension)))).Returns(true);
         _fileMock.Setup(x => x.ReadAllText(It.Is<string>(s => s.Contains(_HtmlExtension)))).Returns(_templateHtmlFile);

         return _load.Layout(_JsonExtension);
      }
      #endregion
   }
}
