namespace DatabaseManager
{
    public class Material : BaseModel
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

        private int _unitId;
        public int UnitId
        {
            get => _unitId;
            set
            {
                _unitId = value;
                OnPropertyChanged(nameof(UnitId));
            }
        }

        private Unit? _unit;
        public Unit? Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                OnPropertyChanged(nameof(Unit));
            }
        }

        private int _locationId;
        public int LocationId
        {
            get => _locationId;
            set
            {
                _locationId = value;
                OnPropertyChanged(nameof(LocationId));
            }
        }

        private Location? _location;
        public Location? Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        private double _count;
        public double Count
        {
            get => Math.Round(_count, 3);
            set
            {
                _count = value > 0 ? value : 0;
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(nameof(GetSum));
            }
        }

        private double _price;
        public double Price
        {
            get => Math.Round(_price, 2);
            set
            {
                _price = value > 0 ? value : 0;
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(GetSum));
            }
        }

        private List<Product_Material> _productMaterials = [];
        public List<Product_Material> ProductMaterials
        {
            get => _productMaterials;
            set
            {
                _productMaterials = value;
                OnPropertyChanged(nameof(ProductMaterials));
            }
        }

        private List<Receipt> _receipts = [];
        public List<Receipt> Receipts
        {
            get => _receipts;
            set
            {
                _receipts = value;
                OnPropertyChanged(nameof(Receipts));
            }
        }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public double GetSum { get => Math.Round(Count * Price, 3); }
    }
}
