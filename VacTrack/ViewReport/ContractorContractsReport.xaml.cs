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
    /// Логика взаимодействия для ContractorContractsReport.xaml
    /// </summary>
    public partial class ContractorContractsReport : Page
    {
        private ContractorContractsReportViewModel ThisViewModel => (ContractorContractsReportViewModel)DataContext;
        public ContractorContractsReport() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public enum ContractGroupedType { NoGrouped, GroupedByCounterpartie, GroupedByProduct }

    public class ContractorContractsReportViewModel : BaseReportViewModel<Contract>
    {
        public ObservableCollection<Counterpartie>? ContractCounterpartie { get; set; }
        public ObservableCollection<Product>? ContractProduct { get; set; }

        #region properties
        private ContractGroupedType GroupedType = ContractGroupedType.NoGrouped;

        public bool IsGroupedTypeNoGrouped
        {
            get => GroupedType == ContractGroupedType.NoGrouped;
            set
            {
                if (value)
                {
                    GroupedType = ContractGroupedType.NoGrouped;
                    Refresh(null);
                    OnPropertyChanged(nameof(IsGroupedTypeNoGrouped));
                }
            }
        }

        public bool IsGroupedTypeGroupedByCounterpartie
        {
            get => GroupedType == ContractGroupedType.GroupedByCounterpartie;
            set
            {
                if (value)
                {
                    GroupedType = ContractGroupedType.GroupedByCounterpartie;
                    Refresh(null);
                    OnPropertyChanged(nameof(IsGroupedTypeGroupedByCounterpartie));
                    OnPropertyChanged(nameof(IsGroupedTypeNoGrouped));
                }
            }
        }

        public bool IsGroupedTypeGroupedByProduct
        {
            get => GroupedType == ContractGroupedType.GroupedByProduct;
            set
            {
                if (value)
                {
                    GroupedType = ContractGroupedType.GroupedByProduct;
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

        public ContractorContractsReportViewModel()
        {
            ClearFilterCommand = new RelayCommand(ClearFilter);
        }

        protected override void LoadData()
        {
            ContractCounterpartie = new ObservableCollection<Counterpartie>([.. Db.Counterparties]);
            ContractProduct = new ObservableCollection<Product>([.. Db.Products]);

            DbSet = Db.Set<Contract>();
            DbSet.Include(e => e.Counterpartie)
                .Include(e => e.Product)
                .ThenInclude(e => e != null ? e.Unit : null)
                .Load();

            Items = [.. DbSet.Local.Where(item =>
                (FilterByCounterpartie == null || item.Counterpartie?.Id == FilterByCounterpartie.Id) &&
                (FilterByProduct == null || item.Product?.Id == FilterByProduct.Id) &&

                ((FilterStartDate == null && FilterEndDate == null) ||

                (FilterStartDate != null && FilterEndDate != null &&
                    item.Date >= FilterStartDate && item.Date <= FilterEndDate) ||

                (FilterStartDate != null && FilterEndDate == null && item.Date == FilterStartDate) ||
                (FilterStartDate == null && FilterEndDate != null && item.Date == FilterEndDate))
                ).ToList()];
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
                case ContractGroupedType.GroupedByCounterpartie: CreateGroupedByCounterpartie(ref dataGroup, ref totalSum); break;
                case ContractGroupedType.GroupedByProduct: CreateGroupedByProduct(ref dataGroup, ref totalSum); break;
                case ContractGroupedType.NoGrouped: CreateNoGroupedRows(ref dataGroup, ref totalSum); break;
            }

            if (AreOverallTotalsEnabled)
                dataGroup.Rows.Add(CreateRow(["Итого", "", "", "", "", "", $"{totalSum}"]));

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

        private void CreateGroupedByProduct(ref TableRowGroup dataGroup, ref double totalSum)
        {
            CreateGroupedRows(
                ref dataGroup,
                item => item.Product?.Name,
                item => item.Summ,
                item => item.Count,
                key => ["", "", "", $"{key}", "", "", ""],
                cntTotal => ["Итого", "", "", "", $"{cntTotal.Item1}", "", $"{cntTotal.Item2}"],
                item => [
                    $"{item.Name}",
                    $"{item.Counterpartie?.Name}",
                    $"{item.Date:dd.MM.yyyy}",
                    string.Empty,
                    $"{item.Count}",
                    $"{item.Product?.Unit?.Name}",
                    $"{item.Summ}"
                    ],
                ref totalSum,
                IsGroupTotalEnabled);
        }

        private void CreateGroupedByCounterpartie(ref TableRowGroup dataGroup, ref double totalSum)
        {
            CreateGroupedRows(
                ref dataGroup,
                item => item.Counterpartie?.Name,
                item => item.Summ,
                key => ["", $"{key}", "", "", "", "", ""],
                total => ["Итого", "", "", "", "", "", $"{total}"],
                item => [
                    $"{item.Name}",
                    String.Empty,
                    $"{item.Date:dd.MM.yyyy}",
                    $"{item.Product?.Name}",
                    $"{item.Count}",
                    $"{item.Product?.Unit?.Name}",
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
                    $"{item.Name}",
                    $"{item.Counterpartie?.Name}",
                    $"{item.Date:dd.MM.yyyy}",
                    $"{item.Product?.Name}",
                    $"{item.Count}",
                    $"{item.Product?.Unit?.Name}",
                    $"{item.Summ}"]));
                totalSum += item.Summ;
            }
        }

        private static void AddTableHeader(ref Table table)
        {
            TableRowGroup headerGroup = new();
            TableRow headerRow = new();

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Номер договора"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Контрагент"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Дата заключения"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Изделие"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Количество"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Ед.изм."))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Сумма,\n"+Properties.Settings.Default.Currency))) { FontWeight = FontWeights.Bold });

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

            Paragraph title = new(new Run("Отчет по договорам с контрагентами"))
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
