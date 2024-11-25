using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

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

        protected DbSet<Product_Material>? DbSetProdMater;
        public ObservableCollection<Product_Material>? ProdMater { get; set; }

        public ICommand AddMaterialCommand { get; }

        public ProductViewModel() : base(new DatabaseContext())
        {
            AddMaterialCommand = new RelayCommand(AddMaterial);
        }

        private void OnPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem) && sender is ProductViewModel pvm && pvm.SelectedItem != null)
                LoadProdMater(pvm.SelectedItem);
        }

        private void LoadProdMater(Product product)
        {
            // Выполняем один запрос к базе данных с учетом всех связанных данных
            var productMaterials = Db.Product_Materials
                .Include(e => e.Material) // Подгружаем связанные материалы
                .Where(e => e.ProductId == product.Id) // Фильтруем по нужному продукту
                .ToList(); // Выполняем запрос и загружаем данные в память

            // Создаем ObservableCollection на основе загруженных данных
            ProdMater = new ObservableCollection<Product_Material>(productMaterials);

            // Уведомляем об изменении свойства
            OnPropertyChanged(nameof(ProdMater));
        }

        private void AddMaterial(object obj)
        {
            throw new NotImplementedException();
        }

        protected override void LoadData()
        {
            TableName = "Изделия";
            ProductUnit = new ObservableCollection<Unit>([.. Db.Units]);
            ProductLocation = new ObservableCollection<Location>([.. Db.Locations]);

            DbSet = Db.Set<Product>();
            DbSet.Include(e => e.Unit).Include(e => e.Location).Load();

            Items = DbSet.Local.ToObservableCollection();
            PropertyChanged += OnPropertyChangedHandler;
        }

        protected override Product CreateNewItem() => new() { Name = "Новое изделие", SerialNo = "Серийный номер" };

        protected override bool FilterItem(Product item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Unit?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Location?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;
    }
}
