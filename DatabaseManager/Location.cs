namespace DatabaseManager
{
    public class Location : BaseModel
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

        private int _employeeId;
        public int EmployeeId
        {
            get => _employeeId;
            set
            {
                _employeeId = value;
                OnPropertyChanged(nameof(EmployeeId));
            }
        }

        private Employee? _employee;
        public Employee? Employee
        {
            get => _employee;
            set
            {
                _employee = value;
                OnPropertyChanged(nameof(Employee));
            }
        }

        private List<Product> _products = new List<Product>();
        public List<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        private List<Material> _materials = new List<Material>();
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
