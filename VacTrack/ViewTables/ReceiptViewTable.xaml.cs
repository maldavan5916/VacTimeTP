using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VacTrack.DialogWindows;


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
        public ICommand AddItemsCommand { get; }
        public new ICommand DeleteCommand { get; }

        public ReceiptViewModel() : 
            base(new DatabaseContext(Tools.AppSession.CurrentUserIsReadOnly))
        {
            PrintCommand = new RelayCommand(PrintDoc);
            AddItemsCommand = new RelayCommand(AddItems);
            DeleteCommand = new RelayCommand(DeleteItem);
        }

        protected override void LoadData()
        {
            TableName = "Поступления";
            ReceiptCounterpartie = new ObservableCollection<Counterpartie>([.. Db.Counterparties]);

            var MaterDbSet = Db.Set<Material>();
            MaterDbSet.Include(m => m.Unit).Load();
            ReceiptMaterial = MaterDbSet.Local.ToObservableCollection();

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
            var printWindow = new ReceiptsPrint();
            bool? dialogResult = printWindow.ShowDialog();

            if (dialogResult == true)
            {
                if (printWindow.Items != null && printWindow.ReceiptsPrintVM != null)
                    new ViewReport.ReceiptReport(
                        printWindow.Items,
                        printWindow.ReceiptsPrintVM
                        ).Show();
                else
                    throw new Exception("Cannot display the report because either `Items` or `ReceiptsPrintVM` is null.");
            }
        }

        private void AddItems(object obj)
        {
            var ReceiptsWindow = new ReceiptsAdd();
            bool? dialogResult = ReceiptsWindow.ShowDialog();

            if (dialogResult == true)
            {
                var newItems = ReceiptsWindow.NewItems;

                if (newItems == null)
                {
                    Message = "Нет элементов для добавления";
                    MessageBrush = Brushes.Orange;
                    return;
                }

                foreach (var item in newItems)
                {
                    if (item.Material == null ||
                        item.Counterpartie == null)
                    {
                        Message = "Сведения о материале или контрагенте не были получены";
                        MessageBrush = Brushes.Red;
                        return;
                    }

                    Items.Add(new Receipt
                    {
                        Id = item.Id,
                        MaterialId = item.Material.Id,
                        Date = item.Date,
                        Count = item.Count,
                        CounterpartyId = item.Counterpartie.Id,
                        Summ = item.Summ
                    });

                    Db.Materials.First(m => m.Id == item.Material.Id).Count += item.Count;
                }

                Message = "Успешно добалено";
                MessageBrush = Brushes.Green;
            }
        }

        private new void DeleteItem(object obj)
        {
            Receipt? tempSelectedItem = SelectedItem;
            base.DeleteItem(obj);
            if (tempSelectedItem == null) { return; }
            Db.Materials.First(m => m.Id == tempSelectedItem.MaterialId).Count -= tempSelectedItem.Count;
        }
    }
}
