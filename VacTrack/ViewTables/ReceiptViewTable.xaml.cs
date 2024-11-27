using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для ReceiptViewTable.xaml
    /// </summary>
    public partial class ReceiptViewTable : Page, ICachedPage
    {
        private ReceiptViewModel ThisViewModel => (ReceiptViewModel)DataContext;
        public ReceiptViewTable() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public class ReceiptViewModel : BaseViewModel<Receipt>
    {
        public ObservableCollection<Counterpartie>? ReceiptCounterpartie { get; set; }
        public ObservableCollection<Material>? ReceiptMaterial{ get; set; }

        public ReceiptViewModel() : base(new DatabaseContext()) { }

        protected override void LoadData()
        {
            TableName = "Поступления";
            ReceiptCounterpartie = new ObservableCollection<Counterpartie>([.. Db.Counterparties]);
            ReceiptMaterial = new ObservableCollection<Material>([.. Db.Materials]);

            DbSet = Db.Set<Receipt>();
            DbSet.Include(e => e.Counterpartie).Include(e => e.Material).Load();

            Items = DbSet.Local.ToObservableCollection();
        }

        protected override Receipt CreateNewItem() => new() { Date = DateTime.Now };

        protected override bool FilterItem(Receipt item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.Counterpartie?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Material?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Summ.ToString().Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;
    }
}
