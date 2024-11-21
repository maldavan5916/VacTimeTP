namespace DatabaseManager
{
    public class Division : BaseModel
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

        private List<Employee> _employees = [];
        public List<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }
    }
}
