using FileConsumer.Services.Validators;
using FileConsumer.Utilities;
using FileConsumer.Utilities.Enums;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace FileConsumer.Tests.Services.Validators;

public class FileValidatorServiceTests
{
    private readonly Mock<IUserMessagesProvider> _userMessagesProviderMock;
    private readonly FileValidatorService _fileValidatorService;

    public FileValidatorServiceTests()
    {
        _userMessagesProviderMock = new Mock<IUserMessagesProvider>();
        _fileValidatorService = new FileValidatorService(_userMessagesProviderMock.Object);
    }

    [Fact]
    public void ValidateFile_NullFile_ReturnsFailure()
    {
        _userMessagesProviderMock.Setup(provider => provider.GetString(MessageKey.EmptyFileError))
            .Returns("File is empty.");

        var result = _fileValidatorService.ValidateFile(null);

        Assert.False(result.IsValid);
        Assert.Equal("File is empty.", result.ErrorMessage);
    }

    [Fact]
    public void ValidateFile_EmptyFile_ReturnsFailure()
    {
        var formFile = new FormFile(Stream.Null, 0, 0, "file", "test.pur");
        _userMessagesProviderMock.Setup(provider => provider.GetString(MessageKey.EmptyFileError))
            .Returns("File is empty.");

        var result = _fileValidatorService.ValidateFile(formFile);

        Assert.False(result.IsValid);
        Assert.Equal("File is empty.", result.ErrorMessage);
    }

    [Fact]
    public void ValidateFile_InvalidFileFormat_ReturnsFailure()
    {
        var fileContent = "Test file content";
        var formFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)), 0, fileContent.Length, "file", "test.txt");
        _userMessagesProviderMock.Setup(provider => provider.GetString(MessageKey.InvalidFileFormat))
            .Returns("Invalid file format.");

        var result = _fileValidatorService.ValidateFile(formFile);

        Assert.False(result.IsValid);
        Assert.Equal("Invalid file format.", result.ErrorMessage);
    }

    [Fact]
    public void ValidateFile_ValidFile_ReturnsSuccess()
    {
        var fileContent = "Test file content";
        var formFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)), 0, fileContent.Length, "file", "test.pur");

        var result = _fileValidatorService.ValidateFile(formFile);

        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }
}

