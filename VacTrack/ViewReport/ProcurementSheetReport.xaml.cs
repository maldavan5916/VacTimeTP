using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DatabaseManager;
using Microsoft.EntityFrameworkCore;

namespace VacTrack.ViewReport
{
    /// <summary>
    /// Interaction logic for ProcurementSheetReport.xaml
    /// </summary>
    public partial class ProcurementSheetReport : Page, ICachedPage
    {
        private ProcurementSheetReportViewModel ThisViewModel => (ProcurementSheetReportViewModel)DataContext;
        public ProcurementSheetReport() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public class ProcurementSheetReportViewModel : BaseReportViewModel<Product_Material>
    {
        public required List<Product> Products { get; set; }
        public required List<Material> Materials { get; set; }
        private List<Material> TotalBuyMaterials { get; set; } = [];

        #region properties
        private bool _IsGroupingEnabled = true;
        public bool IsGroupingEnabled
        {
            get => _IsGroupingEnabled;
            set
            {

                SetProperty(ref _IsGroupingEnabled, value);
                Refresh(null);
            }
        }

        private bool _IsGroupTotalEnabled = true;
        public bool IsGroupTotalEnabled
        {
            get => _IsGroupTotalEnabled;
            set
            {
                SetProperty(ref _IsGroupTotalEnabled, value);
                Refresh(null);
            }
        }

        private bool _AreOverallTotalsEnabled = true;
        public bool AreOverallTotalsEnabled
        {
            get => _AreOverallTotalsEnabled;
            set
            {
                SetProperty(ref _AreOverallTotalsEnabled, value);
                Refresh(null);
            }
        }

        private Product? _SelectedProduct;
        public Product? SelectedProduct
        {
            get => _SelectedProduct;
            set
            {
                SetProperty(ref _SelectedProduct, value);
                LoadData();
            }
        }
        #endregion

        protected override void LoadData()
        {
            Products = new List<Product>([.. Db.Products]);
            Materials = new List<Material>([.. Db.Materials]);
            TotalBuyMaterials.Clear();

            DbSet = Db.Set<Product_Material>();
            DbSet
                .Include(e => e.Material)
                    .ThenInclude(c => c != null ? c.Unit : null)
                .Include(e => e.Product)
            .Load();

            if (SelectedProduct == null)
                Items = DbSet.Local.ToObservableCollection();
            else
                Items = [.. DbSet.Local.Where(item => item.Product?.Id == SelectedProduct.Id).ToList()];
        }

        public override FlowDocument CreateReport()
        {
            FlowDocument doc = new() { FontFamily = new FontFamily("Times New Roman"), FontSize = 12, PagePadding = new Thickness(50) };

            AddHeader(ref doc, SelectedProduct); // Добавляем заголовок Отчёта

            Table table = new() { CellSpacing = 3, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };

            AddTableHeader(ref table);  // Добавляем заголовок таблицы

            // Данные таблицы
            TableRowGroup dataGroup = new();
            double totalSumm = 0;
            if (IsGroupingEnabled)
                CreateGroupedRows(ref dataGroup, ref totalSumm);
            else
                CreateUngroupedRows(ref dataGroup, ref totalSumm);


            if (AreOverallTotalsEnabled) dataGroup.Rows.Add(CreateRow(["Итого", "", "", "", "", "", $"{totalSumm}"]));

            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);

            doc.Blocks.Add(new Paragraph(new Run("Итого нужно:"))
            {
                FontSize = 12,
                Margin = new Thickness(0, 0, 0, 10)
            });
            doc.Blocks.Add(CreatePurchaseTable(TotalBuyMaterials));

            Employee? responsible = Db.Employees
                .Include(e => e.Post)
                .FirstOrDefault(e => e.Id == Properties.Settings.Default.ResponsibleAccountant);

            doc.Blocks.Add(new Paragraph(
                new Run($"{responsible?.Post?.Name}   _____________   {responsible?.Fio}"))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                LineHeight = 1.5 // Можно добавить межстрочный интервал
            });

            return doc;
        }

        private static Table CreatePurchaseTable(IEnumerable<Material> materials)
        {
            // Создаем таблицу и настраиваем основные параметры
            var table = new Table
            {
                CellSpacing = 0,
                Margin = new Thickness(5),
                TextAlignment = TextAlignment.Left,
                FontSize = 12
            };

            // Добавляем колонки с пропорциональными ширинами
            table.Columns.Add(new TableColumn { Width = new GridLength(3, GridUnitType.Star) }); // Наименование
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) }); // Количество
            table.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) }); // Цена
            table.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) }); // Сумма

            // Создаем группу строк
            var rowGroup = new TableRowGroup();

            // Добавляем заголовки
            var headerRow = CreateRow([
                "Наименование", 
                "Количество", 
                "Цена за ед.,\n" + Properties.Settings.Default.Currency, 
                "Сумма,\n" + Properties.Settings.Default.Currency
                ]);
            headerRow.FontWeight = FontWeights.Bold;
            rowGroup.Rows.Add(headerRow);

            // Добавляем данные
            foreach (var m in materials)
            {
                var count = $"{m.Count} {m.Unit?.Name ?? string.Empty}".Trim();
                var price = $"{m.Price:N2}";
                var sum = $"{m.GetSum:N2}";

                rowGroup.Rows.Add(CreateRow( [ m.Name, count, price, sum ]));
            }

            // Добавляем итоговую строку
            var totalRow = CreateRow([ "Итого:", string.Empty, string.Empty, $"{materials.Sum(m => m.GetSum):N2}" ]);
            totalRow.FontWeight = FontWeights.SemiBold;
            rowGroup.Rows.Add(totalRow);

            table.RowGroups.Add(rowGroup);
            return table;
        }

        private void CreateGroupedRows(ref TableRowGroup dataGroup, ref double totalSum)
        {
            Dictionary<int, double> materialDictionary = Materials.ToDictionary(m => m.Id, m => m.Count);

            var groupedItems = Items.GroupBy(item => item.Product?.Name);

            foreach (var group in groupedItems)
            {
                dataGroup.Rows.Add(CreateRow([$"{group.Key}", "", "", "", "", "", ""]));
                double summ = 0;

                foreach (var item in group)
                {
                    var unit = item.Material?.Unit?.Name ?? "";
                    var available = materialDictionary[item.MaterialId];
                    var shortage = GetShortage(item.Quantity, available);
                    var cost = CalculateCost(item.Material?.Price, shortage);

                    dataGroup.Rows.Add(CreateRow([
                        string.Empty,
                        $"{item.Material?.Name}",
                        $"{item.Quantity} {unit}",
                        $"{available} {unit}",
                        $"{shortage} {unit}",
                        $"{item.Material?.Price}",
                        $"{cost}"
                        ]));
                    summ += cost;

                    materialDictionary[item.MaterialId] =
                        Math.Max(0, materialDictionary.GetValueOrDefault(item.MaterialId) - item.Quantity);

                    if (shortage > 0 && item.Material != null) AddOrUpdateMaterial(item.Material, shortage);

                }

                if (IsGroupTotalEnabled) dataGroup.Rows.Add(CreateRow(["Итого", "", "", "", "", "", $"{summ}"]));

                totalSum += summ;
            }
        }

        private void CreateUngroupedRows(ref TableRowGroup dataGroup, ref double totalSumm)
        {
            Dictionary<int, double> materialDictionary = Materials.ToDictionary(m => m.Id, m => m.Count);

            foreach (var item in Items)
            {
                var unit = item.Material?.Unit?.Name ?? "";
                var available = materialDictionary[item.MaterialId];
                var shortage = GetShortage(item.Quantity, available);
                var cost = CalculateCost(item.Material?.Price, shortage);

                totalSumm += cost;

                dataGroup.Rows.Add(CreateRow([
                    $"{item.Product?.Name}",
                    $"{item.Material?.Name}",
                    $"{item.Quantity} {unit}",
                    $"{available} {unit}",
                    $"{shortage} {unit}",
                    $"{item.Material?.Price}",
                    $"{cost}"
                ]));

                materialDictionary[item.MaterialId] =
                    Math.Max(0, materialDictionary.GetValueOrDefault(item.MaterialId) - item.Quantity);

                if (shortage > 0 && item.Material != null) AddOrUpdateMaterial(item.Material, shortage);

            }
        }

        private static double GetShortage(double? required, double? available)
            => Math.Max(0, (required ?? 0) - (available ?? 0));

        private static double CalculateCost(double? price, double? count)
            => (price ?? 0) * (count ?? 0);

        private void AddOrUpdateMaterial(Material newMaterial, double shortage)
        {
            var existingMaterial = TotalBuyMaterials.FirstOrDefault(m => m.Id == newMaterial.Id);
            if (existingMaterial != null)
            {
                existingMaterial.Count += shortage;
            }
            else
            {
                TotalBuyMaterials.Add(new Material
                {
                    Id = newMaterial.Id,
                    Name = newMaterial.Name,
                    UnitId = newMaterial.UnitId,
                    Unit = newMaterial.Unit,
                    LocationId = newMaterial.LocationId,
                    Location = newMaterial.Location,
                    Count = shortage, // ------------- shortage
                    Price = newMaterial.Price,
                    ProductMaterials = newMaterial.ProductMaterials,
                    Receipts = newMaterial.Receipts,
                });
            }
        }


        private static void AddTableHeader(ref Table table)
        {
            TableRowGroup headerGroup = new();
            TableRow headerRow = new();

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Изделие"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("материал"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("необходимо"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Есть на складе"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Нехватет"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Стоимость,\n" + Properties.Settings.Default.Currency))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Сумма,\n" + Properties.Settings.Default.Currency))) { FontWeight = FontWeights.Bold });

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);
        }

        private static void AddHeader(ref FlowDocument doc, Product? SelectedProduct)
        {
            // Добавляем заголовок отчета
            Paragraph header = new(new Run("ООО \"ВакТайм\""))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(header);

            Paragraph title = new(new Run("Список закупок"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(title);


            string filteredText = SelectedProduct == null ? "" : $"\nДля изделия: {SelectedProduct.Name}";

            string headerText = $"Дата формирования: {DateTime.Now:dd.MM.yyyy}" + filteredText;

            Paragraph headerParagraph = new(new Run(headerText))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                LineHeight = 1.5 // Можно добавить межстрочный интервал
            };
            doc.Blocks.Add(headerParagraph);
        }
    }
}
