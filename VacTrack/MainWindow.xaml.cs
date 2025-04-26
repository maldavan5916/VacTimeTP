using System.Windows;
using System.Windows.Controls;
using DatabaseManager;
using MaterialDesignThemes.Wpf;

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
            var Db = new DatabaseContext();
            Db.Database.EnsureCreated();
            InitializeComponent();

            MainFrame.Navigate(new HomePage());

            SetTheme(Properties.Settings.Default.AppTheme);
        }

        private void SetTheme(string SelectTheme)
        {
            var theme = _paletteHelper.GetTheme();

            switch (SelectTheme)
            {
                case "Dark": theme.SetBaseTheme(BaseTheme.Dark); break;
                case "Light": theme.SetBaseTheme(BaseTheme.Light); break;
                default: { theme.SetBaseTheme(BaseTheme.Inherit); } break;
            }

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
                        "ProcurementSheetReport" => new ViewReport.ProcurementSheetReport(),
                        "Settings" => new SettingsPage(),
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