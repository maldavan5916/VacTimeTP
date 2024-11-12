using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для Unit.xaml
    /// </summary>
    public partial class Unit : Page
    {
        private DatabaseContext Db;
        public Unit(DatabaseContext db)
        {
            Db = db;
            InitializeComponent();
            Db.Units.Load();
            UnitTable.ItemsSource = Db.Units.Local.ToObservableCollection();
        }
    }
}
