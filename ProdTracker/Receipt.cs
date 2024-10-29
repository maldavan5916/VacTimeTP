namespace ProdTracker
{
    public class Receipt
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public Material? Material { get; set; }
        public double Summ { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public int CounterpartyId { get; set; }
        public Counterpartie? Counterpartie { get; set; }
    }
}
