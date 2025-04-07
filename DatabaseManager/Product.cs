using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseManager
{
    public class Product : BaseModel
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

        private string? _serialNo;
        public required string SerialNo
        {
            get => _serialNo ?? throw new InvalidOperationException("SerialNo must be initialized.");
            set
            {
                _serialNo = value;
                OnPropertyChanged(nameof(SerialNo));
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

        private List<Contract> _contracts = [];
        public List<Contract> Contracts
        {
            get => _contracts;
            set
            {
                _contracts = value;
                OnPropertyChanged(nameof(Contracts));
            }
        }

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value > 0 ? value : 0;
                OnPropertyChanged(nameof(Count));
            }
        }

        private double _price;
        public double Price
        {
            get => _price;
            set
            {
                _price = Math.Round(value > 0 ? value : 0, 3);
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(SummNds));
                OnPropertyChanged(nameof(Summ));
            }
        }

        [NotMapped]
        public double Nds;
        [NotMapped]
        public double SummNds { get => Math.Round(Price * (Nds / 100), 3); }
        [NotMapped]
        public double Summ { get => Math.Round(Price + SummNds, 3); }
    }
}
