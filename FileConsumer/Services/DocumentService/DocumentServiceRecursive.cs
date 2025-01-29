using FileConsumer.Models.Document;
using FileConsumer.Models;
using FileConsumer.Utilities.Mappers;
using FileConsumer.Services.DocumentService;

public class DocumentServiceRecursive : IDocumentService
{
    private readonly IDocumentMapper _documentMapper;
    private List<DocumentModel> _documents = new();
    private List<ItemModel> _productsWithMaxNetValues = new();

    public DocumentServiceRecursive(IDocumentMapper documentMapper)
    {
        _documentMapper = documentMapper;
    }

    public async Task<DocumentResponse> ReadFileAsync(IFormFile file, int filterAmount)
    {
        using var stream = file.OpenReadStream();
        using StreamReader reader = new(stream);

        var stats = new ProcessingStats();

        await ProcessLineRecursive(reader, stats, filterAmount);

        return new DocumentResponse
        {
            Documents = _documents,
            CharsCount = stats.TotalChars,
            LinesCount = stats.TotalLines,
            Sum = stats.ItemsTotal,
            XCount = _documents.Count(x => x.Items.Count > filterAmount),
            ProductWithMaxNetValue = GetProductWithMaxNetValue()
        };
    }

    private async Task ProcessLineRecursive(StreamReader reader, ProcessingStats stats, int filterAmount)
    {
        var line = await reader.ReadLineAsync();

        if (line == null) return;

        stats.TotalChars += line.Length;
        stats.TotalLines++;

        switch (line[0])
        {
            case 'H':
                var newDocument = new DocumentModel
                {
                    Header = _documentMapper.MapHeader(line),
                    Items = new List<ItemModel>()
                };
                _documents.Add(newDocument);
                await ProcessLineRecursive(reader, stats, filterAmount);
                break;

            case 'B':
                var currentDocument = _documents.LastOrDefault();
                if (currentDocument != null)
                {
                    var item = _documentMapper.MapItem(line);
                    currentDocument.Items.Add(item);
                    stats.ItemsTotal++;
                    CheckIfProductWithMaxNetValue(item);
                }
                await ProcessLineRecursive(reader, stats, filterAmount);
                break;

            case 'C':
                var commentDocument = new DocumentModel { Comment = line };
                _documents.Add(commentDocument);
                await ProcessLineRecursive(reader, stats, filterAmount);
                break;
        }
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

    private string GetProductWithMaxNetValue()
        => string.Join(", ", _productsWithMaxNetValues.Select(p => p.ProductName));

}

public class ProcessingStats
{
    public int TotalChars { get; set; } = 0;
    public int ItemsTotal { get; set; } = 0;
    public int TotalLines { get; set; } = 0;
}
