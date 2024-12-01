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
    /// Логика взаимодействия для EmployeeDivisionReport.xaml
    /// </summary>
    public partial class EmployeeDivisionReport : Page, ICachedPage
    {
        private EmployeeDivisionReportViewModel ThisViewModel => (EmployeeDivisionReportViewModel)DataContext;
        public EmployeeDivisionReport() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public enum GroupedType { NoGrouped, GroupedByDivision, GroupedByPost }

    public class EmployeeDivisionReportViewModel : BaseReportViewModel<Employee>
    {
        public ObservableCollection<Division>? EmployDivisions { get; set; }
        public ObservableCollection<Post>? EmployPosts { get; set; }

        #region properties
        private GroupedType GroupedType;
        public bool IsGroupedTypeNoGrouped 
        { 
            get => GroupedType == GroupedType.NoGrouped;
            set
            {
                if (value)
                {
                    GroupedType = GroupedType.NoGrouped;
                    Refresh(null);
                    OnPropertyChanged(nameof(IsGroupedTypeNoGrouped));
                }
            }
        }

        public bool IsGroupedTypeGroupedByDivision
        {
            get => GroupedType == GroupedType.GroupedByDivision;
            set
            {
                if (value)
                {
                    GroupedType = GroupedType.GroupedByDivision;
                    Refresh(null);
                    OnPropertyChanged(nameof(IsGroupedTypeGroupedByDivision));
                }
            }
        }

        public bool IsGroupedTypeGroupedByPost
        {
            get => GroupedType == GroupedType.GroupedByPost;
            set
            {
                if (value)
                {
                    GroupedType = GroupedType.GroupedByPost;
                    Refresh(null);
                    OnPropertyChanged(nameof(IsGroupedTypeGroupedByPost));
                }
            }
        }

        private Post? _FilterByPost;
        public Post? FilterByPost
        {
            get => _FilterByPost;
            set
            {
                SetProperty(ref _FilterByPost, value);
                LoadData();
            }
        }

        private Division? _FilterByDivision;
        public Division? FilterByDivision
        {
            get => _FilterByDivision;
            set
            {
                SetProperty(ref _FilterByDivision, value);
                LoadData();
            }
        }
        #endregion

        protected override void LoadData()
        {
            EmployDivisions = new ObservableCollection<Division>([.. Db.Divisions]);
            EmployPosts = new ObservableCollection<Post>([.. Db.Posts]);

            DbSet = Db.Set<Employee>();
            DbSet.Include(e => e.Division).Include(e => e.Post).Load();

            Items = new ObservableCollection<Employee>(
                DbSet.Local.Where(item =>
                (FilterByPost == null || item.Post?.Id == FilterByPost.Id) &&
                (FilterByDivision == null || item.Division?.Id == FilterByDivision.Id)
                ).ToList());
        }

        public override FlowDocument CreateReport()
        {
            FlowDocument doc = new() { FontFamily = new FontFamily("Times New Roman"), FontSize = 12, PagePadding = new Thickness(50) };
            AddHeader(ref doc); // Добавляем заголовок Отчёта

            Table table = new() { CellSpacing = 3, BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
            AddTableHeader(ref table);  // Добавляем заголовок таблицы

            TableRowGroup dataGroup = new();

            switch (GroupedType)
            {
                case GroupedType.GroupedByDivision: CreateGroupedByDivisionRows(ref dataGroup); break;
                case GroupedType.GroupedByPost: CreateGroupedByPostRows(ref dataGroup); break;
                case GroupedType.NoGrouped: CreateNoGroupedRows(ref dataGroup); break;
            }

            table.RowGroups.Add(dataGroup);
            doc.Blocks.Add(table);
            return doc;
        }

        private void CreateGroupedByDivisionRows(ref TableRowGroup dataGroup)
        {
            var groupedItems = Items.GroupBy(item => item.Division?.Name);

            foreach (var group in groupedItems)
            {
                dataGroup.Rows.Add(CreateRow(("", $"{group.Key}", "", "", "")));

                foreach (var item in group)
                {
                    TableRow row = new();

                    dataGroup.Rows.Add(CreateRow((
                        $"{item.Fio}",
                        "", // Пустая ячейка, т.к. уже указано в заголовке группы
                        $"{item.Post?.Name}",
                        $"{item.DateHire:dd.MM.yyyy}",
                        $"{item.DateDismissal:dd.MM.yyyy}")));

                    dataGroup.Rows.Add(row);
                }
            }
        }

        private void CreateGroupedByPostRows(ref TableRowGroup dataGroup)
        {
            var groupedItems = Items.GroupBy(item => item.Post?.Name);

            foreach (var group in groupedItems)
            {
                dataGroup.Rows.Add(CreateRow(("", "", $"{group.Key}", "", "")));

                foreach (var item in group)
                {
                    TableRow row = new();

                    dataGroup.Rows.Add(CreateRow((
                        $"{item.Fio}",
                        $"{item.Division?.Name}",
                        "", // Пустая ячейка, т.к. уже указано в заголовке группы
                        $"{item.DateHire:dd.MM.yyyy}",
                        $"{item.DateDismissal:dd.MM.yyyy}")));

                    dataGroup.Rows.Add(row);
                }
            }
        }

        private void CreateNoGroupedRows(ref TableRowGroup dataGroup)
        {
            foreach (var item in Items)
            {
                dataGroup.Rows.Add(CreateRow((
                    $"{item.Fio}",
                    $"{item.Division?.Name}",
                    $"{item.Post?.Name}",
                    $"{item.DateHire:dd.MM.yyyy}",
                    $"{item.DateDismissal:dd.MM.yyyy}")));
            }
        }

        private static void AddTableHeader(ref Table table)
        {
            TableRowGroup headerGroup = new();
            TableRow headerRow = new();

            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("ФИО"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Подразделение"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Должность"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Дата приема"))) { FontWeight = FontWeights.Bold });
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Дата увольнения"))) { FontWeight = FontWeights.Bold });

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

            Paragraph title = new(new Run("Отчет по сотрудникам и их подразделениям"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(title);

            
            string filterByPost = FilterByPost == null ? "" : $"\nПо должности: {FilterByPost.Name}";
            string filterByDivision = FilterByDivision== null ? "" : $"\nПо подразделению: {FilterByDivision.Name}";
            string filteredText = filterByPost + filterByDivision;

            string headerText = $"Дата формирования: {DateTime.Now:dd.MM.yyyy}" + filteredText;

            Paragraph headerParagraph = new(new Run(headerText))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                LineHeight = 1.5 // Можно добавить межстрочный интервал
            };
            doc.Blocks.Add(headerParagraph);
        }

        static private TableRow CreateRow((string Col1, string Col2, string Col3, string Col4, string Col5) data)
        {
            TableRow row = new();
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col1))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col2))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col3))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col4))));
            row.Cells.Add(new TableCell(new Paragraph(new Run(data.Col5))));
            return row;
        }
    }
}
