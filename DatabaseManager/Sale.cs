namespace DatabaseManager
{
    public class Sale : BaseModel
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

        private int _productId;
        public int ProductId
        {
            get => _productId;
            set
            {
                _productId = value;
                OnPropertyChanged(nameof(ProductId));
            }
        }

        private Product? _product;
        public Product? Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
            }
        }

        private int _contractId;
        public int ContractId
        {
            get => _contractId;
            set
            {
                _contractId = value;
                OnPropertyChanged(nameof(ContractId));
            }
        }

        private Contract? _contract;
        public Contract? Contract
        {
            get => _contract;
            set
            {
                _contract = value;
                OnPropertyChanged(nameof(Contract));
            }
        }

        private double _summ;
        public double Summ
        {
            get => _summ;
            set
            {
                _summ = value;
                OnPropertyChanged(nameof(Summ));
            }
        }

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
    }
}
