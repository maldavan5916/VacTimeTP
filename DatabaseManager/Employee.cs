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
    }
}
