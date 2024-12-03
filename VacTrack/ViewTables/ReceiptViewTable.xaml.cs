using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для ReceiptViewTable.xaml
    /// </summary>
    public partial class ReceiptViewTable : Page, ICachedPage
    {
        private ReceiptViewModel ThisViewModel => (ReceiptViewModel)DataContext;
        public ReceiptViewTable() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.OpenFromCache();
    }

    public class ReceiptViewModel : BaseViewModel<Receipt>
    {
        public ObservableCollection<Counterpartie>? ReceiptCounterpartie { get; set; }
        public ObservableCollection<Material>? ReceiptMaterial { get; set; }

        public ICommand PrintCommand { get; }

        public ReceiptViewModel() : base(new DatabaseContext())
        {
            PrintCommand = new RelayCommand(PrintDoc);
        }
        
        protected override void LoadData()
        {
            TableName = "Поступления";
            ReceiptCounterpartie = new ObservableCollection<Counterpartie>([.. Db.Counterparties]);
            ReceiptMaterial = new ObservableCollection<Material>([.. Db.Materials]);

            DbSet = Db.Set<Receipt>();
            DbSet.Include(e => e.Counterpartie)
                .Include(e => e.Material)
                .ThenInclude(e => e != null ? e.Unit : null)
                .Include(e => e.Material != null ? e.Material.Location : null)
                .ThenInclude(e => e != null ? e.Employee : null)
                .Load();

            Items = DbSet.Local.ToObservableCollection();
        }

        protected override Receipt CreateNewItem() => new() { Date = DateTime.Now };

        protected override bool FilterItem(Receipt item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.Counterpartie?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Material?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Summ.ToString().Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;

        private void PrintDoc(object obj)
        {
            if (SelectedItem != null)
                new ViewReport.ReceiptReport(SelectedItem).ShowDialog();
            else
            {
                Message = "Выберете поступление";
                MessageBrush = Brushes.Orange;
            }
        }
    }
}
