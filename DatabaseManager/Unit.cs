namespace DatabaseManager
{
    public class Unit : BaseModel
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string? _name;
        public required string Name
        {
            get => _name ?? throw new InvalidOperationException("Name must be initialized.");
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private List<Product> _products = [];
        public List<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        private List<Material> _materials = [];
        public List<Material> Materials
        {
            get => _materials;
            set
            {
                _materials = value;
                OnPropertyChanged(nameof(Materials));
            }
        }
    }
}
