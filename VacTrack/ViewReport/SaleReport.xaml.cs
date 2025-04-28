using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using DatabaseManager;
using Microsoft.EntityFrameworkCore;

namespace VacTrack.ViewReport
{
    /// <summary>
    /// Interaction logic for SaleReport.xaml
    /// </summary>
    public partial class SaleReport : Window
    {
        private SaleReportViewModel ThisViewModel => (SaleReportViewModel)DataContext;
        public SaleReport(Sale sale)
        {
            InitializeComponent();
            ThisViewModel.SelectSale = sale;
        }
    }

    public class SaleReportViewModel : BaseReportViewModel<Sale>
    {
        private Sale? _selectSale;
        public Sale? SelectSale
        {
            get => _selectSale;
            set
            {
                SetProperty(ref _selectSale, value);
                Refresh(null);
            }
        }

        private readonly double _nds = Properties.Settings.Default.Nds / 100;

        public override FlowDocument CreateReport()
        {
            FlowDocument doc = new() { FontFamily = new FontFamily("Times New Roman"), FontSize = 12, PagePadding = new Thickness(50) };

            AddHeader(ref doc); // Добавляем заголовок Отчёта

            Table table = new() { CellSpacing = 3, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
            AddTableHeader(ref table);  // Добавляем заголовок таблицы

            TableRowGroup dataGroup = new();

            CreateNoGroupedRows(ref dataGroup);

            dataGroup.Rows.Add(CreateRow([
                "Итого",
                "X",
                $"{SelectSale?.Count}",
                $"{SelectSale?.Summ}",
                "X",
                $"{SelectSale?.Summ * _nds}",
                $"{SelectSale?.Summ * (1 + _nds)}"]));

            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);

            Employee? productReleaseApprover = Db.Employees
                .Include(e => e.Post)
                .FirstOrDefault(e => e.Id == Properties.Settings.Default.ProductReleaseApprover);

            Employee? productSubmitter = Db.Employees
                .Include(e => e.Post)
                .FirstOrDefault(e => e.Id == Properties.Settings.Default.ProductSubmitter);

            string footerText =
                $"Отпуск разрешил: {productReleaseApprover?.Post?.Name} _______________ {productReleaseApprover?.Fio}" +
                $"\nСдал грузоотправитель: {productSubmitter?.Post?.Name} _______________ {productSubmitter?.Fio}";

            Paragraph headerParagraph = new(new Run(footerText))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                LineHeight = 1.5 // Можно добавить межстрочный интервал
            };
            doc.Blocks.Add(headerParagraph);

            return doc;
        }

        private void CreateNoGroupedRows(ref TableRowGroup dataGroup)
        {
            dataGroup.Rows.Add(CreateRow([
                    $"{SelectSale?.Contract?.Product?.Name}",
                    $"{SelectSale?.Contract?.Product?.Unit?.Name}",
                    $"{SelectSale?.Count}",
                    $"{SelectSale?.Summ}",
                    $"{_nds * 100}",
                    $"{SelectSale?.Summ * _nds}",
                    $"{SelectSale?.Summ * (1 + _nds)}"
            ]));
        }

        private static void AddTableHeader(ref Table table)
        {
            TableRowGroup headerGroup = new();
            TableRow headerRow = new();

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Наименование товара"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Ед.изм."))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Количество"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Цена,\n" + Properties.Settings.Default.Currency))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Ставка НДС, %"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Сумма НДС,\n" + Properties.Settings.Default.Currency))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Сумма с НДС,\n" + Properties.Settings.Default.Currency))) { FontWeight = FontWeights.Bold });

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);
        }

        private void AddHeader(ref FlowDocument doc)
        {
            Paragraph title = new(new Run($"Товарная накладная №{SelectSale?.Id}\n" +
                $"От{SelectSale?.Date.ToString("d MMMM yyyy 'г.'", new CultureInfo("ru-RU"))}"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(title);

            string headerText =
                $"\nГрузоотправитель: ООО «ВакТайм», адресс: {Properties.Settings.Default.OrganizationAdress}" +
                $"\nГрузополучатель: {SelectSale?.Contract?.Counterpartie?.Name}, адрес: {SelectSale?.Contract?.Counterpartie?.PostalAddress}" +
                $"\nОснование отпуска: договор {SelectSale?.Contract?.Name} от {SelectSale?.Contract?.Date.ToString("dd.MM.yyyy", new CultureInfo("ru-RU"))}";

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
