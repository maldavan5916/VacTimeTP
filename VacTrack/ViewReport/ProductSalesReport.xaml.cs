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
    /// Логика взаимодействия для ProductSalesReport.xaml
    /// </summary>
    public partial class ProductSalesReport : Page
    {
        private ProductSalesReportViewModel ThisViewModel => (ProductSalesReportViewModel)DataContext;
        public ProductSalesReport() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public enum SaleGroupedType { NoGrouped, GroupedByCounterpartie, GroupedByProduct }

    public class ProductSalesReportViewModel : BaseReportViewModel<Sale>
    {
        public ObservableCollection<Counterpartie>? ContractCounterpartie { get; set; }
        public ObservableCollection<Product>? ContractProduct { get; set; }

        #region properties
        private SaleGroupedType GroupedType = SaleGroupedType.NoGrouped;

        public bool IsGroupedTypeNoGrouped
        {
            get => GroupedType == SaleGroupedType.NoGrouped;
            set
            {
                if (value)
                {
                    GroupedType = SaleGroupedType.NoGrouped;
                    Refresh(null);
                    OnPropertyChanged(nameof(IsGroupedTypeNoGrouped));
                }
            }
        }

        public bool IsGroupedTypeGroupedByCounterpartie
        {
            get => GroupedType == SaleGroupedType.GroupedByCounterpartie;
            set
            {
                if (value)
                {
                    GroupedType = SaleGroupedType.GroupedByCounterpartie;
                    Refresh(null);
                    OnPropertyChanged(nameof(IsGroupedTypeGroupedByCounterpartie));
                    OnPropertyChanged(nameof(IsGroupedTypeNoGrouped));
                }
            }
        }

        public bool IsGroupedTypeGroupedByProduct
        {
            get => GroupedType == SaleGroupedType.GroupedByProduct;
            set
            {
                if (value)
                {
                    GroupedType = SaleGroupedType.GroupedByProduct;
                    Refresh(null);
                    OnPropertyChanged(nameof(IsGroupedTypeGroupedByProduct));
                    OnPropertyChanged(nameof(IsGroupedTypeNoGrouped));
                }
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

        private Counterpartie? _FilterByCounterpartie;
        public Counterpartie? FilterByCounterpartie
        {
            get => _FilterByCounterpartie;
            set
            {
                SetProperty(ref _FilterByCounterpartie, value);
                LoadData();
            }
        }

        private Product? _FilterByProduct;
        public Product? FilterByProduct
        {
            get => _FilterByProduct;
            set
            {
                SetProperty(ref _FilterByProduct, value);
                LoadData();
            }
        }

        private DateTime? _FilterStartDate;
        public DateTime? FilterStartDate
        {
            get => _FilterStartDate;
            set
            {
                SetProperty(ref _FilterStartDate, value);
                LoadData();
            }
        }

        private DateTime? _FilterEndDate;
        public DateTime? FilterEndDate
        {
            get => _FilterEndDate;
            set
            {
                SetProperty(ref _FilterEndDate, value);
                LoadData();
            }
        }
        #endregion

        public System.Windows.Input.ICommand ClearFilterCommand { get; }

        public ProductSalesReportViewModel()
        {
            ClearFilterCommand = new RelayCommand(ClearFilter);
        }

        protected override void LoadData()
        {
            ContractCounterpartie = new ObservableCollection<Counterpartie>([.. Db.Counterparties]);
            ContractProduct = new ObservableCollection<Product>([.. Db.Products]);

            DbSet = Db.Set<Sale>();
            DbSet.Include(e => e.Contract)
                .Include(c => c.Contract != null ? c.Contract.Product : null)
                .ThenInclude(e => e != null ? e.Unit : null)
                .Include(c => c.Contract != null ? c.Contract.Counterpartie : null)
                .Load();

            Items = new ObservableCollection<Sale>(
                DbSet.Local.Where(item =>
                (FilterByCounterpartie == null || item.Contract?.Counterpartie?.Id == FilterByCounterpartie.Id) &&
                (FilterByProduct == null || item.Contract?.Product?.Id == FilterByProduct.Id) &&

                ((FilterStartDate == null && FilterEndDate == null) ||

                (FilterStartDate != null && FilterEndDate != null &&
                    item.Date >= FilterStartDate && item.Date <= FilterEndDate) ||

                (FilterStartDate != null && FilterEndDate == null && item.Date == FilterStartDate) ||
                (FilterStartDate == null && FilterEndDate != null && item.Date == FilterEndDate))
                ).ToList());
        }

        public override FlowDocument CreateReport()
        {
            FlowDocument doc = new() { FontFamily = new FontFamily("Times New Roman"), FontSize = 12, PagePadding = new Thickness(50) };
            AddHeader(ref doc); // Добавляем заголовок Отчёта

            Table table = new() { CellSpacing = 3, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
            AddTableHeader(ref table);  // Добавляем заголовок таблицы

            TableRowGroup dataGroup = new();

            double totalSum = 0;

            switch (GroupedType)
            {
                case SaleGroupedType.GroupedByCounterpartie: CreateGroupedByCounterpartie(ref dataGroup, ref totalSum); break;
                case SaleGroupedType.GroupedByProduct: CreateGroupedByProduct(ref dataGroup, ref totalSum); break;
                case SaleGroupedType.NoGrouped: CreateNoGroupedRows(ref dataGroup, ref totalSum); break;
            }

            if (AreOverallTotalsEnabled)
                dataGroup.Rows.Add(CreateRow(["Итого", "", "", "", "", $"{totalSum}"]));

            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);

            Employee? responsible = Db.Employees
                .Include(e => e.Post)
                .FirstOrDefault(e => e.Id == Properties.Settings.Default.ResponsibleAccountant);

            doc.Blocks.Add(new Paragraph(
                new Run($"{responsible?.Post?.Name}    _____________   {responsible?.Fio}"))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                LineHeight = 1.5 // Можно добавить межстрочный интервал
            });

            return doc;
        }

        private void CreateGroupedByCounterpartie(ref TableRowGroup dataGroup, ref double totalSum)
        {
            CreateGroupedRows(
                ref dataGroup,
                item => item.Contract?.Counterpartie?.Name,
                item => item.Summ,
                key => ["", $"{key}", "", "", "", ""],
                total => ["Итого", "", "", "", "", $"{total}"],
                item => [
                    $"{item.Date:dd.MM.yyyy}",
                    String.Empty,
                    $"{item.Contract?.Product?.Name}",
                    $"{item.Count}",
                    $"{item.Contract?.Product?.Unit?.Name}",
                    $"{item.Summ}"
                    ],
                ref totalSum,
                IsGroupTotalEnabled);
        }

        private void CreateGroupedByProduct(ref TableRowGroup dataGroup, ref double totalSum)
        {
            CreateGroupedRows(
                ref dataGroup,
                item => item.Contract?.Product?.Name,
                item => item.Summ,
                item => item.Count,
                key => ["", "", $"{key}", "", "", ""],
                cntTotal => ["Итого", "", "", $"{cntTotal.Item1}", "", $"{cntTotal.Item2}"],
                item => [
                    $"{item.Date:dd.MM.yyyy}",
                    $"{item.Contract?.Counterpartie?.Name}",
                    String.Empty,
                    $"{item.Count}",
                    $"{item.Contract?.Product?.Unit?.Name}",
                    $"{item.Summ}"
                    ],
                ref totalSum,
                IsGroupTotalEnabled);
        }

        private void CreateNoGroupedRows(ref TableRowGroup dataGroup, ref double totalSum)
        {
            foreach (var item in Items)
            {
                dataGroup.Rows.Add(CreateRow([
                    $"{item.Date:dd.MM.yyyy}",
                    $"{item.Contract?.Counterpartie?.Name}",
                    $"{item.Contract?.Product?.Name}",
                    $"{item.Count}",
                    $"{item.Contract?.Product?.Unit?.Name}",
                    $"{item.Summ}"]));
                totalSum += item.Summ;
            }
        }

        private static void AddTableHeader(ref Table table)
        {
            TableRowGroup headerGroup = new();
            TableRow headerRow = new();

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Дата реализации"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Контрагент"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Изделие"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Количество"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Ед.изм."))) { FontWeight = FontWeights.Bold });
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

            Paragraph title = new(new Run("Отчет по реализации продукции"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(title);

            string filterByPost = FilterByCounterpartie == null ? string.Empty : $"\nПо контрагенту: {FilterByCounterpartie.Name}";
            string filterByDivision = FilterByProduct == null ? string.Empty : $"\nПо изделию: {FilterByProduct.Name}";

            string periodFilter = (FilterStartDate, FilterEndDate) switch
            {
                (not null, not null) => $"\nЗа период с {FilterStartDate:dd.MM.yyyy} по {FilterEndDate:dd.MM.yyyy}",
                (not null, null) => $"\nНа день {FilterStartDate:dd.MM.yyyy}",
                (null, not null) => $"\nНа день {FilterEndDate:dd.MM.yyyy}",
                _ => string.Empty
            };

            string filteredText = filterByPost + filterByDivision + periodFilter;

            string headerText = $"Дата формирования: {DateTime.Now:dd.MM.yyyy}" + filteredText;

            Paragraph headerParagraph = new(new Run(headerText))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                LineHeight = 1.5 // Можно добавить межстрочный интервал
            };
            doc.Blocks.Add(headerParagraph);
        }

        private void ClearFilter(object obj)
        {
            FilterByCounterpartie = null;
            FilterByProduct = null;
            FilterStartDate = null;
            FilterEndDate = null;
        }
    }
}
