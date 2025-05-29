using DatabaseManager;
using System.Windows.Controls;
using System.Windows.Input;

namespace VacTrack.Controls
{
    /// <summary>
    /// Interaction logic for UserListView.xaml
    /// </summary>
    public partial class UserListView : UserControl
    {
        public UserListView() => InitializeComponent();
    }

    public class UserListViewModel : ViewTables.BaseViewModel<Users>
    {
        public new ICommand AddCommand { get; }

        public UserListViewModel() : 
            base(new DatabaseContext(Tools.AppSession.CurrentUserIsReadOnly))
        {
            TableName = "Пользователи";
            AddCommand = new RelayCommand(AddUser);
        }

        private void AddUser(object obj)
        {
            var dialog = new CreateUser();
            bool? dialogResult = dialog.ShowDialog();

            if (dialogResult != null)
                LoadData();
        }

        protected override Users CreateNewItem() =>
            throw new NotImplementedException();

        protected override bool FilterItem(Users item, string? searchText) =>
            searchText != null &&
            (
                item.Login != null && item.Login.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) ||
                item.Access != null && item.Access.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)
            );
    }
}
