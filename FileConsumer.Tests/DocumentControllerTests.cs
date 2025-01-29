using FileConsumer.Controllers;
using FileConsumer.Models;
using FileConsumer.Services.DocumentService;
using FileConsumer.Services.Validators;
using FileConsumer.Utilities;
using FileConsumer.Utilities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FileConsumer.Tests.Controllers;

public class DocumentControllerTests
{
    private readonly Mock<IDocumentService> _documentServiceMock;
    private readonly Mock<IUserMessagesProvider> _messagesProviderMock;
    private readonly Mock<IFileValidatorService> _fileValidatorMock;
    private readonly DocumentController _controller;

    public DocumentControllerTests()
    {
        _documentServiceMock = new Mock<IDocumentService>();
        _messagesProviderMock = new Mock<IUserMessagesProvider>();
        _fileValidatorMock = new Mock<IFileValidatorService>();
        _controller = new DocumentController(_documentServiceMock.Object, _messagesProviderMock.Object, _fileValidatorMock.Object);
    }

    [Fact]
    public async Task Test_ReturnsBadRequest_WhenFileIsInvalid()
    {
        var fileMock = new Mock<IFormFile>();
        _fileValidatorMock.Setup(v => v.ValidateFile(fileMock.Object)).Returns(ValidationResult.Failure("Invalid file"));

        var result = await _controller.Test(5, fileMock.Object);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid file", badRequestResult.Value);
    }

    [Fact]
    public async Task Test_ReturnsOk_WhenFileIsValid()
    {
        var fileMock = new Mock<IFormFile>();
        var documentResponse = new DocumentResponse();
        _fileValidatorMock.Setup(v => v.ValidateFile(fileMock.Object)).Returns(ValidationResult.Success());
        _documentServiceMock.Setup(s => s.ReadFileAsync(fileMock.Object, 5)).ReturnsAsync(documentResponse);

        var result = await _controller.Test(5, fileMock.Object);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(documentResponse, okResult.Value);
    }

    [Fact]
    public async Task Test_ReturnsUnprocessableEntity_OnException()
    {
        var fileMock = new Mock<IFormFile>();
        _fileValidatorMock.Setup(v => v.ValidateFile(fileMock.Object)).Returns(ValidationResult.Success());
        _documentServiceMock.Setup(s => s.ReadFileAsync(fileMock.Object, 5)).ThrowsAsync(new Exception());
        _messagesProviderMock.Setup(m => m.GetString(MessageKey.FileProcessingError)).Returns("File processing error");

        var result = await _controller.Test(5, fileMock.Object);

        var unprocessableEntityResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
        Assert.Equal("File processing error", unprocessableEntityResult.Value);
    }
}
