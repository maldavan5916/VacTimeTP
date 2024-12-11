using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace VacTrack.ViewReport
{
    /// <summary>
    /// Логика взаимодействия для MaterialUsageReport.xaml
    /// </summary>
    public partial class MaterialUsageReport : Page, ICachedPage
    {
        private MaterialUsageReportViewModel ThisViewModel => (MaterialUsageReportViewModel)DataContext;
        public MaterialUsageReport() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public class MaterialUsageReportViewModel : BaseReportViewModel<Product_Material>
    {
        public List<Product>? Products { get; set; }

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

            DbSet = Db.Set<Product_Material>();
            DbSet.Include(e => e.Material).ThenInclude(c => c != null ? c.Unit : null).Include(e => e.Product).Load();

            if (SelectedProduct == null)
                Items = DbSet.Local.ToObservableCollection();
            else
                Items = new ObservableCollection<Product_Material>(DbSet.Local.Where(
                    item => item.Product?.Id == SelectedProduct.Id).ToList());
        }

        public override FlowDocument CreateReport()
        {
            FlowDocument doc = new() { FontFamily = new FontFamily("Times New Roman"), FontSize = 12, PagePadding = new Thickness(50) };

            AddHeader(ref doc, SelectedProduct); // Добавляем заголовок Отчёта

            Table table = new() { CellSpacing = 3, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
            table.Columns.Add(new TableColumn { Width = new GridLength(40) });

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

            doc.Blocks.Add(new Paragraph(
                new Run($"      _____________   {new ObservableCollection<Employee>([.. Db.Employees]).FirstOrDefault(e => e.Id == Properties.Settings.Default.ResponsibleAccountant)?.Fio}"))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                LineHeight = 1.5 // Можно добавить межстрочный интервал
            });

            return doc;
        }

        private void CreateGroupedRows(ref TableRowGroup dataGroup, ref double totalSum)
        {
            CreateGroupedRows(
                ref dataGroup,
                item => item.Product?.Name,
                item => item.GetSum,
                key => ["", $"{key}", "", "", "", "", ""],
                total => ["Итого", "", "", "", "", "", $"{total}"],
                item => [
                    $"{item.Id}",
                    String.Empty,
                    $"{item.Material?.Name}",
                    $"{item.Quantity}",
                    $"{item.Material?.Unit?.Name}",
                    $"{item.Material?.Price}",
                    $"{item.GetSum}"
                    ],
                ref totalSum,
                IsGroupTotalEnabled);
        }

        private void CreateUngroupedRows(ref TableRowGroup dataGroup, ref double totalSumm)
        {
            foreach (var item in Items)
            {
                double price = item.GetSum;
                totalSumm += price;

                dataGroup.Rows.Add(CreateRow([
                    $"{item.Id}",
                    $"{item.Product?.Name}",
                    $"{item.Material?.Name}",
                    $"{item.Quantity}",
                    $"{item.Material?.Unit?.Name}",
                    $"{item.Material?.Price}",
                    $"{price}"]));
            }
        }

        private static void AddTableHeader(ref Table table)
        {
            TableRowGroup headerGroup = new();
            TableRow headerRow = new();

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Код"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Наименование изделия"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Наименование материала"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Количество"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Единица измерения"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Стоимость за единицу"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Стоимость"))) { FontWeight = FontWeights.Bold });

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

            Paragraph title = new(new Run("Отчет по использованию материалов на изделия"))
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
