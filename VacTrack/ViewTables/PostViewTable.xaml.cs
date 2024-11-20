using DatabaseManager;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для Post.xaml
    /// </summary>
    public partial class PostViewTable : Page
    {

        public PostViewTable(DatabaseContext db)
        {
            DataContext = new PostViewModel(db);
            InitializeComponent();
        }
    }

    public class PostViewModel : BaseViewModel<Post>
    {
        public PostViewModel(DatabaseContext db) : base(db) { }
        public PostViewModel() : base(new DatabaseContext()) { }
       
        protected override Post CreateNewItem() => new() { Name = "Новая должность" };
        protected override bool FilterItem(Post item, string filter) =>
            item.Name != null && item.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
    }
}
