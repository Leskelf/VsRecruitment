namespace FileConsumer.Models.Document;

public class DocumentModel
{
    public HeaderModel Header { get; set; }
    public string? Comment { get; set; }
    public List<ItemModel> Items { get; set; }

    public DocumentModel()
    {
        Items = [];
    }
}
