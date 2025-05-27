using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        public ObservableCollection<Material>? ProductMaterials { get; set; }

        public ObservableCollection<Product_Material>? ProdMater { get; set; }

        private Product_Material? _SelectedMaterial;
        public Product_Material? SelectedMaterial
        {
            get => _SelectedMaterial;
            set
            {
                if (!EqualityComparer<Product_Material?>.Default.Equals(_SelectedMaterial, value))
                {
                    _SelectedMaterial = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        private Product? LastSelectedMaterial;

        public ICommand AddMaterialCommand { get; }
        public ICommand DeleteMaterialCommand { get; }

        public ProductViewModel() : 
            base(new DatabaseContext(Tools.AppSession.CurrentUserIsReadOnly))
        {
            AddMaterialCommand = new RelayCommand(AddMaterial);
            DeleteMaterialCommand = new RelayCommand(DeleteMaterial);

            PropertyChanged += OnPropertyChangedHandler;
        }

        private void OnPropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem) && sender is ProductViewModel pvm)
                LoadProdMater(pvm.SelectedItem);
        }

        private void LoadProdMater(Product? product)
        {
            if (product == null)
                ProdMater?.Clear();
            else
            {
                if (LastSelectedMaterial == product) return;
                // Выполняем один запрос к базе данных с учетом всех связанных данных
                var productMaterials = Db.Product_Materials
                    .Include(e => e.Material) // Подгружаем связанные материалы
                    .Where(e => e.ProductId == product.Id) // Фильтруем по нужному продукту
                    .ToList(); // Выполняем запрос и загружаем данные в память

                // Создаем ObservableCollection на основе загруженных данных
                ProdMater = [.. productMaterials];
                LastSelectedMaterial = product;
                // Уведомляем об изменении свойства
                OnPropertyChanged(nameof(ProdMater));
            }
        }

        private void AddMaterial(object obj)
        {
            try
            {
                if (ProdMater != null || ProdMater?.Count == 0)
                {
                    if (SelectedItem == null) throw new Exception();
                    var newPM = new Product_Material() { ProductId = SelectedItem.Id, Product = SelectedItem };

                    ProdMater.Add(newPM);
                    Db.Product_Materials.Add(newPM);

                    Message = "Материал добавлен";
                    MessageBrush = Brushes.Green;
                }
                else
                {
                    Message = "Выберите изделие";
                    MessageBrush = Brushes.Orange;
                }
            }
            catch (Exception ex)
            {
                Message = $"Ошибка при добавлении материала: {ex.Message}";
                MessageBrush = Brushes.Red;
            }
        }

        private void DeleteMaterial(object obj)
        {
            try
            {
                if (SelectedMaterial != null && ProdMater != null)
                {
                    Db.Product_Materials.Remove(SelectedMaterial);
                    ProdMater.Remove(SelectedMaterial);

                    Message = "Материал удален";
                    MessageBrush = Brushes.Green;
                }
                else
                {
                    Message = "Выберите материал для удаления.";
                    MessageBrush = Brushes.Orange;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при удалении материала: {ex.Message}");
                MessageBrush = Brushes.Red;
            }
        }

        protected override void LoadData()
        {
            TableName = "Продукция";
            ProductUnit = new ObservableCollection<Unit>([.. Db.Units]);
            ProductLocation = new ObservableCollection<Location>([.. Db.Locations]);
            ProductMaterials = new ObservableCollection<Material>([.. Db.Materials]);

            DbSet = Db.Set<Product>();
            DbSet.Include(e => e.Unit).Include(e => e.Location).Load();

            Items = DbSet.Local.ToObservableCollection();
            foreach (var item in Items) { item.Nds = Properties.Settings.Default.Nds; }
        }

        protected override Product CreateNewItem() => new()
        {
            Name = "Новое изделие",
            SerialNo = "Серийный номер",
            Nds = Properties.Settings.Default.Nds
        };

        protected override bool FilterItem(Product item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Unit?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Location?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;
    }
}
