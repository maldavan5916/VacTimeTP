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
        DatabaseContext Db = new();
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

            Page targetPage = menuItem?.Tag.ToString() switch
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
                "Unit" => new ViewTables.Unit(),
                "Home" => new HomePage(),
                _ => new NotFoundPage("Запрашиваемая страница не найдена"),
            };
            MainFrame.Navigate(targetPage);
        }
    }
}