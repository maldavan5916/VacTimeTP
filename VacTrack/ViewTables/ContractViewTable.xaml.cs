using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для ContractViewTable.xaml
    /// </summary>
    public partial class ContractViewTable : Page, ICachedPage
    {
        private ContractViewModel ThisViewModel => (ContractViewModel)DataContext;
        public ContractViewTable() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public class ContractViewModel : BaseViewModel<Contract>
    {
        public ObservableCollection<Counterpartie>? ContractCounterpartie { get; set; }
        public ObservableCollection<Product>? ContractProduct { get; set; }

        public ContractViewModel() : base(new DatabaseContext()) { }

        protected override void LoadData()
        {
            TableName = "Договора";
            ContractCounterpartie = new ObservableCollection<Counterpartie>([.. Db.Counterparties]);
            ContractProduct = new ObservableCollection<Product>([.. Db.Products]);

            DbSet = Db.Set<Contract>();
            DbSet.Include(e => e.Counterpartie).Include(e => e.Product).Load();

            Items = DbSet.Local.ToObservableCollection();
        }

        protected override Contract CreateNewItem() => new() { Name="Новый договор", Date = DateTime.Now };

        protected override bool FilterItem(Contract item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.Counterpartie?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Product?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Summ.ToString().Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;
    }
}
