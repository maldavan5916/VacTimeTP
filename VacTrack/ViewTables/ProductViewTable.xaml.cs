using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для ProductViewTable.xaml
    /// </summary>
    public partial class ProductViewTable : Page, ICachedPage
    {
        private ProductViewModel ThisViewModel => (ProductViewModel)DataContext;
        public ProductViewTable() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public class ProductViewModel : BaseViewModel<Product>
    {
        public ObservableCollection<Unit>? ProductUnit { get; set; }
        public ObservableCollection<Location>? ProductLocation { get; set; }

        protected DbSet<Product_Material> product_Materials;
        public ObservableCollection<Product_Material> ProdMater { get; set; }

        public ProductViewModel() : base(new DatabaseContext()) 
        {
            product_Materials = Db.Product_Materials;
            product_Materials.Include(e => e.Product).Include(e => e.Material);
            product_Materials.Load();
            ProdMater = product_Materials.Local.ToObservableCollection();
        }

        protected override void LoadData()
        {
            TableName = "Изделия";
            ProductUnit = new ObservableCollection<Unit>([.. Db.Units]);
            ProductLocation = new ObservableCollection<Location>([.. Db.Locations]);

            DbSet = Db.Set<Product>();
            DbSet.Include(e => e.Unit).Include(e => e.Location).Load();

            Items = DbSet.Local.ToObservableCollection();
        }

        protected override Product CreateNewItem() => new() { Name = "Новое изделие", SerialNo = "Серийный номер" };

        protected override bool FilterItem(Product item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Unit?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Location?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;
    }
}
