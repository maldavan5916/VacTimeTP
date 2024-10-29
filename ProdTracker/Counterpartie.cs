namespace ProdTracker
{
    public enum CounterpartieType { Fiz, Ur }

    public class Counterpartie
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string LegalAddress { get; set; }
        public required string PhoneNomber { get; set; }
        public required string PostalAddress { get; set; }
        public required string Unp { get; set; }
        public CounterpartieType Type { get; set; } = CounterpartieType.Fiz;
        public string? Okulp { get; set; }
        public string? Okpo { get; set; }
        public string? Oked { get; set; }

        public List<Contract> Contracts { get; set; } = new List<Contract>();
        public List<Receipt> Receipts { get; set; } = new List<Receipt>();
    }
}
