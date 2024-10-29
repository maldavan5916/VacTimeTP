namespace ProdTracker
{
    public class Product_Material
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int MaterialId { get; set; }
        public Material? Material { get; set; }
        public int Quantity { get; set; }
    }
}
