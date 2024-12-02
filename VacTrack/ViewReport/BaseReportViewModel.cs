using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace VacTrack.ViewReport
{
    public abstract class BaseReportViewModel<T> : INotifyPropertyChanged where T : BaseModel
    {
        #region interface implemented 
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<Ts>(ref Ts field, Ts value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<Ts>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        #region properties
        protected DatabaseContext Db;
        protected DbSet<T> DbSet;
        private System.Timers.Timer? _resetTimer;


        private ObservableCollection<T> _Items;
        public ObservableCollection<T> Items
        {
            get => _Items;
            set
            {
                if (_Items != value)
                {
                    _Items = value;
                    Refresh(null);
                    SetProperty(ref _Items, value);
                }
            }
        }

        private FlowDocument? _Document;
        public FlowDocument? Document
        {
            get => _Document;
            set => SetProperty(ref _Document, value);
        }

        private string? _Message;
        public string? Message
        {
            get => _Message;
            set
            {
                _Message = value;
                OnPropertyChanged();
                StartResetTimer();
            }
        }

        private Brush? _MessageBrush;
        public Brush? MessageBrush
        {
            get => _MessageBrush;
            set
            {
                _MessageBrush = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public ICommand RefreshCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand RotateDocCommand { get; }

        public BaseReportViewModel()
        {
            Db = new DatabaseContext();
            Db.Database.EnsureCreated();
            LoadData();
            if (DbSet == null || _Items == null) throw new Exception("Data loading error");

            RefreshCommand = new RelayCommand(Refresh);
            PrintCommand = new RelayCommand(PrintReport);
            RotateDocCommand = new RelayCommand(SwapPageDimensions);
        }

        public abstract FlowDocument CreateReport();

        protected void Refresh(object? obj)
        {
            Document = CreateReport();

            // Настройка размеров документа
            Document.PageWidth = 793.7; // A4 ширина в пикселях (96 DPI)
            Document.PageHeight = 1122.52; // A4 высота в пикселях (96 DPI)
            Document.PagePadding = new Thickness(50); // Поля страницы
            Document.ColumnWidth = double.PositiveInfinity; // Убрать колонки
        }

        protected virtual void LoadData()
        {
            DbSet = Db.Set<T>();
            DbSet.Load();
            Items = DbSet.Local.ToObservableCollection();
        }

        private void PrintReport(object obj)
        {
            if (Document == null)
            {
                Message = "Документ пуст";
                MessageBrush = Brushes.Orange;
                return;
            }

            PrintDialog printDialog = new();

            if (printDialog.ShowDialog() == true)
            {
                // Установка размера печатной области
                Document.PageWidth = printDialog.PrintableAreaWidth;
                Document.PageHeight = printDialog.PrintableAreaHeight;

                // Печать документа
                printDialog.PrintDocument(((IDocumentPaginatorSource)Document).DocumentPaginator, "Печать отчёта");
            }
        }

        public void OpenFromCache() => LoadData();

        private void StartResetTimer()
        {
            // Останавливаем предыдущий таймер, если он существует
            _resetTimer?.Stop();
            // Создаем новый таймер, который сработает через 10 секунд
            _resetTimer = new System.Timers.Timer(10000); // 10 секунд
            _resetTimer.Elapsed += ResetMessage;
            _resetTimer.Start();
        }

        private void ResetMessage(object? sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Message = string.Empty;
            });
            _resetTimer?.Stop();
        }

        private void SwapPageDimensions(object obj)
        {
            if (Document == null)
            {
                Message = "Документ пуст";
                MessageBrush = Brushes.Orange;
                return;
            }

            // Сохраняем текущие значения
            double tempWidth = Document.PageWidth;
            double tempHeight = Document.PageHeight;

            // Меняем местами ширину и высоту
            Document.PageWidth = tempHeight;
            Document.PageHeight = tempWidth;
        }

        public void CreateGroupedRows<TKey>(
            ref TableRowGroup dataGroup,
            Func<T, TKey> keySelector, // Функция для определения ключа группировки
            Func<TKey, IEnumerable<string>> groupHeaderSelector, // Формат заголовка группы
            Func<T, IEnumerable<string>> rowSelector // Формат строки данных
        )
        {
            var groupedItems = Items.GroupBy(keySelector);

            foreach (var group in groupedItems)
            {
                dataGroup.Rows.Add(CreateRow(groupHeaderSelector(group.Key))); // Добавляем заголовок группы

                foreach (var item in group)
                {
                    dataGroup.Rows.Add(CreateRow(rowSelector(item))); // Добавляем строку данных
                }
            }
        }

        public void CreateGroupedRows<TKey>(
            ref TableRowGroup dataGroup,
            Func<T, TKey> keySelector, // Функция для определения ключа группировки
            Func<T, double> getSum,  // Функция для определения суммы
            Func<TKey, IEnumerable<string>> groupHeaderSelector, // Формат заголовка группы
            Func<double, IEnumerable<string>> groupTotalSelector, // Формат Итога группы
            Func<T, IEnumerable<string>> rowSelector, // Формат строки данных
            ref double totalSum,
            bool IsGroupTotalEnabled
        )
        {
            var groupedItems = Items.GroupBy(keySelector);

            foreach (var group in groupedItems)
            {
                dataGroup.Rows.Add(CreateRow(groupHeaderSelector(group.Key))); // Добавляем заголовок группы
                double summ = 0;

                foreach (var item in group)
                {
                    dataGroup.Rows.Add(CreateRow(rowSelector(item))); // Добавляем строку данных
                    summ += getSum(item);
                }

                if (IsGroupTotalEnabled) dataGroup.Rows.Add(CreateRow(groupTotalSelector(summ)));

                totalSum += summ;
            }
        }

        public void CreateGroupedRows<TKey>(
            ref TableRowGroup dataGroup,
            Func<T, TKey> keySelector, // Функция для определения ключа группировки
            Func<T, double> getSum,  // Функция для определения суммы
            Func<T, double> getCount,
            Func<TKey, IEnumerable<string>> groupHeaderSelector, // Формат заголовка группы
            Func<(double, double), IEnumerable<string>> groupTotalSelector, // Формат Итога группы
            Func<T, IEnumerable<string>> rowSelector, // Формат строки данных
            ref double totalSum,
            bool IsGroupTotalEnabled
        )
        {
            var groupedItems = Items.GroupBy(keySelector);

            foreach (var group in groupedItems)
            {
                dataGroup.Rows.Add(CreateRow(groupHeaderSelector(group.Key))); // Добавляем заголовок группы
                double summ = 0;
                double count = 0;

                foreach (var item in group)
                {
                    dataGroup.Rows.Add(CreateRow(rowSelector(item))); // Добавляем строку данных
                    summ += getSum(item);
                    count += getCount(item);
                }

                if (IsGroupTotalEnabled) dataGroup.Rows.Add(CreateRow(groupTotalSelector((count, summ))));

                totalSum += summ;
            }
        }

        static public TableRow CreateRow(IEnumerable<string> columns)
        {
            TableRow row = new();

            foreach (var column in columns)
                row.Cells.Add(new TableCell(new Paragraph(new Run(column ?? string.Empty))));

            return row;
        }
    }
}
