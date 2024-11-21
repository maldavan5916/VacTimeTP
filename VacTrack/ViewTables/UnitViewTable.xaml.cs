using DatabaseManager;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для Unit.xaml
    /// </summary>
    public partial class UnitViewTable : Page
    {
        public UnitViewTable()
        {
            InitializeComponent();
        }
    }

    public class UnitViewModel : BaseViewModel<Unit>
    {
        public UnitViewModel() : base(new DatabaseContext()) { TableName = "Единицы измерения"; }

        protected override Unit CreateNewItem() => new() { Name = "Новая единица" };
        protected override bool FilterItem(Unit item, string filter) =>
            item.Name != null && item.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
    }
}
