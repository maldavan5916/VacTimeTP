using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace VacTrack.Samples
{
    internal class SampleContract
    {
        public static FlowDocument GenerateContract(
            String ConractNo,
            string contractDate,
            string term,
            string price,
            string address,
            string counterpartieData,
            string OrganizationData)
        {
            // Create the main document
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(50);
            doc.PageWidth = 800;

            // Set document styles
            Style header1Style = new Style(typeof(Paragraph));
            header1Style.Setters.Add(new Setter(Paragraph.FontSizeProperty, 16.0));
            header1Style.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.Bold));
            header1Style.Setters.Add(new Setter(Paragraph.TextAlignmentProperty, TextAlignment.Center));
            header1Style.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0, 20, 0, 10)));
            doc.Resources.Add("header1Style", header1Style);

            Style header2Style = new Style(typeof(Paragraph));
            header2Style.Setters.Add(new Setter(Paragraph.FontSizeProperty, 14.0));
            header2Style.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.Bold));
            header2Style.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0, 15, 0, 5)));
            header2Style.Setters.Add(new Setter(Paragraph.TextAlignmentProperty, TextAlignment.Center));
            doc.Resources.Add("header2Style", header2Style);

            Style normalStyle = new Style(typeof(Paragraph));
            normalStyle.Setters.Add(new Setter(Paragraph.FontSizeProperty, 12.0));
            normalStyle.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0, 5, 0, 5)));
            normalStyle.Setters.Add(new Setter(Paragraph.TextAlignmentProperty, TextAlignment.Justify));
            doc.Resources.Add("normalStyle", normalStyle);

            Style boldStyle = new Style(typeof(Paragraph));
            boldStyle.Setters.Add(new Setter(Paragraph.FontSizeProperty, 12.0));
            boldStyle.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.Bold));
            boldStyle.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0, 5, 0, 5)));
            doc.Resources.Add("boldStyle", boldStyle);

            // Title
            Paragraph title = new Paragraph(new Run($"ДОГОВОР ПОСТАВКИ №{ConractNo}"));
            title.Style = (Style)doc.Resources["header1Style"];
            doc.Blocks.Add(title);

            // Header with city and date
            Table headerTable = new Table();
            headerTable.CellSpacing = 0;
            headerTable.Margin = new Thickness(0, 0, 0, 20);

            // Create columns for city and date
            headerTable.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });
            headerTable.Columns.Add(new TableColumn() { Width = new GridLength(1, GridUnitType.Star) });

            TableRowGroup group = new TableRowGroup();
            headerTable.RowGroups.Add(group);

            // First row with city and date
            TableRow row1 = new TableRow();
            group.Rows.Add(row1);

            // City cell (left aligned)
            TableCell cityCell = new TableCell(new Paragraph(new Run("г. Сморгонь")));
            cityCell.BorderThickness = new Thickness(0, 0, 0, 1);
            cityCell.BorderBrush = Brushes.Black;
            cityCell.Padding = new Thickness(5);
            cityCell.TextAlignment = TextAlignment.Left;  // Выравнивание по левому краю
            row1.Cells.Add(cityCell);

            // Date cell (right aligned)
            TableCell dateCell = new TableCell(new Paragraph(new Run(contractDate)));
            dateCell.BorderThickness = new Thickness(0, 0, 0, 1);
            dateCell.BorderBrush = Brushes.Black;
            dateCell.Padding = new Thickness(5);
            dateCell.TextAlignment = TextAlignment.Right;  // Выравнивание по правому краю
            row1.Cells.Add(dateCell);

            doc.Blocks.Add(headerTable);

            // 1. ПРЕДМЕТ ДОГОВОРА
            Paragraph section1Header = new Paragraph(new Run("1. ПРЕДМЕТ ДОГОВОРА"));
            section1Header.Style = (Style)doc.Resources["header2Style"];
            doc.Blocks.Add(section1Header);

            // 1.1
            Paragraph p11 = new Paragraph();
            p11.Style = (Style)doc.Resources["normalStyle"];
            p11.Inlines.Add(new Run("1.1. Поставщик обязуется поставить Комплектующие к вакуумной установке "));
            p11.Inlines.Add(new Run("(далее - Товар), а Покупатель - принять и оплатить товар, в соответствии "));
            p11.Inlines.Add(new Run("со Счёт-протоколом согласования договорной цены (Приложение №1), "));
            p11.Inlines.Add(new Run("являющегося неотъемлемой частью настоящего договора."));
            doc.Blocks.Add(p11);

            // 1.2
            Paragraph p12 = new Paragraph(new Run("1.2. Покупатель обязуется оплатить и принять Товар."));
            p12.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p12);

            // 1.3
            Paragraph p13 = new Paragraph();
            p13.Style = (Style)doc.Resources["normalStyle"];
            p13.Inlines.Add(new Run("1.3. Срок поставки -- "));
            p13.Inlines.Add(new Bold(new Run(term)));
            p13.Inlines.Add(new Run("."));
            doc.Blocks.Add(p13);

            // 1.4
            Paragraph p14 = new Paragraph(new Run("1.4. Цель приобретения Товара - для собственного потребления и производства."));
            p14.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p14);

            // 2. СТОИМОСТЬ ТОВАРА И ПОРЯДОК РАСЧЕТОВ
            Paragraph section2Header = new Paragraph(new Run("2. СТОИМОСТЬ ТОВАРА И ПОРЯДОК РАСЧЕТОВ"));
            section2Header.Style = (Style)doc.Resources["header2Style"];
            doc.Blocks.Add(section2Header);

            // 2.1
            Paragraph p21 = new Paragraph();
            p21.Style = (Style)doc.Resources["normalStyle"];
            p21.Inlines.Add(new Run("2.1. Стоимость Товара по данному договору, в соответствии со "));
            p21.Inlines.Add(new Run("Счет-протоколом (Приложение №1) составляет "));
            p21.Inlines.Add(new Bold(new Run(price)));
            p21.Inlines.Add(new Run("."));
            doc.Blocks.Add(p21);

            // 2.2
            Paragraph p22 = new Paragraph(new Run("2.2. Покупатель перечисляет на расчетный счет сумму в размере -- 100% стоимости товара."));
            p22.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p22);

            // 2.3
            Paragraph p23 = new Paragraph(new Run("2.3 Цена поставляемого товара остается фиксированной на протяжении действия договора. В цену товара включается тара, упаковка и транспортировка."));
            p23.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p23);

            // 2.4
            Paragraph p24 = new Paragraph(new Run("2.4. Источник финансирования - собственные средства Покупателя."));
            p24.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p24);

            // 2.5
            Paragraph p25 = new Paragraph(new Run("2.5. Датой оплаты считается дата списания денежных средств Покупателя."));
            p25.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p25);

            // 2.6
            Paragraph p26 = new Paragraph(new Run("2.6. Любые изменения по условиям оплаты согласовываются обеими Сторонами и оформляются дополнительным соглашением."));
            p26.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p26);

            // 3. ПОРЯДОК СДАЧИ И ПРИЕМКИ ТОВАРА
            Paragraph section3Header = new Paragraph(new Run("3. ПОРЯДОК СДАЧИ И ПРИЕМКИ ТОВАРА"));
            section3Header.Style = (Style)doc.Resources["header2Style"];
            doc.Blocks.Add(section3Header);

            // 3.1
            Paragraph p31 = new Paragraph(new Run("3.1. Датой поставки Товара считается дата подписи Покупателем товарно-транспортной накладной о принятии Товара."));
            p31.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p31);

            // 3.2
            Paragraph p32 = new Paragraph(new Run("3.2. От даты поставки Товара Покупателю на него переходит право собственности на Товар, риск его случайной гибели или случайного повреждения."));
            p32.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p32);

            // 3.3
            Paragraph p33 = new Paragraph();
            p33.Style = (Style)doc.Resources["normalStyle"];
            p33.Inlines.Add(new Run("3.3. Адрес поставки: "));
            p33.Inlines.Add(new Bold(new Run(address)));
            p33.Inlines.Add(new Run(" (склад Покупателя)."));
            doc.Blocks.Add(p33);

            // 3.4
            Paragraph p34 = new Paragraph(new Run("3.4. Допускается досрочная поставка Товара."));
            p34.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p34);

            // 3.5
            Paragraph p35 = new Paragraph(new Run("3.5. Приемка товара по количеству и качеству, проверка работоспособности Товара осуществляются в присутствии представителей Поставщика и Покупателя и подписании технического акта приемки товара."));
            p35.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p35);

            // 3.6
            Paragraph p36 = new Paragraph(new Run("3.6. Приемка Товара по количеству осуществляется на соответствие количества фактически полученного Товара данным, указанным в товарно-транспортной накладной. В случае обнаружения в ходе приемки недостачи Товара Стороны обязаны: приостановить приемку; принять меры по обеспечению сохранности Товара; оформить факт выявленной недостачи актом, подписанным Сторонами."));
            p36.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p36);

            // 4. КАЧЕСТВО ТОВАРА И ГАРАНТИЯ
            Paragraph section4Header = new Paragraph(new Run("4. КАЧЕСТВО ТОВАРА И ГАРАНТИЯ"));
            section4Header.Style = (Style)doc.Resources["header2Style"];
            doc.Blocks.Add(section4Header);

            // 4.1
            Paragraph p41 = new Paragraph(new Run("4.1. Качество Товара должно соответствовать техническим требованиям Покупателя."));
            p41.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p41);

            // 4.2
            Paragraph p42 = new Paragraph(new Run("4.2. Упаковка товара должна соответствовать требованиям стандартов, обеспечивающих полную сохранность товара при транспортировке всеми видами транспорта."));
            p42.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p42);

            // 4.3
            Paragraph p43 = new Paragraph(new Run("4.3. Поставщик гарантирует качества товара в течение 12 месяцев с даты приемки товара по товарно-транспортной (товарной) накладной. Срок замены или ремонта товара по гарантийным обязательствам составляет 90 дней с момента передачи неисправного товара по дефектному акту Поставщику, если иное не оговорено сторонами."));
            p43.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p43);

            // 4.4
            Paragraph p44 = new Paragraph(new Run("4.4. На время ремонта поставщик продлевает гарантию на товар."));
            p44.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p44);

            // 4.5
            Paragraph p45 = new Paragraph(new Run("4.5. Поставщик не несет гарантийные обязательства в случае, если условия эксплуатации товара не соответствуют требованиям эксплуатационной документации на товар, если имеются механические повреждения товара вследствие неправильного его хранения или эксплуатации."));
            p45.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p45);

            // 5. ОТВЕТСТВЕННОСТЬ СТОРОН
            Paragraph section5Header = new Paragraph(new Run("5. ОТВЕТСТВЕННОСТЬ СТОРОН"));
            section5Header.Style = (Style)doc.Resources["header2Style"];
            doc.Blocks.Add(section5Header);

            // 5.1
            Paragraph p51 = new Paragraph(new Run("5.1. За неисполнение или ненадлежащее исполнение условий договора Стороны несут ответственность, предусмотренную действующим законодательством Республики Беларусь."));
            p51.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p51);

            // 5.2
            Paragraph p52 = new Paragraph(new Run("5.2. В случае просрочки поставки товара Поставщик уплачивает в пользу Покупателя пеню в размере 0,1% от стоимости не поставленного в срок товара, предусмотренного настоящим договором, за каждый день просрочки выполнения обязательства, но не более 10°/о от стоимости не поставленного товара в срок."));
            p52.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p52);

            // 5.3
            Paragraph p53 = new Paragraph(new Run("5.3. В случае нарушения сроков окончательного расчета за поставленный товар Покупатель уплачивает в пользу Поставщика пеню в размере 0,1% от суммы просроченного платежа, за каждый день просрочки выполнения обязательства, но не более 10% от суммы просроченного платежа."));
            p53.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p53);

            // 5.4
            Paragraph p54 = new Paragraph(new Run("5.4. Уплата неустойки (штрафы, пени) не освобождает стороны от выполнения обязательств по настоящему договору."));
            p54.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p54);

            // 5.5
            Paragraph p55 = new Paragraph(new Run("5.5. Споры, не урегулированные Сторонами в претензионном порядке, передаются на разрешение Экономического суда по месту нахождения ответчика."));
            p55.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p55);

            // 5.6
            Paragraph p56 = new Paragraph(new Run("5.6. Во всем остальном, что не предусмотрено настоящим договором, Стороны руководствуются действующим законодательством Республики Беларусь."));
            p56.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p56);

            // 6. СРОК ДЕЙСТВИЯ ДОГОВОРА
            Paragraph section6Header = new Paragraph(new Run("6. СРОК ДЕЙСТВИЯ ДОГОВОРА"));
            section6Header.Style = (Style)doc.Resources["header2Style"];
            doc.Blocks.Add(section6Header);

            // 6.1
            Paragraph p61 = new Paragraph(new Run("6.1. Настоящий договор вступает в силу с момента его подписания Сторонами и действует до полного выполнения Сторонами всех обязательств по настоящему договору."));
            p61.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p61);

            // 6.2
            Paragraph p62 = new Paragraph(new Run("6.2. Документы, переданные по факсимильной связи, имеют юридическую силу."));
            p62.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p62);

            // 6.3
            Paragraph p63 = new Paragraph(new Run("6.3. Все изменения и дополнения к настоящему договору имеют юридическую силу только в том случае, если они совершены в письменной форме и подписаны уполномоченными на то представителями Сторон."));
            p63.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p63);

            // 7. ФОРС-МАЖОР
            Paragraph section7Header = new Paragraph(new Run("7. ФОРС-МАЖОР"));
            section7Header.Style = (Style)doc.Resources["header2Style"];
            doc.Blocks.Add(section7Header);

            // 7.1
            Paragraph p71 = new Paragraph(new Run("7.1. Стороны освобождаются от ответственности по выполнению обязательств по настоящему договору, если оно явилось следствием непреодолимой силы, а именно: пожар, наводнение, землетрясение, война, боевые действия, блокада, эмбарго на импорт и экспорт. При этом срок выполнения обязательств по настоящему договору отодвигается соразмерно времени, в течение которого действовали эти обстоятельства и их последствия."));
            p71.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p71);

            // 7.2
            Paragraph p72 = new Paragraph(new Run("7.2. Сторона, для которой создалась невозможность выполнения обязательств по договору, обязана немедленно известить другую Сторону о наступлении и прекращении вышеуказанных обстоятельств."));
            p72.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p72);

            // 7.3
            Paragraph p73 = new Paragraph(new Run("7.3. Надлежащим доказательством наличия указанных выше обстоятельств и их продолжительности будет служить свидетельство соответствующих торговых палат."));
            p73.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p73);

            // 7.4
            Paragraph p74 = new Paragraph(new Run("7.4. Если эти обстоятельства и их последствия будут длиться более 4-х месяцев, то каждая из Сторон будет вправе аннулировать договор полностью или частично, и в этом случае ни одна из Сторон не будет иметь права потребовать от другой Стороны возмещения возможных убытков."));
            p74.Style = (Style)doc.Resources["normalStyle"];
            doc.Blocks.Add(p74);

            // 8. ЮРИДИЧЕСКИЕ АДРЕСА СТОРОН
            Paragraph section8Header = new Paragraph(new Run("8. ЮРИДИЧЕСКИЕ АДРЕСА СТОРОН"));
            section8Header.Style = (Style)doc.Resources["header2Style"];
            doc.Blocks.Add(section8Header);

            // Create table for parties information without borders
            Table partiesTable = new Table();
            partiesTable.CellSpacing = 0;
            partiesTable.Margin = new Thickness(0, 10, 0, 20);
            // Убраны все свойства, связанные с границами
            partiesTable.BorderBrush = Brushes.Transparent;
            partiesTable.BorderThickness = new Thickness(0);

            TableRowGroup partiesGroup = new TableRowGroup();
            partiesTable.RowGroups.Add(partiesGroup);

            // Header row
            TableRow headerRow = new TableRow();
            partiesGroup.Rows.Add(headerRow);

            TableCell buyerHeaderCell = new TableCell(new Paragraph(new Run("Покупатель")));
            buyerHeaderCell.BorderBrush = Brushes.Transparent; // Убрана граница
            buyerHeaderCell.BorderThickness = new Thickness(0); // Убрана граница
            buyerHeaderCell.Padding = new Thickness(5);
            buyerHeaderCell.TextAlignment = TextAlignment.Center;
            headerRow.Cells.Add(buyerHeaderCell);

            TableCell supplierHeaderCell = new TableCell(new Paragraph(new Run("Поставщик")));
            supplierHeaderCell.BorderBrush = Brushes.Transparent; // Убрана граница
            supplierHeaderCell.BorderThickness = new Thickness(0); // Убрана граница
            supplierHeaderCell.Padding = new Thickness(5);
            supplierHeaderCell.TextAlignment = TextAlignment.Center;
            headerRow.Cells.Add(supplierHeaderCell);

            // Content row
            TableRow contentRow = new TableRow();
            partiesGroup.Rows.Add(contentRow);

            // Buyer cell
            TableCell buyerCell = new TableCell();
            buyerCell.BorderBrush = Brushes.Transparent; // Убрана граница
            buyerCell.BorderThickness = new Thickness(0); // Убрана граница
            buyerCell.Padding = new Thickness(5);

            Paragraph buyerPara = new Paragraph(new Run(counterpartieData));
            buyerPara.Style = (Style)doc.Resources["normalStyle"];
            buyerCell.Blocks.Add(buyerPara);
            contentRow.Cells.Add(buyerCell);

            // Supplier cell
            TableCell supplierCell = new TableCell();
            supplierCell.BorderBrush = Brushes.Transparent; // Убрана граница
            supplierCell.BorderThickness = new Thickness(0); // Убрана граница
            supplierCell.Padding = new Thickness(5);

            Paragraph supplierPara = new Paragraph(new Run(OrganizationData));
            supplierPara.Style = (Style)doc.Resources["normalStyle"];
            supplierCell.Blocks.Add(supplierPara);;
            contentRow.Cells.Add(supplierCell);

            doc.Blocks.Add(partiesTable);

            return doc;
        }
    }
}
