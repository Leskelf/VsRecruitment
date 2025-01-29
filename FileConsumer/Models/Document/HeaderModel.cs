namespace FileConsumer.Models.Document
{
    public class HeaderModel
    {
        public string BaCode { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public DateOnly Date { get; set; }
        public string DocumentDateNumber { get; set; }
        public string ContractorNumber { get; set; }
        public string ContractorName { get; set; }
        public string ExternalDocumentNumber { get; set; }
        public DateOnly ExternalDocumentDate { get; set; }
        public double ValueNet { get; set; }
        public double ValueGross { get; set; }
        public string ExternalFieldOne { get; set; }
        public string ExternalFieldTwo { get; set; }
        public string ExternalFieldThree { get; set; }
    }
}
