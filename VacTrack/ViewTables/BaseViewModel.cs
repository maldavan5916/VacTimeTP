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
    public abstract class BaseViewModel<T> : INotifyPropertyChanged where T : class
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected abstract T CreateNewItem();
        protected abstract bool FilterItem(T item, string filter);

        protected DatabaseContext Db;
        protected DbSet<T> DbSet;
        private System.Timers.Timer? _resetTimer;

        public ObservableCollection<T> Items { get; set; }
        private T? _SelectedItem; public T? SelectedItem { get => _SelectedItem; set { _SelectedItem = value; OnPropertyChanged(); } }
        private string? _Message; public string? Message { get => _Message; set { _Message = value; OnPropertyChanged(); StartResetTimer(); } }
        private Brush? _MessageBrush; public Brush? MessageBrush { get => _MessageBrush; set { _MessageBrush = value; OnPropertyChanged(); } }
        private string? _SearchText; public string? SearchText { get => _SearchText; set { _SearchText = value; OnPropertyChanged(); Search(); } }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public BaseViewModel(DatabaseContext db)
        {
            Db = db;
            DbSet = Db.Set<T>();
            
            Db.Database.EnsureCreated();
            
            DbSet.Load();
            Items = DbSet.Local.ToObservableCollection();
            AddCommand = new RelayCommand(AddItem);
            DeleteCommand = new RelayCommand(DeleteItem);
            SaveCommand = new RelayCommand(SaveChanges);
            CancelCommand = new RelayCommand(CancelChanges);
        }
        public void AddItem(object obj)
        {
            try
            {
                var newItem = CreateNewItem();
                DbSet.Add(newItem);
                Message = "Новая запись добавлена";
                MessageBrush = Brushes.Green;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при добавлении: {ex.Message}");
            }
        }

        public void DeleteItem(object obj)
        {
            try
            {
                if (SelectedItem != null)
                {
                    DbSet.Remove(SelectedItem);
                    Message = "Запись удалена";
                    MessageBrush = Brushes.Green;
                }
                else
                    System.Windows.MessageBox.Show("Выберите строку для удаления.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        public void SaveChanges(object obj)
        {
            try
            {
                Db.SaveChanges();
                Message = "Изменения сохранены.";
                MessageBrush = Brushes.Green;
            }
            catch (Exception ex)
            {
                Message = $"Ошибка при сохранении: {ex.Message}";
                MessageBrush = Brushes.Red;
            }
        }

        public void CancelChanges(object obj)
        {
            try
            {
                var changedEntries = Db.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Deleted);

                foreach (var entry in changedEntries)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entry.CurrentValues.SetValues(entry.OriginalValues); // Отменяем изменения
                            entry.State = EntityState.Unchanged;
                            break;
                        case EntityState.Added:
                            entry.State = EntityState.Detached; // Удаляем добавленные записи
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Unchanged; // Восстанавливаем удаленные записи
                            break;
                    }
                }

                Message = "Изменения отменены.";
                MessageBrush = Brushes.Orange;
            }
            catch (Exception ex)
            {
                Message = $"Ошибка при отмене изменений: {ex.Message}";
                MessageBrush = Brushes.Red;
            }
        }

        public void Search ()
        {
            try
            {
                var filteredItems = (string.IsNullOrWhiteSpace(SearchText) ? DbSet.Local
                : DbSet.Local.Where(item => FilterItem(item, SearchText)));

                Items = new ObservableCollection<T>(filteredItems);
            } catch (Exception ex)
            {
                Message = $"Ошибка поиска: {ex.Message}";
                MessageBrush = Brushes.Red;
            }
            OnPropertyChanged(nameof(Items));
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
            // Очищаем сообщение после 30 секунд
            Message = string.Empty;
            // Останавливаем таймер после срабатывания
            _resetTimer?.Stop();
        }
    }
}
