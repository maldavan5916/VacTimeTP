using DatabaseManager;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace VacTrack
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page, ICachedPage
    {
        private SettingViewModel ThisViewModel => (SettingViewModel)DataContext;
        public SettingsPage() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.ReloadData();
    }

    public class SettingViewModel : INotifyPropertyChanged
    {
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

        private ObservableCollection<Employee> _employees = null!;
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
        }

        private ObservableCollection<Users> _users = null!;
        private Users _currentUser = null!;
        public Users CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        private string? _message;
        public string? Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private Employee? _selectedAccountant;
        public Employee? SelectedAccountant
        {
            get => _selectedAccountant;
            set => SetProperty(ref _selectedAccountant, value);
        }
        private Employee? _selecteStorekeeper;
        public Employee? SelecteStorekeeper
        {
            get => _selecteStorekeeper;
            set => SetProperty(ref _selecteStorekeeper, value);
        }
        private Employee? _selectedProductReleaseApprover;
        public Employee? SelectedProductReleaseApprover
        {
            get => _selectedProductReleaseApprover;
            set => SetProperty(ref _selectedProductReleaseApprover, value);
        }

        private Employee? _selectedProductSubmitter;
        public Employee? SelectedProductSubmitter
        {
            get => _selectedProductSubmitter;
            set => SetProperty(ref _selectedProductSubmitter, value);
        }
        private double _selectNds;
        public double SelectNds
        {
            get => _selectNds;
            set
            {
                SetProperty(ref _selectNds, value);
            }
        }
        private string _selectCurrency = Properties.Settings.Default.Currency;
        public string SelectCurrency
        {
            get => _selectCurrency;
            set
            {
                SetProperty(ref _selectCurrency, value);
            }
        }

        public bool IsDarkTheme
        {
            get => SelectTheme == BaseTheme.Dark;
            set
            {
                if (!value) return;
                SelectTheme = BaseTheme.Dark;
            }
        }
        public bool IsLightTheme
        {
            get => SelectTheme == BaseTheme.Light;
            set
            {
                if (!value) return;
                SelectTheme = BaseTheme.Light;
            }
        }
        public bool IsInheritTheme
        {
            get => SelectTheme == BaseTheme.Inherit;
            set
            {
                if (!value) return;
                SelectTheme = BaseTheme.Inherit;
            }
        }

        private bool _isColorAdjusted;
        public bool IsColorAdjusted
        {
            get => _isColorAdjusted;
            set
            {
                SetProperty(ref _isColorAdjusted, value);
                ModifyTheme(theme =>
                {
                    if (theme is Theme internalTheme)
                    {
                        internalTheme.ColorAdjustment = value
                            ? new ColorAdjustment
                            {
                                DesiredContrastRatio = DesiredContrastRatio,
                                Contrast = ContrastValue,
                                Colors = ColorSelectionValue
                            }
                            : null;
                    }
                });
            }
        }

        private float _desiredContrastRatio = 4.5f;
        public float DesiredContrastRatio
        {
            get => _desiredContrastRatio;
            set
            {
                SetProperty(ref _desiredContrastRatio, value);

                ModifyTheme(theme =>
                {
                    if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                        internalTheme.ColorAdjustment.DesiredContrastRatio = value;
                });
            }
        }

        private Contrast _contrastValue;
        public Contrast ContrastValue
        {
            get => _contrastValue;
            set
            {
                SetProperty(ref _contrastValue, value);
                ModifyTheme(theme =>
                {
                    if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                        internalTheme.ColorAdjustment.Contrast = value;
                });
            }
        }
        public static IEnumerable<ColorSelection> ColorSelectionValues => Enum.GetValues(typeof(ColorSelection)).Cast<ColorSelection>();

        private ColorSelection _colorSelectionValue;
        public ColorSelection ColorSelectionValue
        {
            get => _colorSelectionValue;
            set
            {
                SetProperty(ref _colorSelectionValue, value);
                ModifyTheme(theme =>
                 {
                     if (theme is Theme internalTheme && internalTheme.ColorAdjustment != null)
                         internalTheme.ColorAdjustment.Colors = value;
                 });
            }
        }

        public static IEnumerable<Contrast> ContrastValues => Enum.GetValues(typeof(Contrast)).Cast<Contrast>();

        private readonly PaletteHelper _paletteHelper = new();
        private readonly Theme _theme;
        private readonly DatabaseContext Db = new();

        public string? OldPasswordString { get; set; }
        public string? NewPasswordString { get; set; }
        public string? NewSecondPasswordString { get; set; }

        private BaseTheme _thisTheme;
        private BaseTheme SelectTheme
        {
            get => _thisTheme;
            set
            {
                SetProperty(ref _thisTheme, value);
                _theme.SetBaseTheme(_thisTheme);
                _paletteHelper.SetTheme(_theme);

                OnPropertyChanged(nameof(IsDarkTheme));
                OnPropertyChanged(nameof(IsLightTheme));
                OnPropertyChanged(nameof(IsInheritTheme));
            }
        }

        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ChangePassword { get; set; }
        public ICommand ResetSettingCommand { get; set; }

        public SettingViewModel()
        {
            _theme = _paletteHelper.GetTheme();

            CancelCommand = new RelayCommand(Cancel);
            SaveCommand = new RelayCommand(Save);
            ChangePassword = new RelayCommand(ChangePass);
            ResetSettingCommand = new RelayCommand(ResetSetting);

            Init();

            if (_currentUser == null ||
                _users == null ||
                _employees == null)
            {
                Properties.Settings.Default.LogInUserId = -1;
                Properties.Settings.Default.Save();
#if !DEBUG
                System.Windows.MessageBox.Show("Не удалось загрузить пользователя. Возможно, сохранённый ID недействителен.\n" +
                    "Сохранённый пользователь не найден. Выполните вход заново.");
#endif
            }

            if (_theme is Theme internalTheme)
            {
                _isColorAdjusted = internalTheme.ColorAdjustment is not null;

                var colorAdjustment = internalTheme.ColorAdjustment ?? new ColorAdjustment();
                _desiredContrastRatio = colorAdjustment.DesiredContrastRatio;
                _contrastValue = colorAdjustment.Contrast;
                _colorSelectionValue = colorAdjustment.Colors;
            }
        }

        private void Init()
        {
            LoadData();
            Tools.ThemeManager.ApplySavedTheme();

            if (Enum.TryParse<BaseTheme>(Properties.Settings.Default.AppTheme, out var savedBase))
                SelectTheme = savedBase;
            else
                SelectTheme = BaseTheme.Inherit;

            if (Properties.Settings.Default.DesiredContrastRatio > 0 &&
    !string.IsNullOrWhiteSpace(Properties.Settings.Default.Contrast) &&
    Enum.TryParse<Contrast>(Properties.Settings.Default.Contrast, ignoreCase: true, out var savedContrast) &&
    Enum.IsDefined(typeof(Contrast), savedContrast))
            {
                DesiredContrastRatio = Properties.Settings.Default.DesiredContrastRatio;
                ContrastValue = savedContrast;
                IsColorAdjusted = true;
            }
            else
            {
                IsColorAdjusted = false;
            }

            SelectedAccountant = Employees.FirstOrDefault(e => e.Id == Properties.Settings.Default.ResponsibleAccountant);
            SelecteStorekeeper = Employees.FirstOrDefault(e => e.Id == Properties.Settings.Default.ResponsibleStorekeeper);
            SelectNds = Properties.Settings.Default.Nds;
            SelectedProductReleaseApprover = Employees.FirstOrDefault(e => e.Id == Properties.Settings.Default.ProductReleaseApprover);
            SelectedProductSubmitter = Employees.FirstOrDefault(e => e.Id == Properties.Settings.Default.ProductSubmitter);
            SelectCurrency = Properties.Settings.Default.Currency;

            var tempUsr = _users.FirstOrDefault(u => u.Id == Properties.Settings.Default.LogInUserId);
            if (tempUsr != null) CurrentUser = tempUsr;
        }

        private void Cancel(object obj) => Init();

        private void LoadData()
        {
            var emplDbSet = Db.Set<Employee>();
            emplDbSet.Include(m => m.Post).Load();
            Employees = emplDbSet.Local.ToObservableCollection();

            var usersDbSet = Db.Set<Users>();
            usersDbSet.Load();
            _users = usersDbSet.Local.ToObservableCollection();
        }

        public void ReloadData() => LoadData();

        private void Save(object obj)
        {
            if (
                SelectedAccountant == null ||
                SelecteStorekeeper == null ||
                SelectedProductReleaseApprover == null ||
                SelectedProductSubmitter == null
                ) return;

            Properties.Settings.Default.ResponsibleAccountant = SelectedAccountant.Id;
            Properties.Settings.Default.ResponsibleStorekeeper = SelecteStorekeeper.Id;
            Tools.ThemeManager.SaveTheme(SelectTheme);
            Properties.Settings.Default.Nds = SelectNds;
            Properties.Settings.Default.ProductReleaseApprover = SelectedProductReleaseApprover.Id;
            Properties.Settings.Default.ProductSubmitter = SelectedProductSubmitter.Id;
            Properties.Settings.Default.Currency = SelectCurrency;



            Properties.Settings.Default.Save();
            Db.SaveChanges();
        }

        private void ChangePass(object obj)
        {
            if (OldPasswordString == null || NewPasswordString == null || NewSecondPasswordString == null)
            {
                Message = "поля для пароля пусты";
                return;
            }

            if (NewPasswordString != NewSecondPasswordString)
            {
                Message = "пароли не совпадают";
                return;
            }

            string oldPassHash = CreateUserViewModel.HashPassword(OldPasswordString);
            if (oldPassHash == CurrentUser.Password)
            {
                string newPassHash = CreateUserViewModel.HashPassword(NewPasswordString);
                CurrentUser.Password = newPassHash;
                Db.SaveChanges();
                Message = "Пароль изменён";

                OldPasswordString = string.Empty; OnPropertyChanged(nameof(OldPasswordString));
                NewPasswordString = string.Empty; OnPropertyChanged(nameof(NewPasswordString));
                NewSecondPasswordString = string.Empty; OnPropertyChanged(nameof(NewSecondPasswordString));

                return;
            }
            else
            {
                Message = "Неверный старый пароль";
                return;
            }
        }

        private static void ModifyTheme(Action<Theme> modificationAction)
        {
            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }

        private void ResetSetting(object obj)
        {
            var userId = Properties.Settings.Default.LogInUserId;
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.LogInUserId = userId;
            Init();
        }
    }
}
