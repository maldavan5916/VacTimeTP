using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        private void CheckBox_Click(object sender, RoutedEventArgs e) => ThisViewModel.OpenFromCache();
    }

    public enum GroupedType { NoGrouped, GroupedByDivision, GroupedByPost }

    public class ColumnSetting : INotifyPropertyChanged
    {
        private bool _isVisible;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public required string Name { get; set; }
        public required string Code { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

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

        public ObservableCollection<ColumnSetting> Columns { get; set; } =
        [
            new ColumnSetting { Name = "ФИО",             Code = "fio",            IsVisible = true },
            new ColumnSetting { Name = "Подразделение",   Code = "division",       IsVisible = true },
            new ColumnSetting { Name = "Должность",       Code = "post",           IsVisible = true },
            new ColumnSetting { Name = "Дата приема",     Code = "dateHire",       IsVisible = true },
            new ColumnSetting { Name = "Дата рождения",   Code = "dateOfBirth",    IsVisible = true },
            new ColumnSetting { Name = "Телефон",         Code = "phoneNumber",    IsVisible = true },
            new ColumnSetting { Name = "Адресс",          Code = "address",        IsVisible = true },
            new ColumnSetting { Name = "Оклад,\n" + Properties.Settings.Default.Currency, Code = "salary", IsVisible = true },
            new ColumnSetting { Name = "Дата увольнения", Code = "dateDismissal",  IsVisible = false },
        ];
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

            Items = [.. DbSet.Local.Where(item =>
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
                ).ToList()];
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

        private List<string> GetVisibleValues(Employee item, string codeEmptyCol = "")
        {
            var result = new List<string>();

            foreach (var column in Columns)
            {
                if (!column.IsVisible) continue;
                
                if (column.Code == codeEmptyCol)
                {
                    result.Add(string.Empty);
                    continue;
                }

                switch (column.Code)
                {
                    case "fio":           result.Add($"{item.Fio}");                      break;
                    case "division":      result.Add($"{item.Division?.Name}");           break;
                    case "post":          result.Add($"{item.Post?.Name}");               break;
                    case "dateHire":      result.Add($"{item.DateHire:dd.MM.yyyy}");      break;
                    case "dateOfBirth":   result.Add($"{item.DateOfBirth:dd.MM.yyyy}");   break;
                    case "phoneNumber":   result.Add($"{item.PhoneNumber}");              break;
                    case "address":       result.Add($"{item.Address}");                  break;
                    case "salary":        result.Add($"{item.Salary:N2}");                break;
                    case "dateDismissal": result.Add($"{item.DateDismissal:dd.MM.yyyy}"); break;
                    default: result.Add(string.Empty); break;
                }
            }

            return result;
        }

        private void CreateGroupedByDivisionRows(ref TableRowGroup dataGroup)
        {
            // Группируем по подразделению
            var groupedItems = Items.GroupBy(item => item.Division?.Name);

            foreach (var group in groupedItems)
            {
                // Добавляем строку заголовка для группы
                var headerValues = Columns
                    .Where(c => c.IsVisible)
                    .Select(c => c.Code == "division" ? group.Key : "") // Заголовок по подразделению
                    .ToList();


#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                dataGroup.Rows.Add(CreateRow(headerValues));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

                // Создаём строки для каждого сотрудника в группе
                foreach (var item in group)
                {
                    var visibleValues = GetVisibleValues(item, "division");
                    dataGroup.Rows.Add(CreateRow(visibleValues));
                }
            }
        }

        private void CreateGroupedByPostRows(ref TableRowGroup dataGroup)
        {
            // Группируем по подразделению
            var groupedItems = Items.GroupBy(item => item.Post?.Name);

            foreach (var group in groupedItems)
            {
                // Добавляем строку заголовка для группы
                var headerValues = Columns
                    .Where(c => c.IsVisible)
                    .Select(c => c.Code == "post" ? group.Key : "") // Заголовок по подразделению
                    .ToList();

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                dataGroup.Rows.Add(CreateRow(headerValues));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

                // Создаём строки для каждого сотрудника в группе
                foreach (var item in group)
                {
                    var visibleValues = GetVisibleValues(item, "post");
                    dataGroup.Rows.Add(CreateRow(visibleValues));
                }
            }
        }

        private void CreateNoGroupedRows(ref TableRowGroup dataGroup)
        {
            foreach (var item in Items)
                dataGroup.Rows.Add(CreateRow(GetVisibleValues(item)));
        }

        private void AddTableHeader(ref Table table)
        {
            TableRowGroup headerGroup = new();
            TableRow headerRow = new();

            foreach (var col in Columns) 
                if (col.IsVisible)
                    headerRow.Cells.Add(new TableCell(new Paragraph(new Run(col.Name))) { FontWeight = FontWeights.Bold });

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

            Paragraph title = new(new Run("Отчет по сотрудникам"))
            {
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(title);


            string filterByPost = FilterByPost == null ? string.Empty : $"\nПо должности: {FilterByPost.Name}";
            string filterByDivision = FilterByDivision == null ? string.Empty : $"\nПо подразделению: {FilterByDivision.Name}";

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
