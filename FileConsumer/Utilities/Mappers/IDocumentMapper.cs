using FileConsumer.Models.Document;

namespace FileConsumer.Utilities.Mappers
{
    public interface IDocumentMapper
    {
        HeaderModel MapHeader(string line);
        ItemModel MapItem(string line);
        string MapComment(string line);
    }
}
