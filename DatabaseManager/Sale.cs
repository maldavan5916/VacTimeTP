namespace DatabaseManager
{
    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int ContractId { get; set; }
        public Contract? Contract { get; set; }
        public double Summ { get; set; }
        public DateTime Date { get; set; }
    }
}
