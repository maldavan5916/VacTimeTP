using System.Globalization; // Для CultureInfo
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using DatabaseManager;

namespace VacTrack.ViewReport
{
    /// <summary>
    /// Interaction logic for ContractReport.xaml
    /// </summary>
    public partial class ContractReport : Window
    {
        private ContarcReportViewModel ThisViewModel => (ContarcReportViewModel)DataContext;
        public ContractReport(Contract contract)
        {
            InitializeComponent();
            ThisViewModel.SelectContract = contract;
        }
    }

    public class ContarcReportViewModel : BaseReportViewModel<Contract>
    {
        private Contract? _selectContract;
        public Contract? SelectContract
        {
            get => _selectContract;
            set
            {
                SetProperty(ref _selectContract, value);
                Refresh(null);
            }
        }

        public override FlowDocument CreateReport()
        {
            FlowDocument doc = Samples.SampleContract.GenerateContract(
                SelectContract?.Id != null ? SelectContract.Id : 0,
                $"{SelectContract?.Date.ToString("'«'d'»' MMMM yyyy 'г.'", new CultureInfo("ru-RU"))}",
                $"{SelectContract?.Term} месяцев",
                $"{SelectContract?.Summ}",
                $"{SelectContract?.Counterpartie?.PostalAddress}",

                $"{SelectContract?.Counterpartie?.Name}\n" +
                $"{SelectContract?.Counterpartie?.LegalAddress}\n" +
                $"БИК: {SelectContract?.Counterpartie?.BankAccount}\n" +
                $"УНП: {SelectContract?.Counterpartie?.Unp}\n" +
                $"Тел: {SelectContract?.Counterpartie?.PhoneNomber}\n",

                $"ООО «Вактайм»\n" +
                $"231000 г. Сморгонь, Гродненская обл., ул. Пушкина 91\n" +
                $"p/c BY20AKBB30120877400144200000\n" +
                $"БИК АКВВВУ2Х в ЦБУ 423 ОЛО АСБ «Беларусбанк» г. Сморгонь ул. Советская 36\n" +
                $"УНН: 590976294\n" +
                $"ΟΚΠΟ: 300438284000\n" +
                $"Тел. GSM: +375 29-6299854;\n" +
                $"Факс: +375(1592)-4-55-23"
                );

            Section newSection = new()
            {
                BreakPageBefore = true
            };
            AddAttachment( newSection );
            doc.Blocks.Add(newSection);

            return doc;
        }

        private void AddAttachment(Section section)
        {
            section.FontFamily = new FontFamily("Times New Roman");
            section.FontSize = 14;

            // Заголовок
            Paragraph header = new Paragraph
            {
                TextAlignment = TextAlignment.Right,
                FontWeight = FontWeights.Bold
            };
            header.Inlines.Add(new Run("Приложение 1"));
            header.Inlines.Add(new LineBreak());
            header.Inlines.Add(new Run($"к договору № {SelectContract?.Id} от {SelectContract?.Date.ToString("'«'d'»' MMMM yyyy 'г.'", new CultureInfo("ru-RU"))} г."));
            section.Blocks.Add(header);

            // Название
            section.Blocks.Add(new Paragraph(new Run("Счет-протокол согласования договорной цены"))
            {
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10)
            });

            // Таблица
            Table table = new Table
            {
                CellSpacing = 0,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            double[] columnWidths = { 30, 200, 60, 100, 100, 80, 100, 100 };
            foreach (double width in columnWidths)
            {
                table.Columns.Add(new TableColumn { });
            }

            TableRowGroup trg = new TableRowGroup();
            table.RowGroups.Add(trg);

            // Заголовок таблицы
            trg.Rows.Add(new TableRow());
            string[] headers = [
                "Наименование", 
                "Кол-во", 
                "Цена за ед.",
                "Сумма", 
                "ставка НДС", 
                "Сумма НДС", 
                "Всего с НДС"
            ];
            foreach (string h in headers)
            {
                trg.Rows[0].Cells.Add(new TableCell(new Paragraph(new Run(h))));
            }

            var nds = Properties.Settings.Default.Nds;
            // Строка с товаром
            trg.Rows.Add(new TableRow());
            string[] row1 = [
                $"{SelectContract?.Product?.Name}", 
                $"{SelectContract?.Count} {SelectContract?.Product?.Unit?.Name}", 
                $"{SelectContract?.Product?.Price}",
                $"{SelectContract?.Summ}", 
                $"{nds}%", 
                $"{SelectContract?.Summ * (nds / 100)}", 
                $"{SelectContract?.Summ * (1 + nds / 100)}"
            ];
            foreach (string cell in row1)
            {
                trg.Rows[1].Cells.Add(new TableCell(new Paragraph(new Run(cell))));
            }

            // Итого
            trg.Rows.Add(new TableRow());
            TableRow totalRow = trg.Rows[2];
            totalRow.Cells.Add(new TableCell(new Paragraph(new Run("ИТОГО"))) { ColumnSpan = 3 });
            totalRow.Cells[0].FontWeight = FontWeights.Bold;
            totalRow.Cells.Add(new TableCell(new Paragraph(new Run($"{SelectContract?.Summ}"))) { FontWeight = FontWeights.Bold });
            totalRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
            totalRow.Cells.Add(new TableCell(new Paragraph(new Run($"{SelectContract?.Summ * (nds / 100)}"))));
            totalRow.Cells.Add(new TableCell(new Paragraph(new Run($"{SelectContract?.Summ * (1 + nds / 100)}"))));

            section.Blocks.Add(table);

            // Суммы
            Paragraph totals = new Paragraph();
            totals.Inlines.Add(new Bold(new Run("Сумма НДС: ")));
            totals.Inlines.Add(new Run($"{SelectContract?.Summ * (nds / 100)}"));
            totals.Inlines.Add(new LineBreak());
            totals.Inlines.Add(new Bold(new Run("Всего с НДС: ")));
            totals.Inlines.Add(new Run($"{SelectContract?.Summ * (1 + nds / 100)}"));
            totals.Margin = new Thickness(0, 10, 0, 0);
            section.Blocks.Add(totals);

            // Таблица для "Покупатель" и "Поставщик"
            Table infoTable = new Table
            {
                CellSpacing = 0,
                Margin = new Thickness(20, 20, 0, 0)
            };
            infoTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            infoTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

            TableRowGroup infoGroup = new TableRowGroup();
            infoTable.RowGroups.Add(infoGroup);

            // Строка с двумя ячейками
            TableRow row = new TableRow();
            infoGroup.Rows.Add(row);

            // Ячейка "Покупатель"
            Paragraph buyer = new Paragraph();
            buyer.Inlines.Add(new Bold(new Run("Покупатель")));
            buyer.Inlines.Add(new LineBreak());
            buyer.Inlines.Add(new Run(
                $"{SelectContract?.Counterpartie?.Name}\n" +
                $"{SelectContract?.Counterpartie?.LegalAddress}\n" +
                $"БИК: {SelectContract?.Counterpartie?.BankAccount}\n" +
                $"УНП: {SelectContract?.Counterpartie?.Unp}\n" +
                $"Тел: {SelectContract?.Counterpartie?.PhoneNomber}"
            ));
            row.Cells.Add(new TableCell(buyer) { BorderThickness = new Thickness(0) });

            // Ячейка "Поставщик"
            Paragraph supplier = new Paragraph();
            supplier.Inlines.Add(new Bold(new Run("Поставщик")));
            supplier.Inlines.Add(new LineBreak());
            supplier.Inlines.Add(new Run(
                $"ООО «Вактайм»\n" +
                $"231000 г. Сморгонь, Гродненская обл., ул. Пушкина 91\n" +
                $"p/c BY20AKBB30120877400144200000\n" +
                $"БИК АКВВВУ2Х в ЦБУ 423 ОЛО АСБ «Беларусбанк» г. Сморгонь ул. Советская 36\n" +
                $"УНН: 590976294\n" +
                $"ΟКПΟ: 300438284000\n" +
                $"Тел. GSM: +375 29-6299854;\n" +
                $"Факс: +375(1592)-4-55-23"
            ));
            row.Cells.Add(new TableCell(supplier) { BorderThickness = new Thickness(0) });

            section.Blocks.Add(infoTable);

        }
    }
}
