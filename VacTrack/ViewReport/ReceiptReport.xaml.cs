using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace VacTrack.ViewReport
{
    /// <summary>
    /// Логика взаимодействия для ReceiptReport.xaml
    /// </summary>
    public partial class ReceiptReport : Window
    {
        private ReceiptReportViewModel ThisViewModel => (ReceiptReportViewModel)DataContext;
        public ReceiptReport(
            ObservableCollection<Receipt> items,
            DialogWindows.ReceiptsPrintVM receiptsPrintVM)
        {
            InitializeComponent();

            ThisViewModel.FilteredItems = items;

            ThisViewModel.SelectCounterpartie = receiptsPrintVM.SelectCounterpartie;
            ThisViewModel.SelectLocation = receiptsPrintVM.SelectLocation;
            ThisViewModel.SelectMaterial = receiptsPrintVM.SelectMaterial;
            ThisViewModel.SelectStartDate = receiptsPrintVM.SelectStartDate;
            ThisViewModel.SelectEndDate = receiptsPrintVM.SelectEndDate;
        }
    }

    public class ReceiptReportViewModel : BaseReportViewModel<Receipt>
    {
        #region properties
        private Counterpartie? _selectCounterpartie;
        public Counterpartie? SelectCounterpartie
        {
            get => _selectCounterpartie;
            set
            {
                if (_selectCounterpartie == value) return;
                SetProperty(ref _selectCounterpartie, value);
                Refresh(null);
            }
        }

        private Location? _selectLocation;
        public Location? SelectLocation
        {
            get => _selectLocation;
            set
            {
                Location? location = value != null ? Db.Locations
                        .Include(e => e.Employee)
                        .FirstOrDefault(e => e.Id == value.Id) : null;
                
                if (_selectLocation == location) return;
                
                SetProperty(ref _selectLocation, location);
                Refresh(null);
            }
        }

        private Material? _selectMaterial;
        public Material? SelectMaterial
        {
            get => _selectMaterial;
            set
            {
                if (_selectMaterial == value) return;
                SetProperty(ref _selectMaterial, value);
                Refresh(null);
            }
        }

        private DateTime? _selectStartDate;
        public DateTime? SelectStartDate
        {
            get => _selectStartDate;
            set
            {
                if (_selectStartDate == value) return;
                SetProperty(ref _selectStartDate, value);
                Refresh(null);
            }
        }

        private DateTime? _selectEndDate;
        public DateTime? SelectEndDate
        {
            get => _selectEndDate;
            set
            {
                if (_selectEndDate == value) return;
                SetProperty(ref _selectEndDate, value);
                Refresh(null);
            }
        }

        private ObservableCollection<Receipt>? _filteredItems;
        public ObservableCollection<Receipt>? FilteredItems
        {
            get => _filteredItems;
            set => SetProperty(ref _filteredItems, value);
        }
        #endregion

        public override FlowDocument CreateReport()
        {
            FlowDocument doc = new() { FontFamily = new FontFamily("Times New Roman"), FontSize = 12, PagePadding = new Thickness(50) };
            AddHeader(ref doc); // Добавляем заголовок Отчёта

            Table table = new() { CellSpacing = 3, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
            AddTableHeader(ref table);  // Добавляем заголовок таблицы

            TableRowGroup dataGroup = new();

            double totalSum = 0;

            CreateNoGroupedRows(ref dataGroup, ref totalSum);

            dataGroup.Rows.Add(CreateRow(["Итого", "X", "X", "X", $"{totalSum:N2}"]));


            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);

            Employee? responsible = SelectLocation == null ? 
               Db.Employees
                .Include(e => e.Post)
                .FirstOrDefault(e => e.Id == Properties.Settings.Default.ResponsibleStorekeeper) :
               Db.Employees
                .Include(e => e.Post)
                .FirstOrDefault(e => e.Id == SelectLocation.EmployeeId);

            Paragraph headerParagraph = new(new Run($"{responsible?.Post?.Name}  _____________  {responsible?.Fio}"))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                LineHeight = 1.5 // Можно добавить межстрочный интервал
            };
            doc.Blocks.Add(headerParagraph);

            return doc;
        }

        private void CreateNoGroupedRows(ref TableRowGroup dataGroup, ref double totalSum)
        {
            if (FilteredItems == null) return;

            foreach (Receipt item in FilteredItems)
            {
                dataGroup.Rows.Add(CreateRow([
                    $"{item.Material?.Name}",
                    $"{item.Count}",
                    $"{item.Material?.Unit?.Name}",
                    $"{item.Material?.Price}",
                    $"{item.Summ:N2}"
                    ]));
                totalSum += item.Summ;
            }
        }

        private static void AddTableHeader(ref Table table)
        {
            TableRowGroup headerGroup = new();
            TableRow headerRow = new();

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("ТМЦ"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Количество"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("ед.изм."))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("цена,\n" + Properties.Settings.Default.Currency))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Сумма,\n" + Properties.Settings.Default.Currency))) { FontWeight = FontWeights.Bold });

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);
        }

        private void AddHeader(ref FlowDocument doc)
        {
            string? startDate = SelectStartDate?.ToString("d MMMM yyyy 'г.'", new CultureInfo("ru-RU"));
            string? endDate = SelectEndDate?.ToString("d MMMM yyyy 'г.'", new CultureInfo("ru-RU"));
           
            string date = startDate != null && endDate != null ?
                $"За период с {startDate} по {endDate}" :
                startDate != null ? $"От {startDate}" : $"От {endDate}";

            Paragraph title = new(new Run($"Приходная накладная № {FilteredItems?[0]?.Id}\n" + date))
                {
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                };
            doc.Blocks.Add(title);

            string headerText = $"\nПоставщик: {SelectCounterpartie?.Name}" +
                $"\nСклад получатель: {SelectLocation?.Name}, {SelectLocation?.Employee?.Fio}";

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
