using Builder.Domain;
using Builder.Domain.Models;
using Microsoft.Extensions.Logging;
using Myce.Wrappers.Contracts;

namespace Builder.Tests.Domain
{
   public class DataTests
   {
      private readonly Mock<IDirectoryWrapper> _directoryWrapper;
      private readonly Mock<ILogger<BuilderService>> _loggerMock;
      private readonly Mock<ILoadService> _loadMock;
      private readonly Mock<ISave> _saveMock;

      private readonly IData _data;

      private readonly string _jsonExtension = ".json";

      public DataTests() {
         _directoryWrapper = new Mock<IDirectoryWrapper>();
         _loggerMock= new Mock<ILogger<BuilderService>>();
         _loadMock = new Mock<ILoadService>();
         _saveMock = new Mock<ISave>();

         _data = new Data(_directoryWrapper.Object, _loadMock.Object, _saveMock.Object, _loggerMock.Object);
      }


      #region GetContentList Tests
      [Fact]
      public void GetContentList_ReceiveTwoNewContentsAndEmptyList_ShouldAddBoth()
      {
         var projectFolder = new ProjectFolder
         {
            Project = @"c:\project"
         };

         MockDataForGetContentListTests(new List<Content>());

         _data.GetContentList(projectFolder, "en", "/pages/", "/posts/");

         Assert.Single(_data.ContentList.Where(x => x.Type == ContentType.Page));
         Assert.Single(_data.ContentList.Where(x => x.Type == ContentType.Post));
      }

      [Fact]
      public void GetContentList_ReceiveTwoNewContentsOneContentInDraftAndEmptyList_ShouldIgnoreTheDraftContent()
      {
         var projectFolder = new ProjectFolder
         {
            Project = @"c:\project"
         };

         MockDataForGetContentListTests(new List<Content>());

         _data.GetContentList(projectFolder, "en", "/pages/", "/posts/");

         Assert.Equal(2, _data.ContentList.Count());
         Assert.Empty(_data.ContentList.Where(x => x. Draft));
      }

      [Fact]
      public void GetContentList_ReceiveTwoNewContentsAndListThatContainsOneOfTheContent_ShouldAddOneAndUpdateAnother()
      {
         var projectFolder = new ProjectFolder
         {
            Project = @"c:\project"
         };

         var contentList = CreateContentList();

         MockDataForGetContentListTests(contentList);

         string oldPageTitle = contentList.Single(x => x.Type == ContentType.Page).Title;

         _data.GetContentList(projectFolder, "en", "/pages/", "/posts/");

         string newPageTitle = _data.ContentList.Single(x => x.Type == ContentType.Page).Title;

         Assert.Equal(2, _data.ContentList.Count());
         Assert.NotEqual(newPageTitle, oldPageTitle);
         Assert.Null(_data.ContentList.First().Categories);
      }

      [Fact]
      public void UpdateContentList_ReceiveListOfContent_ShouldUpdate()
      {
         _data.ContentList = CreateContentList();

         _data.UpdateContentList();

         var post = _data.ContentList.Single(x => x.Type == ContentType.Post);

         Assert.Single(post.Categories);
         Assert.Equal(2, post.Tags.Count());
         Assert.Contains(post.Categories.First(), post.Keywords);
         Assert.Contains(post.Tags.First(), post.Keywords);
         Assert.Contains(post.Tags.Last(), post.Keywords);
      }
      #endregion

      #region SaveContentList Tests
      [Fact]
      public void SaveContentList_ReceiveListOfContent_ShouldSave()
		{
         _data.ContentList = CreateContentList();

         _saveMock.Setup(x => x.ToJsonFile(It.Is<string>(s => s.Contains(_jsonExtension)), _data.ContentList)).Verifiable();

         _data.SaveContentList();

         _saveMock.Verify(x => x.ToJsonFile(It.Is<string>(s => s.Contains(_jsonExtension)), _data.ContentList), Times.Once);
		}
      #endregion

      #region Private Methods
      private void MockDataForGetContentListTests(List<Content> contentList)
      {
         var files = new string[] { @"c:\project\en\pages\page.md", @"c:\project\en\posts\post.md", @"c:\posts\draft.md" };
         var yamlPage = "page content header";
         var yamlPost = "post content header";
         var yamlDraft = "draft content header";

         var pageContentHeader = new ContentHeader
         {
            Title = "My Page",
            Draft = false,
         };

         var postContentHeader = new ContentHeader
         {
            Title = "My Post",
            Categories = new string[] { "cat1" },
            Tags = new string[] { "tag1", "tag2" },
            Draft = false,
         };

         var draftContentHeader = new ContentHeader
         {
            Title = "My Draft Post",
            Draft = true,
         };

         _loadMock.Setup(x => x.ContentList(It.IsAny<string>())).Returns(contentList);

         _loadMock.Setup(x => x.YamlContentHeader(files[0])).Returns(yamlPage);
         _loadMock.Setup(x => x.YamlContentHeader(files[1])).Returns(yamlPost);
         _loadMock.Setup(x => x.YamlContentHeader(files[2])).Returns(yamlDraft);

         _loadMock.Setup(x => x.ContentHeader(yamlPage)).Returns(pageContentHeader);
         _loadMock.Setup(x => x.ContentHeader(yamlPost)).Returns(postContentHeader);
         _loadMock.Setup(x => x.ContentHeader(yamlDraft)).Returns(draftContentHeader);

         _directoryWrapper.Setup(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SearchOption>())).Returns(files);
      }

      private static List<Content> CreateContentList()
      {
         return new List<Content> {
            new Content
            {
               Title = "Mock Page Content",
               Categories = new string[] { "cat1"},
               Tags = new string[] { "tag1", "tag2"},
               Filename = "page.md",
               Type = ContentType.Page,
               Created = DateTime.Now,
            },
            new Content
            {
               Title = "Mock Post Content",
               Categories = new string[] { "cat2"},
               Tags = new string[] { "tag1", "tag2"},
               Filename = "post.md",
               Type = ContentType.Post,
               Created = DateTime.Now,
            }
         };
      }
      #endregion
   }
}
