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

        private void NavigateToPage(object sender, RoutedEventArgs e)
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
                    "Contract" => new ViewTables.ContractViewTable(),
                    "Counterpartie" => new ViewTables.CounterpartieViewTable(),
                    "Division" => new ViewTables.DivisionViewTable(),
                    "Employee" => new ViewTables.EmployeeViewTable(),
                    "Location" => new ViewTables.LocationViewTable(),
                    "Material" => new ViewTables.MaterialViewTable(),
                    "Post" => new ViewTables.PostViewTable(),
                    "Product" => new ViewTables.ProductViewTable(),
                    "Receipt" => new ViewTables.ReceiptViewTable(),
                    "Sale" => new ViewTables.SaleViewTable(),
                    "Unit" => new ViewTables.UnitViewTable(),
                    "Home" => new HomePage(),
                    "MaterialUsageReport" => new ViewReport.MaterialUsageReport(),
                    "EmployeeDivisionReport" => new ViewReport.EmployeeDivisionReport(),
                    //"StockBalanceReport" => new ViewReport.StockBalanceReport(),
                    //"ContractorContractsReport" => new ViewReport.ContractorContractsReport(),
                    //"ProductSalesReport" => new ViewReport.ProductSalesReport(),
                    _ => new NotFoundPage("Запрашиваемая страница не найдена"),
                };
                // Добавляем новую страницу в кэш
                _pagesCache[pageKey] = targetPage;
            }
            else
            {
                // Если страница из кэша, уведомляем её
                if (targetPage is ICachedPage cachedPage)
                    cachedPage.OnNavigatedFromCache();
            }
            // Навигация на найденную или созданную страницу
            MainFrame.Navigate(targetPage);
        }
    }
}