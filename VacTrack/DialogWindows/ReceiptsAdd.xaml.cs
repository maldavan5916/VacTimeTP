using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace VacTrack.DialogWindows
{
    /// <summary>
    /// Логика взаимодействия для ReceiptsAdd.xaml
    /// </summary>
    public partial class ReceiptsAdd : Window
    {
        private ReceiptsAddVM ThisViewModel => (ReceiptsAddVM)DataContext;
        public ObservableCollection<Receipt>? NewItems;
        public ReceiptsAdd()
        {
            InitializeComponent();
        }

        private void AcceptClick(object sender, RoutedEventArgs e)
        {
            if (ThisViewModel.SelectLocation != null &&
                ThisViewModel.SelectCounterpartie != null)
            {
                ThisViewModel.Message = string.Empty;
                NewItems = ThisViewModel.Items;
                DialogResult = NewItems != null;
            }
            else
            {
                ThisViewModel.Message = "Не все поля заполнены";
                ThisViewModel.MessageBrush = Brushes.Red;
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e) => DialogResult = false;
    }

    public class ReceiptsAddVM : ViewTables.BaseViewModel<Receipt>
    {

        public ObservableCollection<Counterpartie> ReceiptCounterpartie { get; set; }
        public ObservableCollection<Material> ReceiptMaterial { get; set; }
        public ObservableCollection<Location> ReceiptLocation { get; set; }

        private Counterpartie? _selectCounterpartie;
        public Counterpartie? SelectCounterpartie
        {
            get => _selectCounterpartie;
            set
            {
                _selectCounterpartie = value;
                foreach (var item in Items) item.Counterpartie = value;
                OnPropertyChanged(nameof(SelectCounterpartie));
            }
        }
        public Location? SelectLocation { get; set; }
        public DateTime SelectDate { get; set; }

        public ReceiptsAddVM() : base(new DatabaseContext())
        {
            var MaterDbSet = Db.Set<Material>();
            MaterDbSet.Include(m => m.Unit).Load();
            ReceiptMaterial = MaterDbSet.Local.ToObservableCollection();

            ReceiptCounterpartie = new ObservableCollection<Counterpartie>([.. Db.Counterparties]);
            ReceiptLocation = new ObservableCollection<Location>([.. Db.Locations]);

            SelectDate = DateTime.Now;
        }

        protected override void LoadData()
        {
            DbSet = Db.Set<Receipt>();
            Items = [];
        }

        protected override Receipt CreateNewItem() => new() { Date = SelectDate, Counterpartie = SelectCounterpartie };
        protected override bool FilterItem(Receipt item, string? searchText) => false;
    }
}
