using FileConsumer.Models.Document;
using System.Globalization;

namespace FileConsumer.Utilities.Mappers;

public class DocumentMapper : IDocumentMapper
{
    public string MapComment(string line)
    {
        string[] headerLine = line.Split(',');
        return headerLine[1];
    }

    public HeaderModel MapHeader(string line)
    {
        string[] headerLine = line.Split(',');

        return new HeaderModel()
        {
            BaCode = headerLine[1],
            Type = headerLine[2],
            Number = headerLine[3],
            Date = DateOnly.ParseExact(headerLine[4], "dd-MM-yyyy", CultureInfo.InvariantCulture),
            DocumentDateNumber = headerLine[5],
            ContractorNumber = headerLine[6],
            ContractorName = headerLine[7],
            ExternalDocumentNumber = headerLine[8],
            ExternalDocumentDate = DateOnly.ParseExact(headerLine[9], "dd-MM-yyyy", CultureInfo.InvariantCulture),
            ValueNet = double.Parse(headerLine[10], CultureInfo.InvariantCulture),
            ValueGross = double.Parse(headerLine[11], CultureInfo.InvariantCulture),
            ExternalFieldOne = headerLine[12],
            ExternalFieldTwo = headerLine[13],
            ExternalFieldThree = headerLine[14]
        };
    }

    public ItemModel MapItem(string line)
    {
        string[] itemLine = line.Split(',');

        return new ItemModel()
        {
            ProductId = itemLine[1],
            ProductName = itemLine[2],
            Quantity = double.Parse(itemLine[3], CultureInfo.InvariantCulture),
            PriceNet = decimal.Parse(itemLine[4], CultureInfo.InvariantCulture),
            PriceGross = decimal.Parse(itemLine[5], CultureInfo.InvariantCulture),
            VatRate = decimal.Parse(itemLine[6], CultureInfo.InvariantCulture),
            QuantityBefore = double.Parse(itemLine[7], CultureInfo.InvariantCulture),
            AverageBefore = decimal.Parse(itemLine[8], CultureInfo.InvariantCulture),
            QuantityAfter = double.Parse(itemLine[9], CultureInfo.InvariantCulture),
            AverageAfter = decimal.Parse(itemLine[10], CultureInfo.InvariantCulture),
            Group = itemLine[11]
        };
    }
}
