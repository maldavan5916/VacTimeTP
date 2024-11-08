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
using DatabaseManager;
using MaterialDesignThemes.Wpf;

namespace VacTrack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseContext Db = new ();
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();

        public MainWindow()
        {
            Db.Database.EnsureCreated();
            InitializeComponent();
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

        private void OpenAboutProgram (object sender, RoutedEventArgs e)
        {
            new AboutBox().Show();
        }
    }
}