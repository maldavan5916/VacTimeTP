using DatabaseManager;
using System.Windows;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для Post.xaml
    /// </summary>
    public partial class PostViewTable : Page
    {
        private readonly BasePageLogic<Post> _PageLogic;

        public PostViewTable(DatabaseContext db)
        {
            InitializeComponent();
            _PageLogic = new PostPageLogic(db, dataTable, searchBox, mesLabel);
        }

        private void AddRow(object sender, RoutedEventArgs e) => _PageLogic.AddRow();
        private void DeleteRow(object sender, RoutedEventArgs e) => _PageLogic.DeleteRow();
        private void SaveChange(object sender, RoutedEventArgs e) => _PageLogic.SaveChanges();
        private void CancelChange(object sender, RoutedEventArgs e) => _PageLogic.CancelChanges();
    }

    public class PostPageLogic(DatabaseContext db, DataGrid mainDataGrid, TextBox searchBox, Label messageLabel) :
        BasePageLogic<Post>(db, db.Posts, mainDataGrid, searchBox, messageLabel)
    {
        protected override Post CreateNewItem() => new() { Name = "Новая должность" };

        protected override bool FilterItem(Post item, string filter) =>
            item.Name != null && item.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
    }
}
