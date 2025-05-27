namespace DatabaseManager
{
    public class Users : BaseModel
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                SetProperty(ref _id, value);
            }
        }

        private string? _login;
        public required string Login
        {
            get => _login ?? throw new InvalidOperationException("Login must be initialized.");
            set
            {
                SetProperty(ref _login, value);
            }
        }

        private string? _pass;
        public required string Password
        {
            get => _pass ?? throw new InvalidOperationException("Password must be initialized.");
            set
            {
                SetProperty(ref _pass, value);
            }
        }

        private string? _access;
        public string? Access
        {
            get => _access?.ToLower();
            set
            {
                SetProperty(ref _access, value?.ToLower());
            }
        }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public bool IsAdmin { get => Access != null && Access.Contains('a'); }
    }
}
