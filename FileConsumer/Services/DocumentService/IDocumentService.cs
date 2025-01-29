using FileConsumer.Models;

namespace FileConsumer.Services.DocumentService;

public interface IDocumentService
{
    Task<DocumentResponse> ReadFileAsync(IFormFile file, int filterAmount);
}
