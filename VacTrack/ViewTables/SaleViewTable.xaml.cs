using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VacTrack.ViewReport;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для SaleViewTable.xaml
    /// </summary>
    public partial class SaleViewTable : Page, ICachedPage
    {
        private SaleViewModel ThisViewModel => (SaleViewModel)DataContext;
        public SaleViewTable() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.Refresh();
    }

    public class SaleViewModel : BaseViewModel<Sale>
    {
        public ObservableCollection<Contract>? SaleContract { get; set; }

        public ICommand PrintCommand { get; }
        public SaleViewModel() :
            base(new DatabaseContext(Tools.AppSession.CurrentUserIsReadOnly))
        {
            PrintCommand = new RelayCommand(PrintDoc);
        }

        protected override void LoadData()
        {
            TableName = "Выбытие";
            SaleContract = new ObservableCollection<Contract>([.. Db.Contracts]);

            DbSet = Db.Set<Sale>();
            DbSet
                .Include(e => e.Contract)
                    .ThenInclude(c => c != null ? c.Product : null)
                        .ThenInclude(p => p != null ? p.Unit : null)
                .Include(e => e.Contract)
                    .ThenInclude(c => c != null ? c.Counterpartie : null)
                .Load();

            Items = DbSet.Local.ToObservableCollection();
        }

        protected override Sale CreateNewItem() => new() { Date = DateTime.Now };

        protected override bool FilterItem(Sale item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.Contract?.Product?.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Contract?.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Summ.ToString().Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;

        private void PrintDoc(object obj)
        {
            if (SelectedItem == null)
            {
                Message = "Запись не выбранна";
                MessageBrush = Brushes.Orange;
                return;
            }
            new SaleReport(SelectedItem).Show();
        }
    }
}
