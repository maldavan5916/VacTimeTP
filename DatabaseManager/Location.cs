namespace DatabaseManager
{
    public class Location
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
        public List<Material> Materials { get; set; } = new List<Material>();
    }
}
