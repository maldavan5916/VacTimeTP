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
    public partial class MaterialUsageReport : Page
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


            if (AreOverallTotalsEnabled) dataGroup.Rows.Add(CreateRow(("Итого", "", "", "", "", "", $"{totalSumm}")));

            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);
            return doc;
        }

        private void CreateGroupedRows(ref TableRowGroup dataGroup, ref double totalSumm)
        {
            // Группируем элементы по названию изделия
            var groupedItems = Items.GroupBy(item => item.Product?.Name);

            foreach (var group in groupedItems)
            {
                dataGroup.Rows.Add(CreateRow(("", $"{group.Key}", "", "", "", "", "")));

                double summ = 0;
                // Добавляем строки для каждого материала в группе
                foreach (var item in group)
                {
                    TableRow row = new();

                    double price = item.GetSum;
                    summ += price;

                    dataGroup.Rows.Add(CreateRow((
                        $"{item.Id}",
                        "", // Пустая ячейка для "Изделие", т.к. уже указано в заголовке группы
                        $"{item.Material?.Name}",
                        $"{item.Quantity}",
                        $"{item.Material?.Unit?.Name}",
                        $"{item.Material?.Price}",
                        $"{price}")));

                    dataGroup.Rows.Add(row);
                }
                totalSumm += summ;
                if (IsGroupTotalEnabled) dataGroup.Rows.Add(CreateRow(("Итого", "", "", "", "", "", $"{summ}")));
            }
        }

        private void CreateUngroupedRows(ref TableRowGroup dataGroup, ref double totalSumm)
        {
            foreach (var item in Items)
            {
                double price = item.GetSum;
                totalSumm += price;

                dataGroup.Rows.Add(CreateRow((
                    $"{item.Id}",
                    $"{item.Product?.Name}",
                    $"{item.Material?.Name}",
                    $"{item.Quantity}",
                    $"{item.Material?.Unit?.Name}",
                    $"{item.Material?.Price}",
                    $"{price}")));
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

        static private TableRow CreateRow((string Col1, string Col2, string Col3, string Col4, string Col5, string Col6, string Col7) data)
        {
            TableRow row = new();
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col1))) { TextAlignment = TextAlignment.Center });
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col2))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col3))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col4))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col5))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col6))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col7))));
            return row;
        }
    }
}
