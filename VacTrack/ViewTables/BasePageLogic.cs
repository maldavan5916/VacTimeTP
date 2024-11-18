using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    public abstract class BasePageLogic<T> : Page where T : class
    {
        protected DatabaseContext Db;
        protected DbSet<T> DbSet;

        private DataGrid MainDataGrid;
        private TextBox SearchBox;
        private Label MessageLabel;

        public BasePageLogic(DatabaseContext db, DbSet<T> dbSet, DataGrid mainDataGrid, TextBox searchBox, Label messageLabel)
        {
            Db = db;
            DbSet = dbSet;
            MainDataGrid = mainDataGrid;
            SearchBox = searchBox;
            MessageLabel = messageLabel;

            DbSet.Load();
            MainDataGrid.ItemsSource = DbSet.Local.ToObservableCollection();

            if (SearchBox != null)
            {
                SearchBox.TextChanged += SearchBox_TextChanged;
            }
        }

        protected abstract T CreateNewItem();

        protected abstract bool FilterItem(T item, string filter);

        private void ShowMessage(string text, System.Windows.Media.Brush brush)
        {
            MessageLabel.Content = text;
            MessageLabel.Foreground = brush;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filterText = SearchBox.Text.ToLower();
            MainDataGrid.ItemsSource = string.IsNullOrWhiteSpace(filterText)
                ? DbSet.Local.ToObservableCollection()
                : DbSet.Local.Where(item => FilterItem(item, filterText)).ToList();
        }

        public void AddRow()
        {
            try
            {
                var newItem = CreateNewItem();
                DbSet.Add(newItem);
                MainDataGrid.Items.Refresh();
                ShowMessage("Новая запись добавлена", System.Windows.Media.Brushes.Green);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при добавлении: {ex.Message}");
            }
        }

        public void DeleteRow()
        {
            try
            {
                if (MainDataGrid.SelectedItem is T selectedItem)
                {
                    DbSet.Remove(selectedItem);
                    MainDataGrid.Items.Refresh();
                    ShowMessage("Запись удалена", System.Windows.Media.Brushes.Green);
                }
                else
                    System.Windows.MessageBox.Show("Выберите строку для удаления.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        public void SaveChanges()
        {
            try
            {
                Db.SaveChanges();
                ShowMessage("Изменения сохранены", System.Windows.Media.Brushes.Green);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        public void CancelChanges()
        {
            try
            {
                if (!Db.ChangeTracker.HasChanges())
                {
                    ShowMessage("Нечего отменять", System.Windows.Media.Brushes.Yellow);
                    return;
                }
                foreach (var entry in Db.ChangeTracker.Entries())
                {
                    if (entry.State == EntityState.Modified)
                        entry.Reload();
                    else if (entry.State == EntityState.Added)
                        entry.State = EntityState.Detached;
                    else if (entry.State == EntityState.Deleted)
                        entry.State = EntityState.Unchanged;
                }
                MainDataGrid.Items.Refresh();
                ShowMessage("Отменено", System.Windows.Media.Brushes.Green);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при отмене: {ex.Message}");
            }
        }
    }
}
