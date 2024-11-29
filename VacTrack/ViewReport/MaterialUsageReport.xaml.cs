using DatabaseManager;
using System.Windows.Controls;
using VacTrack.ViewTables;

namespace VacTrack.ViewReport
{
    /// <summary>
    /// Логика взаимодействия для MaterialUsageReport.xaml
    /// </summary>
    public partial class MaterialUsageReport : Page
    {
        private MaterialUsageReportViewModel ThisViewModel => (MaterialUsageReportViewModel)DataContext;
        public MaterialUsageReport() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public class MaterialUsageReportViewModel : BaseReportViewModel<Product>
    {

    }
}
