using System.Diagnostics;
using System.Windows;

namespace VacTrack
{
    /// <summary>
    /// Логика взаимодействия для AboutProgram.xaml
    /// </summary>
    public partial class AboutProgram : Window
    {
        public AboutProgram()
        {
            InitializeComponent();
        }

        private void GitHubOpen(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://github.com/maldavan5916/VacTimeTP") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OK_BtnClick(object sender, RoutedEventArgs e) => Close();
    }
}
