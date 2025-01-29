using FileConsumer.Models;
using FileConsumer.Models.Document;
using FileConsumer.Utilities.Mappers;

namespace FileConsumer.Services.DocumentService;

public class DocumentService : IDocumentService
{
    private readonly IDocumentMapper _documentMapper;
    private List<DocumentModel> _documents = [];
    private List<ItemModel> _productsWithMaxNetValues = [];
    private DocumentModel _tempDocumentModel = new();
    int _totalLines = 0;

    public DocumentService(IDocumentMapper documentMapper)
    {
        _documentMapper = documentMapper;
    }

    public async Task<DocumentResponse> ReadFileAsync(IFormFile file, int filterAmount)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using StreamReader reader = new(stream);
            string line;

            int totalChars = 0;
            int itemsRowsCount = 0;
            int itemsTotal = 0;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                totalChars += line.Length;
                _totalLines++;

                switch (line[0])
                {
                    case 'H':
                        ProcessHeader(line, filterAmount);
                        itemsRowsCount = 0;
                        break;

                    case 'B':
                        ProcessItem(line);
                        itemsTotal++;
                        itemsRowsCount++;
                        break;

                    case 'C':
                        ProcessComment(line);
                        break;
                }
            }

            return new DocumentResponse
            {
                Documents = _documents,
                CharsCount = totalChars,
                LinesCount = _totalLines,
                Sum = itemsTotal,
                XCount = _documents.Count(x => x.Items.Count > filterAmount),
                ProductWithMaxNetValue = GetProductWithMaxNetValue()
            };
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private void ProcessHeader(string line, int x)
    {
        _tempDocumentModel = new();
        _tempDocumentModel.Header = _documentMapper.MapHeader(line);
        _documents.Add(_tempDocumentModel);
    }

    private void ProcessItem(string line)
    {
        var item = _documentMapper.MapItem(line);
        _tempDocumentModel.Items.Add(item);
        CheckIfProductWithMaxNetValue(item);        
    }

    private void ProcessComment(string line)
    {
        if (_totalLines != 1)
        {
            _tempDocumentModel = new();
            _tempDocumentModel.Comment = _documentMapper.MapComment(line);
            _documents.Add(_tempDocumentModel);
            return;
        }

        _tempDocumentModel.Comment = line;
    }

    private void CheckIfProductWithMaxNetValue(ItemModel item)
    {
        if (_productsWithMaxNetValues.Count == 0)
        {
            _productsWithMaxNetValues.Add(item);
            return;
        }

        var product = _productsWithMaxNetValues.First();

        if (item.PriceNet == product.PriceNet)
        {
            _productsWithMaxNetValues.Add(item);
        }
        else if (item.PriceNet > product.PriceNet)
        {
            _productsWithMaxNetValues.Clear();
            _productsWithMaxNetValues.Add(item);
        }
    }

    private string GetProductWithMaxNetValue() => string.Join(", ", _productsWithMaxNetValues.Select(p => p.ProductName));
}
