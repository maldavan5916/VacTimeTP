using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace VacTrack.ViewReport
{
    public abstract class BaseReportViewModel<T> : INotifyPropertyChanged where T : BaseModel
    {
        #region interface implemented 
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void SetProperty<Ts>(ref Ts field, Ts value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<Ts>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        #region properties
        protected DatabaseContext Db;
        protected DbSet<T> DbSet;


        private ObservableCollection<T> _Items;
        public ObservableCollection<T> Items
        {
            get => _Items;
            set => SetProperty(ref _Items, value);
        }
        #endregion

        public BaseReportViewModel()
        {
            Db = new DatabaseContext();
            Db.Database.EnsureCreated();
            LoadData();
            if (DbSet == null || _Items == null) throw new Exception("Data loading error");
        }

        protected virtual void LoadData()
        {
            DbSet = Db.Set<T>();
            DbSet.Load();
            Items = DbSet.Local.ToObservableCollection();
        }

        public void OpenFromCache() => LoadData();
    }
}
