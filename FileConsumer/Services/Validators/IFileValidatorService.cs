namespace FileConsumer.Services.Validators;

public interface IFileValidatorService
{
    ValidationResult ValidateFile(IFormFile file);
}