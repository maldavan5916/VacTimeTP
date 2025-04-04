using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
        private GroupedType GroupedType = GroupedType.NoGrouped;

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

        public ICommand ClearFilterCommand { get; }

        public EmployeeDivisionReportViewModel()
        {
            ClearFilterCommand = new RelayCommand(ClearFilter);
        }

        protected override void LoadData()
        {
            EmployDivisions = new ObservableCollection<Division>([.. Db.Divisions]);
            EmployPosts = new ObservableCollection<Post>([.. Db.Posts]);

            DbSet = Db.Set<Employee>();
            DbSet.Include(e => e.Division).Include(e => e.Post).Load();

            Items = new ObservableCollection<Employee>(
                DbSet.Local.Where(item =>
                // Фильтрация по должности
                (FilterByPost == null || item.Post?.Id == FilterByPost.Id) &&

                // Фильтрация по подразделению
                (FilterByDivision == null || item.Division?.Id == FilterByDivision.Id) &&

                // Проверка на наличие фильтров по датам
                ((FilterStartDate == null && FilterEndDate == null) ||

                // Если заданы оба фильтра (FilterStartDate и FilterEndDate)
                (FilterStartDate != null && FilterEndDate != null &&
                    // Проверяем, что DateHire или DateDismissal попадают в указанный диапазон
                    ((item.DateHire >= FilterStartDate && item.DateHire <= FilterEndDate) ||
                     (item.DateDismissal >= FilterStartDate && item.DateDismissal <= FilterEndDate))) ||

                // Если задан только FilterStartDate, фильтруем по точному совпадению с этой датой
                (FilterStartDate != null && FilterEndDate == null &&
                    (item.DateHire == FilterStartDate || item.DateDismissal == FilterStartDate)) ||

                // Если задан только FilterEndDate, фильтруем по точному совпадению с этой датой
                (FilterStartDate == null && FilterEndDate != null &&
                    (item.DateHire == FilterEndDate || item.DateDismissal == FilterEndDate)))
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

            Employee? responsible = Db.Employees
                .Include(e => e.Post)
                .FirstOrDefault(e => e.Id == Properties.Settings.Default.ResponsibleAccountant);

            doc.Blocks.Add(new Paragraph(
                new Run($"{responsible?.Post?.Name}   _____________   {responsible?.Fio}"))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Left,
                LineHeight = 1.5 // Можно добавить межстрочный интервал
            });

            return doc;
        }

        private void CreateGroupedByDivisionRows(ref TableRowGroup dataGroup)
        {
            CreateGroupedRows(
                ref dataGroup,
                item => item.Division?.Name,
                key => ["", $"{key}", "", "", ""], // Заголовок группы
                item => [
                    $"{item.Fio}",
                    string.Empty,
                    $"{item.Post?.Name}",
                    $"{item.DateHire:dd.MM.yyyy}",
                    $"{item.DateDismissal:dd.MM.yyyy}"]
            );
        }

        private void CreateGroupedByPostRows(ref TableRowGroup dataGroup)
        {
            CreateGroupedRows(
                ref dataGroup,
                item => item.Post?.Name,
                key => ["", "", $"{key}", "", ""], // Заголовок группы
                item => [
                    $"{item.Fio}",
                    $"{item.Division?.Name}",
                    string.Empty,
                    $"{item.DateHire:dd.MM.yyyy}",
                    $"{item.DateDismissal:dd.MM.yyyy}"]
            );
        }

        private void CreateNoGroupedRows(ref TableRowGroup dataGroup)
        {
            foreach (var item in Items)
            {
                dataGroup.Rows.Add(CreateRow([
                    $"{item.Fio}",
                    $"{item.Division?.Name}",
                    $"{item.Post?.Name}",
                    $"{item.DateHire:dd.MM.yyyy}",
                    $"{item.DateDismissal:dd.MM.yyyy}"]));
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


            string filterByPost = FilterByPost == null ? string.Empty : $"\nПо должности: {FilterByPost.Name}";
            string filterByDivision = FilterByDivision == null ? string.Empty : $"\nПо подразделению: {FilterByDivision.Name}";

            //string periodFilter =
            //    FilterStartDate != null && FilterEndDate != null ? $"\nЗа период с {FilterStartDate:dd.MM.yyyy} по {FilterEndDate:dd.MM.yyyy}" :
            //    FilterStartDate != null && FilterEndDate == null ? $"\nНа день {FilterStartDate:dd.MM.yyyy}" :
            //    FilterStartDate == null && FilterEndDate != null ? $"\nНа день {FilterEndDate:dd.MM.yyyy}" : string.Empty;

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
            FilterByDivision = null;
            FilterByPost = null;
            FilterStartDate = null;
            FilterEndDate = null;
        }
    }
}
