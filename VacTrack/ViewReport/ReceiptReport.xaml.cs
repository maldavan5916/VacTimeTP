using DatabaseManager;
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
        public ReceiptReport(Receipt receipt)
        {
            InitializeComponent();
            ThisViewModel.SelectedReceipt = receipt;
        }
    }

    public class ReceiptReportViewModel : BaseReportViewModel<Receipt>
    {
        private Receipt? _SelectedReceipt;
        public Receipt? SelectedReceipt
        {
            get => _SelectedReceipt;
            set
            {
                SetProperty(ref _SelectedReceipt, value);
                Refresh(null);
            }
        }

        public override FlowDocument CreateReport()
        {
            FlowDocument doc = new() { FontFamily = new FontFamily("Times New Roman"), FontSize = 12, PagePadding = new Thickness(50) };
            AddHeader(ref doc); // Добавляем заголовок Отчёта

            Table table = new() { CellSpacing = 3, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
            AddTableHeader(ref table);  // Добавляем заголовок таблицы

            TableRowGroup dataGroup = new();

            double totalSum = 0;

            CreateNoGroupedRows(ref dataGroup, ref totalSum);

            dataGroup.Rows.Add(CreateRow(["Итого", "", "", "", $"{totalSum}"]));


            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);

            Paragraph headerParagraph = new(new Run($"      _____________   {SelectedReceipt?.Material?.Location?.Employee?.Fio}"))
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
            dataGroup.Rows.Add(CreateRow([
                $"{SelectedReceipt?.Material?.Name}",
                    $"{SelectedReceipt?.Count}",
                    $"{SelectedReceipt?.Material?.Unit?.Name}",
                    $"{SelectedReceipt?.Material?.Price}",
                    $"{SelectedReceipt?.Summ}"]));
            totalSum += SelectedReceipt != null ? SelectedReceipt.Summ : 0;
        }

        private static void AddTableHeader(ref Table table)
        {
            TableRowGroup headerGroup = new();
            TableRow headerRow = new();

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("ТМЦ"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Количество"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Единица измерения"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("цена"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Сумма"))) { FontWeight = FontWeights.Bold });

            headerGroup.Rows.Add(headerRow);
            table.RowGroups.Add(headerGroup);
        }

        private void AddHeader(ref FlowDocument doc)
        {
            Paragraph title = new(new Run($"Приходная накладная № {SelectedReceipt?.Id}\n" +
                $"От {SelectedReceipt?.Date.ToString("d MMMM yyyy 'г.'", new CultureInfo("ru-RU"))}"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(title);

            string headerText = $"\nПоставщик {SelectedReceipt?.Counterpartie?.Name}" +
                $"\nСклад получатель {SelectedReceipt?.Material?.Location?.Name}, {SelectedReceipt?.Material?.Location?.Employee?.Fio}";

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
