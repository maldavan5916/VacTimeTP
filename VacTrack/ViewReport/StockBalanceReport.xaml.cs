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
    /// Логика взаимодействия для StockBalanceReport.xaml
    /// </summary>
    public partial class StockBalanceReport : Page, ICachedPage
    {
        private EmployeeDivisionReportViewModel ThisViewModel => (EmployeeDivisionReportViewModel)DataContext;
        public StockBalanceReport() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public class StockBalanceReportViewModel : BaseReportViewModel<Material>
    {
        public ObservableCollection<Location>? MaterialLocation { get; set; }

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

        private Location? _FilterByLocation;
        public Location? FilterByLocation
        {
            get => _FilterByLocation;
            set
            {
                SetProperty(ref _FilterByLocation, value);
                LoadData();
            }
        }
        #endregion

        protected override void LoadData()
        {
            MaterialLocation = new ObservableCollection<Location>([.. Db.Locations]);

            DbSet = Db.Set<Material>();
            DbSet.Include(e => e.Unit).Include(e => e.Location).Load();

            Items = new ObservableCollection<Material>(DbSet.Local.Where(item =>
                FilterByLocation == null || item.Location?.Id == FilterByLocation.Id
                ).ToList());
        }

        public override FlowDocument CreateReport()
        {
            FlowDocument doc = new() { FontFamily = new FontFamily("Times New Roman"), FontSize = 12, PagePadding = new Thickness(50) };
            AddHeader(ref doc); // Добавляем заголовок Отчёта

            Table table = new() { CellSpacing = 3, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
            AddTableHeader(ref table);  // Добавляем заголовок таблицы

            TableRowGroup dataGroup = new();

            double totalSum = 0.0;

            if (IsGroupingEnabled)
                CreateGroupedRows(ref dataGroup, ref totalSum);
            else
                CreateNoGroupedRows(ref dataGroup, ref totalSum);

            if (AreOverallTotalsEnabled)
                dataGroup.Rows.Add(CreateRow(("Итого", "", "", "", "", $"{totalSum}")));

            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);
            return doc;
        }

        private void CreateGroupedRows(ref TableRowGroup dataGroup, ref double totalSum)
        {
            var groupedItems = Items.GroupBy(item => item.Location?.Name);

            foreach (var group in groupedItems)
            {
                dataGroup.Rows.Add(CreateRow(("", $"{group.Key}", "", "", "", "")));

                double summ = 0;
                foreach (var item in group)
                {
                    TableRow row = new();

                    double itemSum = item.GetSum;
                    summ += itemSum;

                    dataGroup.Rows.Add(CreateRow((
                        $"{item.Name}",
                        "", // Пустая ячейка , т.к. уже указано в заголовке группы
                        $"{item.Count}",
                        $"{item.Unit?.Name}",
                        $"{item.Price}",
                        $"{itemSum}")));

                    dataGroup.Rows.Add(row);
                }
                totalSum += summ;
                if (IsGroupTotalEnabled) dataGroup.Rows.Add(CreateRow(("Итого", "", "", "", "", $"{summ}")));
            }
        }

        private void CreateNoGroupedRows(ref TableRowGroup dataGroup, ref double totalSum)
        {
            foreach (var item in Items)
            {
                double summ = item.GetSum;
                totalSum += summ;

                dataGroup.Rows.Add(CreateRow((
                    $"{item.Name}",
                    $"{item.Location?.Name}",
                    $"{item.Count}",
                    $"{item.Unit?.Name}",
                    $"{item.Price}",
                    $"{summ}")));
            }
        }

        private static void AddTableHeader(ref Table table)
        {
            TableRowGroup headerGroup = new();
            TableRow headerRow = new();

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Наименование"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Место хранения"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Количество"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Единица измерения"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Стоимость за единицу"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Стоимость"))) { FontWeight = FontWeights.Bold });

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);
        }

        private void AddHeader(ref FlowDocument doc)
        {
            // Добавляем заголовок отчета
            Paragraph header = new(new Run("ООО \"ВакТайм\""))
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(header);

            Paragraph title = new(new Run("Отчет по остаткам материалов и комплектующих"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(title);


            string filteredText = FilterByLocation != null ? $"\nПо месту хранения: {FilterByLocation.Name}" : string.Empty;

            string headerText = $"Дата формирования: {DateTime.Now:dd.MM.yyyy}" + filteredText;

            Paragraph headerParagraph = new(new Run(headerText))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                LineHeight = 1.5 // Можно добавить межстрочный интервал
            };
            doc.Blocks.Add(headerParagraph);
        }

        static private TableRow CreateRow((string Col1, string Col2, string Col3, string Col4, string Col5, string Col6) data)
        {
            TableRow row = new();
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col1))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col2))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col3))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col4))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col5))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col6))));
            return row;
        }
    }
}
