using DatabaseManager;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для Post.xaml
    /// </summary>
    public partial class PostViewTable : Page
    {
        public PostViewTable()
        {
            InitializeComponent();
        }
    }

    public class PostViewModel : BaseViewModel<Post>
    {
        public PostViewModel() : 
            base(new DatabaseContext(Tools.AppSession.CurrentUserIsReadOnly)) 
        { TableName = "Должности"; }

        protected override Post CreateNewItem() => new() { Name = "Новая должность" };
        protected override bool FilterItem(Post item, string? filter) =>
            string.IsNullOrWhiteSpace(filter) ||
            item.Name != null && item.Name.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
    }
}