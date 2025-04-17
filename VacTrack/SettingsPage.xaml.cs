using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using DatabaseManager;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using VacTrack.ViewTables;

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

        private ObservableCollection<Employee> _employees;
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set => SetProperty(ref _employees, value);
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
        
        public bool IsDarkTheme {  
            get => SelectTheme == BaseTheme.Dark;
            set
            {
                if (!value) return;
                SelectTheme = BaseTheme.Dark;
            }
        }
        public bool IsLightTheme {
            get => SelectTheme == BaseTheme.Light;
            set
            {
                if (!value) return;
                SelectTheme = BaseTheme.Light;
            }
        }
        public bool IsInheritTheme {
            get => SelectTheme == BaseTheme.Inherit;
            set
            {
                if (!value) return;
                SelectTheme = BaseTheme.Inherit;
            }
        }

        private readonly PaletteHelper _paletteHelper = new();
        private readonly Theme _theme;
        private readonly DatabaseContext Db = new();
        
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

        public SettingViewModel()
        {
            _theme = _paletteHelper.GetTheme();

            CancelCommand = new RelayCommand(Cancel);
            SaveCommand = new RelayCommand(Save);

            Init();
            
            if (Employees == null || _employees == null) 
                throw new ArgumentNullException(nameof(Employees));
        }

        private void Init()
        {
            LoadData();

            SelectTheme = Properties.Settings.Default.AppTheme switch
                            {
                                "Dark" => BaseTheme.Dark,
                                "Light" => BaseTheme.Light,
                                "Inherit" => BaseTheme.Inherit,
                                _ => BaseTheme.Inherit,
                            };
            SelectedAccountant = Employees.FirstOrDefault(e => e.Id == Properties.Settings.Default.ResponsibleAccountant);
            SelecteStorekeeper = Employees.FirstOrDefault(e => e.Id == Properties.Settings.Default.ResponsibleStorekeeper);
            SelectNds = Properties.Settings.Default.Nds;
            SelectedProductReleaseApprover = Employees.FirstOrDefault(e => e.Id == Properties.Settings.Default.ProductReleaseApprover);
            SelectedProductSubmitter = Employees.FirstOrDefault(e => e.Id == Properties.Settings.Default.ProductSubmitter);
        }

        private void Cancel(object obj) => Init();

        private void LoadData()
        {
            var emplDbSet = Db.Set<Employee>();
            emplDbSet.Include(m => m.Post).Load();
            Employees = emplDbSet.Local.ToObservableCollection();
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
            Properties.Settings.Default.AppTheme = SelectTheme switch
                            {
                                BaseTheme.Dark => "Dark",
                                BaseTheme.Light => "Light",
                                BaseTheme.Inherit => "Inherit",
                                _ => "Inherit"
                            };
            Properties.Settings.Default.Nds = SelectNds;
            Properties.Settings.Default.ProductReleaseApprover = SelectedProductReleaseApprover.Id;
            Properties.Settings.Default.ProductSubmitter = SelectedProductSubmitter.Id;

            Properties.Settings.Default.Save();
        }
    }
}
