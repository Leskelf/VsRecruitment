using FileConsumer.Models.Document;

namespace FileConsumer.Models
{
    public class DocumentResponse
    {
        public List<DocumentModel>? Documents { get; set; }
        public int LinesCount { get; set; }
        public int CharsCount { get; set; }
        public int Sum { get; set; }
        public int XCount{ get; set; }
        public string? ProductWithMaxNetValue { get; set; }
    }
}
