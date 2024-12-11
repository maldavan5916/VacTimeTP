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
        private readonly Dictionary<string, Page> _pagesCache = [];
        private readonly PaletteHelper _paletteHelper = new();

        public MainWindow()
        {
            new DatabaseContext().Database.EnsureCreated();
            InitializeComponent();

            MainFrame.Navigate(new HomePage());

            switch (Properties.Settings.Default.AppTheme)
            {
                case "Dark": DarkRb.IsChecked = true; ChangeTheme("Dark"); break;
                case "Light": LightRB.IsChecked = true; ChangeTheme("Light"); break;
            }
        }

        private void MenuThemeSwitched(object sender, RoutedEventArgs e)
        {            
            if (sender is RadioButton themeRB)
            {
                ChangeTheme(themeRB.Tag.ToString());
            }            
        }

        private void ChangeTheme(string? toTheme)
        {
            var theme = _paletteHelper.GetTheme();

            switch (toTheme)
            {
                case "Dark": theme.SetDarkTheme(); Properties.Settings.Default.AppTheme = "Dark"; break;
                case "Light": theme.SetLightTheme(); Properties.Settings.Default.AppTheme = "Light"; break;
                default: return;
            }

            Properties.Settings.Default.Save();
            _paletteHelper.SetTheme(theme);
        }
       
        private void Close(object sender, RoutedEventArgs e) => Close();

        private void OpenAboutProgram(object sender, RoutedEventArgs e) => new AboutProgram().ShowDialog();

        private void CreateNewWindow(object sender, RoutedEventArgs e) => new MainWindow().Show();

        private void OpenHelp(object sender, RoutedEventArgs e) => OpenChmFile(@"UserGuide.chm");

        static void OpenChmFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Путь к файлу справки не указан.");
                return;
            }

            if (!System.IO.File.Exists(filePath))
            {
                MessageBox.Show($"Файл справки не найден: {filePath}");
                return;
            }

            try
            {
                // Запускаем файл
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "hh.exe", // Команда для открытия CHM
                    Arguments = filePath, // Передаем путь к файлу
                    UseShellExecute = true // Используем оболочку Windows
                });
            }
            catch (Exception ex)
            {
                // Обрабатываем возможные ошибки
                System.Diagnostics.Debug.WriteLine($"Не удалось открыть файл: {ex.Message}");
                MessageBox.Show($"Не удалось открыть файл: {ex.Message}");
            }
        }

        private void NavigateToPage(object sender, RoutedEventArgs e)
        {
            try
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
                        "StockBalanceReport" => new ViewReport.StockBalanceReport(),
                        "ContractorContractsReport" => new ViewReport.ContractorContractsReport(),
                        "ProductSalesReport" => new ViewReport.ProductSalesReport(),
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}