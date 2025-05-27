using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для LocationViewTable.xaml
    /// </summary>
    public partial class LocationViewTable : Page, ICachedPage
    {
        private LocationViewModel ThisViewModel => (LocationViewModel)DataContext;
        public LocationViewTable() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public class LocationViewModel : BaseViewModel<Location>
    {
        public ObservableCollection<Employee>? LocalEmployee { get; set; }

        public LocationViewModel() : 
            base(new DatabaseContext(Tools.AppSession.CurrentUserIsReadOnly)) 
        { }

        protected override void LoadData()
        {
            TableName = "Места хранения";
            LocalEmployee = new ObservableCollection<Employee>([.. Db.Employees]);

            DbSet = Db.Set<Location>();
            DbSet.Include(e => e.Employee).Load();

            Items = DbSet.Local.ToObservableCollection();
        }

        protected override Location CreateNewItem() => new() { Name = "Новый склад" };

        protected override bool FilterItem(Location item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Employee?.Fio?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;
    }
}
