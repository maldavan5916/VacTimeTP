using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DatabaseManager;

namespace VacTrack
{
    /// <summary>
    /// Interaction logic for CreateUser.xaml
    /// </summary>
    public partial class CreateUser : Window
    {
        private CreateUserViewModel ThisViewModel => (CreateUserViewModel)DataContext;
        public CreateUser()
        {
            InitializeComponent();
            ThisViewModel.ThisWin = this;
        }
    }

    public class CreateUserViewModel : INotifyPropertyChanged
    {
        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set
            {
                SetProperty(ref _message, value);
            }
        }

        public Window? ThisWin { get; set; }

        #region interface implemented 
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<Ts>(ref Ts field, Ts value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<Ts>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        public string? LoginString { get; set; }
        public string? PasswordString { get; set; }
        public string? SecondPasswordString { get; set; }

        public ICommand CreateUser { get; set; }

        public CreateUserViewModel()
        {
            CreateUser = new RelayCommand(AddUser);
        }

        private void AddUser(object obj)
        {
            if (PasswordString != SecondPasswordString)
            {
                Message = "Пароли не совпадают";
                return;
            }

            if (LoginString == null || PasswordString == null)
            {
                Message = "Пароль не введен";
                return;
            }

            using (var db = new DatabaseContext())
            {
                var newUser = new Users
                {
                    Login = LoginString,
                    Password = HashPassword(PasswordString),
                    Access = "FULL"
                };

                db.Set<Users>().Add(newUser);
                db.SaveChanges();
            }

            if (ThisWin != null)
            {
                ThisWin.Close();
                Message = "Успешно";
            }
            else
                Message = "Успешно, закройте это окно";
        }

        public static string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
