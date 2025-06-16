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
                if (Contract == null || Contract.Product == null) return;

                if (value > _count)
                {
                    int required = value - _count;

                    if (value > Contract.Count)
                    {
#if !DEBUG
                        throw new InvalidOperationException(
                                $"По договору больше не требуется '{Contract.Product?.Name}': требуется {Contract.Count}");
#else
                        System.Diagnostics.Debug.WriteLine(
                            $"По договору больше не требуется '{Contract.Product?.Name}': требуется {Contract.Count}");
                        return;
#endif
                    }

                    if (required > Contract.Product.Count)
                    {
#if !DEBUG
                            throw new InvalidOperationException(
                                $"Недостаточно материала '{Contract.Product?.Name}': требуется {required}, в наличии {Contract.Product?.Count}");
#else
                        System.Diagnostics.Debug.WriteLine(
                            $"Недостаточно изделий '{Contract.Product?.Name}': требуется {required}, в наличии {Contract.Product?.Count}");
                        return;
#endif
                    }

                    Contract.Product.CountWithoutCheck -= required; // Deduct product

                    _count = value;
                }
                else if (value < _count)
                {
                    int required = _count - value;
                    Contract.Product.CountWithoutCheck += required; // restore product
                    _count = value;
                }

                OnPropertyChanged(nameof(Count));
            }
        }
    }
}
