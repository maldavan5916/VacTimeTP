namespace DatabaseManager
{
    public class Contract : BaseModel
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

        private int _counterpartyId;
        public int CounterpartyId
        {
            get => _counterpartyId;
            set
            {
                _counterpartyId = value;
                OnPropertyChanged(nameof(CounterpartyId));
            }
        }

        private Counterpartie? _counterpartie;
        public Counterpartie? Counterpartie
        {
            get => _counterpartie;
            set
            {
                _counterpartie = value;
                OnPropertyChanged(nameof(Counterpartie));
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

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged(nameof(Count));
            }
        }

        private List<Sale> _sales = [];
        public List<Sale> Sales
        {
            get => _sales;
            set
            {
                _sales = value;
                OnPropertyChanged(nameof(Sales));
            }
        }
    }
}
