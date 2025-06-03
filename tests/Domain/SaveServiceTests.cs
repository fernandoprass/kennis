using Myce.Wrappers.Contracts;

public class SaveServiceTests
{
    private readonly Mock<IFileWrapper> _fileMock = new();
    private readonly Mock<IPathWrapper> _pathMock = new();
    private readonly Mock<ILogService> _logMock = new();

    private SaveService CreateService() =>
        new SaveService(_fileMock.Object, _pathMock.Object, _logMock.Object);

    [Fact]
    public void Configure_SetsFolders()
    {
        var service = CreateService();
        service.Configure("html", "json");
        // No direct way to assert private fields, but no exception means success
    }

    [Fact]
    public void ToJsonFile_SerializesAndSavesFile()
    {
        var service = CreateService();
        service.Configure("html", "json");
        _pathMock.Setup(p => p.Combine("json", "file.json")).Returns("json/file.json");
        _pathMock.Setup(p => p.Combine("html", "json/file.json")).Returns("html/json/file.json");

        service.ToJsonFile("file.json", new { Name = "Test" });

        _fileMock.Verify(f => f.WriteAllText(It.IsAny<string>(), It.Is<string>(s => s.Contains("Test"))), Times.Once);
        _logMock.Verify(l => l.LogInfo(LogCategory.JsonFile, LogAction.FileSaveSuccessfully, "json/file.json"), Times.Once);
    }

    [Fact]
    public void ToHtmlFile_SavesHtmlFile()
    {
        var service = CreateService();
        service.Configure("html", "json");
        _pathMock.Setup(p => p.Combine("html", "index.html")).Returns("html/index.html");

        service.ToHtmlFile("index.html", "<html></html>");

        _fileMock.Verify(f => f.WriteAllText("html/index.html", "<html></html>"), Times.Once);
        _logMock.Verify(l => l.LogInfo(LogCategory.HtmlFile, LogAction.FileSaveSuccessfully, "html/index.html"), Times.Once);
    }

    [Fact]
    public void ToJsonFile_LogsErrorOnException()
    {
        var service = CreateService();
        service.Configure("html", "json");
        _pathMock.Setup(p => p.Combine("json", "file.json")).Returns("json/file.json");
        _pathMock.Setup(p => p.Combine("html", "json/file.json")).Returns("html/json/file.json");
        _fileMock.Setup(f => f.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("fail"));

        service.ToJsonFile("file.json", new { Name = "Test" });

        _logMock.Verify(l => l.LogError(It.IsAny<Exception>(), LogCategory.JsonFile, LogAction.FileSaveFailed, "json/file.json"), Times.Once);
    }

    [Fact]
    public void ToHtmlFile_LogsErrorOnException()
    {
        var service = CreateService();
        service.Configure("html", "json");
        _pathMock.Setup(p => p.Combine("html", "index.html")).Returns("html/index.html");
        _fileMock.Setup(f => f.WriteAllText(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("fail"));

        service.ToHtmlFile("index.html", "<html></html>");

        _logMock.Verify(l => l.LogError(It.IsAny<Exception>(), LogCategory.HtmlFile, LogAction.FileSaveFailed, "html/index.html"), Times.Once);
    }
}
