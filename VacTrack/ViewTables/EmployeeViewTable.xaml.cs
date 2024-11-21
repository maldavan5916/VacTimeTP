using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для EmployeeViewTable.xaml
    /// </summary>
    public partial class EmployeeViewTable : Page
    {
        public EmployeeViewTable()
        {
            InitializeComponent();
        }
    }

    public class EmployeeViewModel : BaseViewModel<Employee>
    {
        public ObservableCollection<Division>? EmployDivisions { get; set; }
        public ObservableCollection<Post>? EmployPosts { get; set; }

        public EmployeeViewModel() : base(new DatabaseContext()) { }

        protected override void LoadData()
        {
            TableName = "Сотрудники";
            DbSet = Db.Set<Employee>();
            DbSet.Load();
            EmployDivisions = new ObservableCollection<Division>([.. Db.Divisions]);
            EmployPosts = new ObservableCollection<Post>([.. Db.Posts]);
            
            Items = new ObservableCollection<Employee>([.. Db.Employees
                .Include(e => e.Division)
                .Include(e => e.Post)
                ]);
        }

        protected override Employee CreateNewItem() => new() { Fio = "Новый сотрудник" };
        protected override bool FilterItem(Employee item, string filter) =>
            item.Fio != null && item.Fio.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
    }
}
