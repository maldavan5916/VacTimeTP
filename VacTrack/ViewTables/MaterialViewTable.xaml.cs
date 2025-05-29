using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для MaterialViewTable.xaml
    /// </summary>
    public partial class MaterialViewTable : Page, ICachedPage
    {
        private MaterialViewModel ThisViewModel => (MaterialViewModel)DataContext;
        public MaterialViewTable() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.Refresh();
    }

    public class MaterialViewModel : BaseViewModel<Material>
    {
        public ObservableCollection<Unit>? MaterialUnit { get; set; }
        public ObservableCollection<Location>? MaterialLocation { get; set; }

        public MaterialViewModel() : 
            base(new DatabaseContext(Tools.AppSession.CurrentUserIsReadOnly)) 
        { }

        protected override void LoadData()
        {
            TableName = "Материалы\\Комплектующие";
            MaterialUnit = new ObservableCollection<Unit>([.. Db.Units]);
            MaterialLocation = new ObservableCollection<Location>([.. Db.Locations]);

            DbSet = Db.Set<Material>();
            DbSet.Include(e => e.Unit).Include(e => e.Location).Load();

            Items = DbSet.Local.ToObservableCollection();
        }

        protected override Material CreateNewItem() => new() { Name = "Новый материал" };

        protected override bool FilterItem(Material item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Unit?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Location?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;
    }
}
