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
        private readonly Dictionary<string, Page> _pagesCache = new();
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();

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
            new AboutBox().Show();
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
                    "Contract" => new ViewTables.Contract(),
                    "Counterpartie" => new ViewTables.Counterpartie(),
                    "Division" => new ViewTables.Division(),
                    "Employee" => new ViewTables.Employee(),
                    "Location" => new ViewTables.Location(),
                    "Material" => new ViewTables.Material(),
                    "Post" => new ViewTables.Post(),
                    "Product" => new ViewTables.Product(),
                    "Receipt" => new ViewTables.Receipt(),
                    "Sale" => new ViewTables.Sale(),
                    "Unit" => new ViewTables.Unit(Db),
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