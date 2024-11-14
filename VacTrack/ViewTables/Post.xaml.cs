using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для Post.xaml
    /// </summary>
    public partial class Post : Page
    {
        private DatabaseContext Db;
        public Post(DatabaseContext db)
        {
            Db = db;
            InitializeComponent();
            Db.Posts.Load();
            PostTable.ItemsSource = Db.Posts.Local.ToObservableCollection();
        }

        private void SendMess(string text, System.Windows.Media.Brush brush)
        {
            mesLabel.Foreground = brush;
            mesLabel.Content = text;
        }

        private void AddRow(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создаем новый объект Unit
                var newPost = new DatabaseManager.Post { Name = "Новая должность" };
                // Добавляем его в контекст базы данных
                Db.Posts.Add(newPost);
                // Обновляем отображение таблицы
                PostTable.Items.Refresh();
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
                if (PostTable.SelectedItem is DatabaseManager.Post selectedPost)
                {
                    // Удаляем выбранный объект из контекста базы данных
                    Db.Posts.Remove(selectedPost);
                    // Обновляем отображение таблицы
                    PostTable.Items.Refresh();
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
                PostTable.Items.Refresh();
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
                    PostTable.ItemsSource = Db.Posts.Local.ToObservableCollection();
                    return;
                }
                string filterText = searchBox.Text.ToLower();
                // Применяем фильтрацию к локальной коллекции
                var filteredPost = Db.Posts.Local
                    .Where(post => post.Name != null && post.Name.Contains(filterText, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();
                // Обновляем источник данных DataGrid
                PostTable.ItemsSource = filteredPost;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при поиске: {ex.Message}");
            }
        }
    }
}
