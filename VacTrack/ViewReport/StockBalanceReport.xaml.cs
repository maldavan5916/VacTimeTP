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
        private StockBalanceReportViewModel ThisViewModel => (StockBalanceReportViewModel)DataContext;
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

            DbSet.Include(e => e.Unit)
                .Include(e => e.Location)
                .ThenInclude(e => e != null ? e.Employee : null)
                .Load();

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
                dataGroup.Rows.Add(CreateRow(["Итого", "", "", "", $"{totalSum:N2}"]));

            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);

            Employee? responsible = Db.Employees
                .Include(e => e.Post)
                .FirstOrDefault(e => e.Id == Properties.Settings.Default.ResponsibleStorekeeper);

            doc.Blocks.Add( new Paragraph(
                new Run(FilterByLocation != null ? $"      _____________   { FilterByLocation.Employee?.Fio}" :
                $"{responsible?.Post?.Name}    _____________   {responsible?.Fio}"))
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
                item => item.Location?.Name,
                item => item.GetSum,
                key => ["", $"{key}", "", "", ""],
                total => ["Итого", "", "", "", $"{total:N2}"],
                item => [
                    $"{item.Name}",
                    String.Empty,
                    $"{item.Count} {item.Unit?.Name}",
                    $"{item.Price:N2}",
                    $"{item.GetSum:N2}"
                    ],
                ref totalSum,
                IsGroupTotalEnabled);
        }

        private void CreateNoGroupedRows(ref TableRowGroup dataGroup, ref double totalSum)
        {
            foreach (var item in Items)
            {
                double summ = item.GetSum;
                totalSum += summ;

                dataGroup.Rows.Add(CreateRow([
                    $"{item.Name}",
                    $"{item.Location?.Name}",
                    $"{item.Count} {item.Unit?.Name}",
                    $"{item.Price:N2}",
                    $"{summ:N2}"
                    ]));
            }
        }

        private static void AddTableHeader(ref Table table)
        {
            TableRowGroup headerGroup = new();
            TableRow headerRow = new();

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Наименование"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Место хранения"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Количество"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Стоимость,\n" + Properties.Settings.Default.Currency))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Сумма,\n" + Properties.Settings.Default.Currency))) { FontWeight = FontWeights.Bold });

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


            string filteredText = FilterByLocation != null ? 
                $"\nПо месту хранения: {FilterByLocation.Name}, Ответственный: {FilterByLocation.Employee?.Fio}" 
                : string.Empty;

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
