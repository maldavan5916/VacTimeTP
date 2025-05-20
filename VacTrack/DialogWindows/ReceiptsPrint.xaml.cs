using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DatabaseManager;
using Microsoft.EntityFrameworkCore;

namespace VacTrack.DialogWindows
{
    /// <summary>
    /// Логика взаимодействия для ReceiptsPrint.xaml
    /// </summary>
    public partial class ReceiptsPrint : Window
    {
        private ReceiptsPrintVM ThisVM => (ReceiptsPrintVM)DataContext;
        
        public ReceiptsPrintVM? ReceiptsPrintVM {  get; private set; }
        public ObservableCollection<Receipt>? Items { get; set; }
        
        public ReceiptsPrint() => InitializeComponent();
        private void Cansel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
        private void Accept_Click(object sender, RoutedEventArgs e)
        {

            Items = ThisVM.GetItems();

            if (Items.Count > 0)
            {
                ReceiptsPrintVM = ThisVM;
                DialogResult = true;
            }
            else
            {
                ThisVM.Message = "Ничего не найдено";
            }
        }
    }

    public class ReceiptsPrintVM : INotifyPropertyChanged
    {
        public ObservableCollection<Counterpartie> ReceiptCounterpartie { get; set; }
        public ObservableCollection<Location> ReceiptLocation { get; set; }
        public ObservableCollection<Material> ReceiptMaterial { get; set; }

        private string? _message;
        public string? Message {
            get => _message; 
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

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

        public DatabaseContext Db;

        public Counterpartie? SelectCounterpartie { get; set; }
        public Location? SelectLocation { get; set; }
        public Material? SelectMaterial { get; set; }
        public DateTime? SelectStartDate { get; set; }
        public DateTime? SelectEndDate { get; set; }

        public ReceiptsPrintVM()
        {
            Db = new DatabaseContext();

            ReceiptCounterpartie = new ObservableCollection<Counterpartie>([.. Db.Counterparties]);
            ReceiptLocation = new ObservableCollection<Location>([.. Db.Locations]);
            ReceiptMaterial = new ObservableCollection<Material>([.. Db.Materials]);
        }

        public ObservableCollection<Receipt> GetItems()
        {
            var DbSet = Db.Set<Receipt>();
            DbSet.Include(e => e.Counterpartie)
                .Include(e => e.Material)
                .ThenInclude(e => e != null ? e.Unit : null)
                .Load();

            return [.. DbSet.Local.Where(item =>
                (SelectCounterpartie == null || item.Counterpartie?.Id == SelectCounterpartie.Id) &&
                (SelectMaterial == null || item.Material?.Id == SelectMaterial.Id) &&

                ((SelectStartDate == null && SelectEndDate == null) ||

                (SelectStartDate != null && SelectEndDate != null &&
                    item.Date.Date >= SelectStartDate.Value.Date && item.Date.Date <= SelectEndDate.Value.Date) ||

                (SelectStartDate != null && SelectEndDate == null && item.Date.Date == SelectStartDate.Value.Date) ||
                (SelectStartDate == null && SelectEndDate != null && item.Date.Date == SelectEndDate.Value.Date))
                ).ToList()];
        }
    }
}
