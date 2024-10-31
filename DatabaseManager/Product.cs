namespace DatabaseManager
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string SerialNo { get; set; }

        public int UnitId { get; set; }
        public Unit? Unit { get; set; }

        public int LocationId { get; set; }
        public Location? Location { get; set; }

        public List<Product_Material> ProductMaterials { get; set; } = new List<Product_Material>();
        public List<Contract> Contracts { get; set; } = new List<Contract>();
        public List<Sale> Sales { get; set; } = new List<Sale>();
    }
}
