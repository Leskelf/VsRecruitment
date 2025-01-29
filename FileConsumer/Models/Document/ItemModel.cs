namespace FileConsumer.Models.Document;

public class ItemModel
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public double Quantity { get; set; }
    public decimal PriceNet { get; set; }
    public decimal PriceGross { get; set; }
    public decimal VatRate { get; set; }
    public double QuantityBefore { get; set; }
    public decimal AverageBefore { get; set; }
    public double QuantityAfter { get; set; }
    public decimal AverageAfter { get; set; }
    public string Group { get; set; }
}
