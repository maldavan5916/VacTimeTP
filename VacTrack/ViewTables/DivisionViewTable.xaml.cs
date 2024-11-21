using DatabaseManager;
using System.Windows;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для DivisionViewTable.xaml
    /// </summary>
    public partial class DivisionViewTable : Page
    {
        public DivisionViewTable()
        {
            InitializeComponent();
        }
    }

    public class DivisionViewModel : BaseViewModel<Division>
    {
        public DivisionViewModel() : base(new DatabaseContext()) { TableName = "Подразделения"; }

        protected override Division CreateNewItem() => new() { Name = "Новое подразделение" };
        protected override bool FilterItem(Division item, string filter) =>
            item.Name != null && item.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
    }
}
