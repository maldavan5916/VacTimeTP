using System.Windows.Controls;

namespace VacTrack
{
    /// <summary>
    /// Логика взаимодействия для NotFoundPage.xaml
    /// </summary>
    public partial class NotFoundPage : Page
    {
        public NotFoundPage(string message)
        {
            InitializeComponent();
            textBlock.Text = message;
        }
    }
}
