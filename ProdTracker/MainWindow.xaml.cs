using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.System;

namespace ProdTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationContext db = new ApplicationContext();
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        // при загрузке окна
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // гарантируем, что база данных создана
            db.Database.EnsureCreated();
            // загружаем данные из БД
            db.Units.Load();
            // и устанавливаем данные в качестве контекста
            DataContext = db.Units.Local.ToObservableCollection();
        }

        // добавление
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            UnitWindow UnitWindow = new(new Unit());
            if (UnitWindow.ShowDialog() == true)
            {
                Unit Unit = UnitWindow.Unit;
                db.Units.Add(Unit);
                db.SaveChanges();
            }
        }
        // редактирование
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // получаем выделенный объект
            Unit? unit = usersList.SelectedItem as Unit;
            // если ни одного объекта не выделено, выходим
            if (unit is null) return;

            UnitWindow UserWindow = new(new Unit
            {
                Id = unit.Id,
                Name = unit.Name
            });

            if (UserWindow.ShowDialog() == true)
            {
                // получаем измененный объект
                unit = db.Units.Find(UserWindow.Unit.Id);
                if (unit != null)
                {
                    unit.Id = UserWindow.Unit.Id;
                    unit.Name = UserWindow.Unit.Name;
                    db.SaveChanges();
                    usersList.Items.Refresh();
                }
            }
        }
        // удаление
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // получаем выделенный объект
            Unit? user = usersList.SelectedItem as Unit;
            // если ни одного объекта не выделено, выходим
            if (user is null) return;
            db.Units.Remove(user);
            db.SaveChanges();
        }
    }
}   