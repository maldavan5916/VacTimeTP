using DatabaseManager;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace VacTrack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly DatabaseContext Db = new();
        private readonly Dictionary<string, Page> _pagesCache = [];
        private readonly PaletteHelper _paletteHelper = new();

        public MainWindow()
        {
            Db.Database.EnsureCreated();
            InitializeComponent();

            MainFrame.Navigate(new HomePage());
        }

        private void MenuThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            var theme = _paletteHelper.GetTheme();

            if (MenuThemeToggle.IsChecked)
                theme.SetDarkTheme();
            else
                theme.SetLightTheme();

            _paletteHelper.SetTheme(theme);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenAboutProgram(object sender, RoutedEventArgs e)
        {
           throw new NotImplementedException();
        }

        private void NavigateToTable(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            string? pageKey = menuItem?.Tag.ToString();

            if (string.IsNullOrEmpty(pageKey))
                return;
            // Проверяем, есть ли страница в кэше
            if (!_pagesCache.TryGetValue(pageKey, out Page? targetPage))
            {
                // Создаем новую страницу, если её нет в кэше
                targetPage = pageKey switch
                {
                    //"Contract" => new ViewTables.ContractViewTable(Db),
                    //"Counterpartie" => new ViewTables.CounterpartieViewTable(Db),
                    "Division" => new ViewTables.DivisionViewTable(Db),
                    //"Employee" => new ViewTables.EmployeeViewTable(Db),
                    //"Location" => new ViewTables.LocationViewTable(Db),
                    //"Material" => new ViewTables.MaterialViewTable(Db),
                    "Post" => new ViewTables.PostViewTable(Db),
                    //"Product" => new ViewTables.ProductViewTable(Db),
                    //"Receipt" => new ViewTables.ReceiptViewTable(Db),
                    //"Sale" => new ViewTables.SaleViewTable(Db),
                    "Unit" => new ViewTables.UnitViewTable(Db),
                    "Home" => new HomePage(),
                    _ => new NotFoundPage("Запрашиваемая страница не найдена"),
                };
                // Добавляем новую страницу в кэш
                _pagesCache[pageKey] = targetPage;
            }
            // Навигация на найденную или созданную страницу
            MainFrame.Navigate(targetPage);
        }
    }
}