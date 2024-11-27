using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для SaleViewTable.xaml
    /// </summary>
    public partial class SaleViewTable : Page, ICachedPage
    {
        private SaleViewModel ThisViewModel => (SaleViewModel)DataContext;
        public SaleViewTable() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public class SaleViewModel : BaseViewModel<Sale>
    {
        public ObservableCollection<Contract>? SaleContract { get; set; }

        public SaleViewModel() : base(new DatabaseContext()) { }

        protected override void LoadData()
        {
            TableName = "Реализация";
            SaleContract = new ObservableCollection<Contract>([.. Db.Contracts]);

            DbSet = Db.Set<Sale>();
            DbSet.Include(e => e.Contract).ThenInclude(c => c != null ? c.Product : null).Load();

            Items = DbSet.Local.ToObservableCollection();
        }

        protected override Sale CreateNewItem() => new() { Date = DateTime.Now };

        protected override bool FilterItem(Sale item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.Contract?.Product?.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Contract?.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Summ.ToString().Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;
    }
}
