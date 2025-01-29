using FileConsumer.Utilities;
using FileConsumer.Utilities.Enums;

namespace FileConsumer.Services.Validators;

public class FileValidatorService : IFileValidatorService
{
    private readonly IUserMessagesProvider _errorMessagesProvider;

    public FileValidatorService(IUserMessagesProvider errorMessagesProvider)
    {
        _errorMessagesProvider = errorMessagesProvider;
    }

    public ValidationResult ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return ValidationResult.Failure(_errorMessagesProvider.GetString(MessageKey.EmptyFileError));

        if (Path.GetExtension(file.FileName).ToLower() != ".pur")
            return ValidationResult.Failure(_errorMessagesProvider.GetString(MessageKey.InvalidFileFormat));

        return ValidationResult.Success();
    }
}