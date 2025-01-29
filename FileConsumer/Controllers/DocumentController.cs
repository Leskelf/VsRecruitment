using FileConsumer.Models;
using FileConsumer.Services.DocumentService;
using FileConsumer.Services.Validators;
using FileConsumer.Utilities;
using FileConsumer.Utilities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileConsumer.Controllers
{
    [ApiController]
    [Route("api/")]    
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IUserMessagesProvider _messagesProvider;
        private readonly IFileValidatorService _fileValidator;

        public DocumentController(IDocumentService documentService, IUserMessagesProvider resourceProvider, IFileValidatorService fileValidator)
        {
            _documentService = documentService;
            _messagesProvider = resourceProvider;
            _fileValidator = fileValidator;
        }

        [Authorize]
        [HttpPost("test/{x:int}")]
        [Produces("application/json")]
        [ProducesResponseType<DocumentResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Test(int x, IFormFile file)
        {
            try
            {
                var validationResult = _fileValidator.ValidateFile(file);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.ErrorMessage);

                var response = await _documentService.ReadFileAsync(file, x);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var errorMessage = _messagesProvider.GetString(MessageKey.FileProcessingError);
                return UnprocessableEntity(errorMessage);
            }
        }
    }
}
