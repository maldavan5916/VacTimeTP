namespace DatabaseManager
{
    public enum CounterpartieType { Fiz, Ur }

    public class Counterpartie : BaseModel
    {
        private int _Id;
        public int Id 
        {
            get => _Id; 
            set => SetProperty(ref _Id, value);
        }

        private string _Name;
        public required string Name 
        {
            get => _Name; 
            set => SetProperty(ref _Name, value);
        }

        private string _LegalAddress;
        public required string LegalAddress 
        {
            get => _LegalAddress;
            set => SetProperty(ref _LegalAddress, value);
        }

        private string _PhoneNomber;
        public required string PhoneNomber 
        {
            get => _PhoneNomber; 
            set => SetProperty(ref _PhoneNomber, value);
        }

        private string _PostalAddress;
        public required string PostalAddress 
        { 
            get => _PostalAddress; 
            set => SetProperty(ref _PostalAddress, value);
        }

        private string _Unp;
        public required string Unp 
        { 
            get => _Unp; 
            set => SetProperty(ref _Unp, value); 
        }

        private CounterpartieType _Type = CounterpartieType.Fiz;
        public CounterpartieType Type
        {
            get => _Type;
            set => SetProperty(ref _Type, value);
        }

        private string? _Okulp;
        public string? Okulp
        {
            get => _Okulp;
            set => SetProperty(ref _Okulp, value);
        }

        private string? _Okpo;
        public string? Okpo
        {
            get => _Okpo;
            set => SetProperty(ref _Okpo, value);
        }

        private string? _Oked;
        public string? Oked
        {
            get => _Oked;
            set => SetProperty(ref _Oked, value);
        }

        private List<Contract> _Contracts = [];
        public List<Contract> Contracts
        {
            get => _Contracts;
            set => SetProperty(ref _Contracts, value);
        }

        private List<Receipt> _Receipts = [];
        public List<Receipt> Receipts
        {
            get => _Receipts;
            set => SetProperty(ref _Receipts, value);
        }
    }
}
