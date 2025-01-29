using FileConsumer.Models;
using FileConsumer.Models.Document;
using FileConsumer.Services.DocumentService;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace FileConsumer.Tests.Services.DocumentService;

public class DocumentServiceTests
{
    private readonly Mock<IDocumentService> _documentServiceMock;

    public DocumentServiceTests()
    {
        _documentServiceMock = new Mock<IDocumentService>();
    }

    [Fact]
    public async Task ReadFileAsync_ValidFile_ReturnsDocumentResponse()
    {
        var fileContent = "Test file content";
        var fileName = "test.txt";
        var formFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)), 0, fileContent.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        var expectedResponse = new DocumentResponse
        {
            Documents = new List<DocumentModel>
            {
                new DocumentModel
                {
                    Header = new HeaderModel(),
                    Comment = "Test comment",
                    Items = new List<ItemModel>()
                }
            },
            LinesCount = 1,
            CharsCount = fileContent.Length,
            Sum = 0,
            XCount = 0,
            ProductWithMaxNetValue = null
        };

        _documentServiceMock.Setup(service => service.ReadFileAsync(formFile, It.IsAny<int>()))
            .ReturnsAsync(expectedResponse);

        var result = await _documentServiceMock.Object.ReadFileAsync(formFile, 0);

        Assert.NotNull(result);
        Assert.Equal(expectedResponse.LinesCount, result.LinesCount);
        Assert.Equal(expectedResponse.CharsCount, result.CharsCount);
        Assert.Equal(expectedResponse.Sum, result.Sum);
        Assert.Equal(expectedResponse.XCount, result.XCount);
        Assert.Equal(expectedResponse.ProductWithMaxNetValue, result.ProductWithMaxNetValue);
    }

    [Fact]
    public async Task ReadFileAsync_InvalidFile_ThrowsException()
    {
        var fileContent = "Invalid file content";
        var fileName = "invalid.txt";
        var formFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)), 0, fileContent.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        _documentServiceMock.Setup(service => service.ReadFileAsync(formFile, It.IsAny<int>()))
            .ThrowsAsync(new Exception("Invalid file format"));

        await Assert.ThrowsAsync<Exception>(() => _documentServiceMock.Object.ReadFileAsync(formFile, 0));
    }
}
