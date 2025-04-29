using System.ComponentModel.DataAnnotations;

namespace DatabaseManager
{
    public class Employee : BaseModel
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

        private string? _fio;
        public required string Fio
        {
            get => _fio ?? throw new InvalidOperationException("Fio must be initialized.");
            set
            {
                _fio = value;
                OnPropertyChanged(nameof(Fio));
            }
        }

        private int _divisionId;
        public int DivisionId
        {
            get => _divisionId;
            set
            {
                _divisionId = value;
                OnPropertyChanged(nameof(DivisionId));
            }
        }

        private Division? _division;
        public Division? Division
        {
            get => _division;
            set
            {
                _division = value;
                OnPropertyChanged(nameof(Division));
            }
        }

        private int _postId;
        public int PostId
        {
            get => _postId;
            set
            {
                _postId = value;
                OnPropertyChanged(nameof(PostId));
            }
        }

        private Post? _post;
        public Post? Post
        {
            get => _post;
            set
            {
                _post = value;
                OnPropertyChanged(nameof(Post));
            }
        }

        private DateTime _dateHire;
        public DateTime DateHire
        {
            get => _dateHire;
            set
            {
                _dateHire = value;
                OnPropertyChanged(nameof(DateHire));
            }
        }

        private DateTime? _dateDismissal;
        public DateTime? DateDismissal
        {
            get => _dateDismissal;
            set
            {
                _dateDismissal = value;
                OnPropertyChanged(nameof(DateDismissal));
            }
        }

        private DateTime _dateOfBirth;
        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged(nameof(DateOfBirth));
            }
        }

        [StringLength(13, MinimumLength = 10, ErrorMessage = "The line length must be exactly 10-13 characters.")]
        private string? _phoneNumber;
        public required string PhoneNumber
        {
            get => _phoneNumber ?? throw new InvalidOperationException("PhoneNumber must be initialized.");
            set
            {
                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        private string? _address;
        public required string Address
        {
            get => _address ?? throw new InvalidOperationException("Address must be initialized.");
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        private string? _passportData;
        public required string PassportData
        {
            get => _passportData ?? throw new InvalidOperationException("PassportData must be initialized.");
            set
            {
                _passportData = value;
                OnPropertyChanged(nameof(PassportData));
            }
        }

        private double _salary;
        public double Salary
        {
            get => Math.Round(_salary, 2);
            set
            {
                _salary = value;
                OnPropertyChanged(nameof(Salary));
            }
        }

        private double? _bonuses;
        public double? Bonuses
        {
            get => Math.Round(_bonuses ?? 0, 3);
            set
            {
                _bonuses = value;
                OnPropertyChanged(nameof(Bonuses));
            }
        }

        private string? _bankDetails;
        public required string BankDetails
        {
            get => _bankDetails ?? throw new InvalidOperationException("BankDetails must be initialized.");
            set
            {
                _bankDetails = value;
                OnPropertyChanged(nameof(BankDetails));
            }
        }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string GetFioAndPost { get => $"{Fio}\t({Post?.Name})"; }
    }
}
