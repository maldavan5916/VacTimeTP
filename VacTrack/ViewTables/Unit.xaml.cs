using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для Unit.xaml
    /// </summary>
    public partial class Unit : Page
    {
        private DatabaseContext Db;
        public Unit(DatabaseContext db)
        {
            Db = db;
            InitializeComponent();
            Db.Units.Load();
            UnitTable.ItemsSource = Db.Units.Local.ToObservableCollection();
        }

        private void SendMess(string text, System.Windows.Media.Brush brush)
        {
            mesLabel.Content = "...";
            mesLabel.Foreground = brush;
            mesLabel.Content = text;
        }

        private void AddRow(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создаем новый объект Unit
                var newUnit = new DatabaseManager.Unit { Name = "Новая единица" };
                // Добавляем его в контекст базы данных
                Db.Units.Add(newUnit);
                // Обновляем отображение таблицы
                UnitTable.Items.Refresh();
                SendMess("Новая запись создана", System.Windows.Media.Brushes.Green);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при добавлении: {ex.Message}");
            }
        }

        private void DeleteRow(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, что выбранная строка не пустая
                if (UnitTable.SelectedItem is DatabaseManager.Unit selectedUnit)
                {
                    // Удаляем выбранный объект из контекста базы данных
                    Db.Units.Remove(selectedUnit);
                    // Обновляем отображение таблицы
                    UnitTable.Items.Refresh();
                    SendMess("Запись удалена", System.Windows.Media.Brushes.Green);
                }
                else
                    System.Windows.MessageBox.Show("Выберите строку для удаления.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при удалении: {ex.Message}");
            }
        }

        private void SaveChange(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сохраняем все изменения в базе данных
                Db.SaveChanges();
                SendMess("Изменения успешно сохранены.", System.Windows.Media.Brushes.Green);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void CancelChange(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, есть ли изменения
                if (!Db.ChangeTracker.HasChanges())
                {
                    SendMess("Нечего отменять", System.Windows.Media.Brushes.Yellow);
                    return;
                }
                // Откатываем изменения только для измененных записей
                foreach (var entry in Db.ChangeTracker.Entries())
                {
                    if (entry.State == EntityState.Modified)
                    {
                        entry.Reload();
                    }
                    else if (entry.State == EntityState.Added)
                    {
                        entry.State = EntityState.Detached;
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        entry.State = EntityState.Unchanged;
                    }
                }
                // Обновляем таблицу
                UnitTable.Items.Refresh();
                SendMess("Отменено", System.Windows.Media.Brushes.Green);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при отмене: {ex.Message}");
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(searchBox.Text))
                {
                    SendMess("Пустой запрос", System.Windows.Media.Brushes.Yellow);
                    UnitTable.ItemsSource = Db.Units.Local.ToObservableCollection();
                    return;
                }
                string filterText = searchBox.Text.ToLower();
                // Применяем фильтрацию к локальной коллекции
                var filteredUnits = Db.Units.Local
                    .Where(unit => unit.Name != null && unit.Name.Contains(filterText, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
                // Обновляем источник данных DataGrid
                UnitTable.ItemsSource = filteredUnits;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при поиске: {ex.Message}");
            }
        }
    }
}
