using DatabaseManager;
using System.Windows;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для Unit.xaml
    /// </summary>
    public partial class UnitViewTable : Page
    {
        private readonly BasePageLogic<Unit> _PageLogic;

        public UnitViewTable(DatabaseContext db)
        {
            InitializeComponent();
            _PageLogic = new UnitPageLogic(db, dataTable, searchBox, mesLabel);
        }

        private void AddRow(object sender, RoutedEventArgs e) => _PageLogic.AddRow();
        private void DeleteRow(object sender, RoutedEventArgs e) => _PageLogic.DeleteRow();
        private void SaveChange(object sender, RoutedEventArgs e) => _PageLogic.SaveChanges();
        private void CancelChange(object sender, RoutedEventArgs e) => _PageLogic.CancelChanges();
    }

    public class UnitPageLogic(DatabaseContext db, DataGrid mainDataGrid, TextBox searchBox, Label messageLabel) :
        BasePageLogic<Unit>(db, db.Units, mainDataGrid, searchBox, messageLabel)
    {
        protected override Unit CreateNewItem() => new() { Name = "Новая единица" };

        protected override bool FilterItem(Unit item, string filter) =>
            item.Name != null && item.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
    }
}
