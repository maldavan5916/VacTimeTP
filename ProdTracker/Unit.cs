namespace ProdTracker
{
    public class Unit
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        // Навигационные свойства для связи с Products и Materials
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Material> Materials { get; set; } = new List<Material>();
    }
}
