namespace DatabaseManager
{
    public class Receipt : BaseModel
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

        private int _materialId;
        public int MaterialId
        {
            get => _materialId;
            set
            {
                _materialId = value;
                OnPropertyChanged(nameof(MaterialId));
            }
        }

        private Material? _material;
        public Material? Material
        {
            get => _material;
            set
            {
                _material = value;
                OnPropertyChanged(nameof(Material));
                Summ = Count * (Material != null ? Material.Price : 0);
            }
        }

        private double _summ;
        public double Summ
        {
            get => Math.Round(_summ, 2);
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

        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged(nameof(Count));
                Summ = Count * (Material != null ? Material.Price : 0);
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
    }
}
