namespace DatabaseManager
{
    public class Contract
    {
        public int Id { get; set; }
        public int CounterpartyId { get; set; }
        public Counterpartie? Counterpartie { get; set; }
        public DateTime Date { get; set; }
        public double Summ { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Count { get; set; }

        public List<Sale> Sales { get; set; } = new List<Sale>();
    }
}
