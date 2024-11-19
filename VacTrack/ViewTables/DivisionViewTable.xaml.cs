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
        private readonly BasePageLogic<Division> _PageLogic;

        public DivisionViewTable(DatabaseContext db)
        {
            InitializeComponent();
            _PageLogic = new DivisionPageLogic(db, dataTable, searchBox, mesLabel);
        }

        private void AddRow(object sender, RoutedEventArgs e) => _PageLogic.AddRow();
        private void DeleteRow(object sender, RoutedEventArgs e) => _PageLogic.DeleteRow();
        private void SaveChange(object sender, RoutedEventArgs e) => _PageLogic.SaveChanges();
        private void CancelChange(object sender, RoutedEventArgs e) => _PageLogic.CancelChanges();
    }

    public class DivisionPageLogic(DatabaseContext db, DataGrid mainDataGrid, TextBox searchBox, Label messageLabel) :
        BasePageLogic<Division>(db, db.Divisions, mainDataGrid, searchBox, messageLabel)
    {
        protected override Division CreateNewItem() => new() { Name = "Новое подразделение" };

        protected override bool FilterItem(Division item, string filter) =>
            item.Name != null && item.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
    }
}
