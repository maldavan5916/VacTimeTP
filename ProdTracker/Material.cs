namespace ProdTracker
{
    public class Material
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        
        public int UnitId { get; set; }
        public Unit? Unit { get; set; }
        
        public int LocationId { get; set; }
        public Location? Location { get; set; }
        
        public int Count { get; set; }
        public double Price { get; set; }

        public List<Product_Material> ProductMaterials { get; set; } = new List<Product_Material>();
        public List<Receipt> Receipts { get; set; } = new List<Receipt>();
    }
}
