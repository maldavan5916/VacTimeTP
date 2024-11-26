using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;

namespace VacTrack.ViewTables
{
    public abstract class BaseViewModel<T> : INotifyPropertyChanged where T : BaseModel
    {
        #region interface implemented 
        public event PropertyChangedEventHandler? PropertyChanged;
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
                    OnPropertyChanged(nameof(Items));
                }
            }
        }

        private T? _SelectedItem;
        public T? SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
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

        private string? _SearchText;
        public string? SearchText
        {
            get => _SearchText;
            set
            {
                _SearchText = value;
                OnPropertyChanged();
                Search();
            }
        }

        private string? _TableName;
        public string? TableName
        {
            get => _TableName;
            set
            {
                _TableName = value;
                OnPropertyChanged();
            }
        }

        public bool? IsAllSelected
        {
            get
            {
                var selected = Items.Select(item => item.IsSelected).Distinct().ToList();
                return selected.Count == 1 ? selected.Single() : null;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAll(value.Value, Items);
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public BaseViewModel(DatabaseContext db)
        {
            Db = db;
            Db.Database.EnsureCreated();
            LoadData();

            if (DbSet == null || _Items == null) throw new Exception("Data loading error");

            AddCommand = new RelayCommand(AddItem);
            DeleteCommand = new RelayCommand(DeleteItem);
            SaveCommand = new RelayCommand(SaveChanges);
            CancelCommand = new RelayCommand(CancelChanges);

            foreach (var model in Items)
            {
                model.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(Employee.IsSelected))
                        OnPropertyChanged(nameof(IsAllSelected));
                };
            }
        }

        protected abstract T CreateNewItem();

        protected abstract bool FilterItem(T item, string? searchText);

        protected virtual void LoadData()
        {
            DbSet = Db.Set<T>();
            DbSet.Load();
            Items = DbSet.Local.ToObservableCollection();
        }

        private void AddItem(object obj)
        {
            try
            {
                var newItem = CreateNewItem();
                Items.Add(newItem); //DbSet.Add(newItem);
                Message = "Новая запись добавлена";
                MessageBrush = Brushes.Green;
            }
            catch (Exception ex)
            {
                Message = $"Ошибка при добавлении: {ex.Message}";
                MessageBrush = Brushes.Red;
            }
        }

        private void DeleteItem(object obj)
        {
            try
            {
                if (SelectedItem != null)
                {
                    Items.Remove(SelectedItem); //DbSet.Remove(SelectedItem);
                    Message = "Запись удалена";
                    MessageBrush = Brushes.Green;
                }
                else
                {
                    Message = "Выберите строку для удаления.";
                    MessageBrush = Brushes.Orange;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                MessageBrush = Brushes.Red;
            }
        }

        private void SaveChanges(object obj)
        {
            try
            {
                // Получаем все измененные, добавленные или удаленные записи для таблицы T
                var changedEntries = Db.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Deleted)
                    .ToList();

                // Если изменения для таблицы T существуют, сохраняем их
                if (changedEntries.Count != 0)
                {
                    Db.SaveChanges(); // Сохраняем только изменения для текущего контекста
                    Message = $"Изменения для таблицы \"{TableName}\" успешно сохранены.";
                    MessageBrush = Brushes.Green; // Устанавливаем зеленый цвет для сообщения об успехе
                }
                else
                {
                    Message = $"Нет изменений для сохранения в таблице \"{TableName}\".";
                    MessageBrush = Brushes.Orange; // Устанавливаем оранжевый цвет для сообщения, если изменений нет
                }
            }
            catch (DbUpdateException dbEx)
            {
                // Обрабатываем исключения, связанные с обновлением базы данных
                Message = $"Ошибка обновления базы данных: {dbEx.Message}";
                MessageBrush = Brushes.Red; // Устанавливаем красный цвет для сообщения об ошибке
            }
            catch (Exception ex)
            {
                // Обрабатываем все остальные исключения
                Message = $"Ошибка сохранения изменений: {ex.Message}";
                MessageBrush = Brushes.Red; // Устанавливаем красный цвет для сообщения об ошибке
            }
        }


        private void CancelChanges(object obj)
        {
            try
            {
                var changedEntries = Db.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Deleted);

                if (changedEntries.ToList().Count == 0)
                {
                    Message = "Нечего отменять";
                    MessageBrush = Brushes.Orange;
                    return;
                }

                Db.ChangeTracker.Clear();
                LoadData(); // Перезагрузка данных, чтобы обновить коллекцию

                Message = "Изменения отменены.";
                MessageBrush = Brushes.Green;
            }
            catch (Exception ex)
            {
                Message = $"Ошибка при отмене изменений: {ex.Message}";
                MessageBrush = Brushes.Red;
            }
        }

        private void Search()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    LoadData(); // Перезагрузка данных, если строка поиска пустая
                    Message = string.Empty;
                    return;
                }

                var filteredItems = DbSet.Local.Where(item => FilterItem(item, SearchText)).ToList();
                Items = new ObservableCollection<T>(filteredItems);

                Message = $"Найдено записей: {filteredItems.Count}";
                MessageBrush = filteredItems.Count != 0 ? Brushes.Green : Brushes.Orange;
            }
            catch (Exception ex)
            {
                Message = $"Ошибка поиска: {ex.Message}";
                MessageBrush = Brushes.Red;
            }
        }


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

        public void OpenFromCache() => LoadData();


        private static void SelectAll(bool select, IEnumerable<T> models)
        {
            foreach (var model in models)
            {
                model.IsSelected = select;
            }
        }
    }
}
