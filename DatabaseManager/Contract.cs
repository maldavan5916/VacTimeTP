﻿namespace DatabaseManager
{
    public enum ContractStatus { created, running, completed }

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

        private string? _name;
        public required string Name
        {
            //_name ?? throw new InvalidOperationException("Name must be initialized.");
            get => $"ДП-{(Id > 0 ? Id : new DatabaseContext().Contracts.Any()
                ? new DatabaseContext().Contracts.Max(e => e.Id) + 1
                : 1):D3}";
            set
            {
                _name = value;
                SetProperty(ref _name, value);
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

        private int _term;
        public int Term
        {
            get => _term;
            set
            {
                _term = value;
                OnPropertyChanged(nameof(Term));
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
                SetProperty(ref _count, value > 0 ? value : 0);
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

        private ContractStatus _status = ContractStatus.created;
        public ContractStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string DisplayFull => $"Назвение: {Name};\nКому: {Counterpartie?.Name};\nИзделие: {Product?.Name}";

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int CntLeft
        {
            get
            {
                int available = _count - _sales.Sum(s => s.Count);

                //if (available == _count) Status = ContractStatus.created;
                //else if (available == 0) Status = ContractStatus.completed;
                //else                     Status = ContractStatus.running;

                return available;
            }
        }
    }
}
