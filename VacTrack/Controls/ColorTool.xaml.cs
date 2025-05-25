using System.Windows.Controls;

namespace VacTrack.Controls
{
    /// <summary>
    /// Interaction logic for ColorTool.xaml
    /// </summary>
    public partial class ColorTool : UserControl
    {
        public ColorTool()
        {
            DataContext = new ColorToolViewModel();
            InitializeComponent();
        }
    }
}
