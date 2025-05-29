using DatabaseManager;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для CounterpartieViewTable.xaml
    /// </summary>
    public partial class CounterpartieViewTable : Page, ICachedPage
    {
        private CounterpartieViewModel ThisViewModel => (CounterpartieViewModel)DataContext;
        public CounterpartieViewTable() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.Refresh();
    }

    public class CounterpartieViewModel : BaseViewModel<Counterpartie>
    {
        public List<CounterpartieType> Types { get; } = [CounterpartieType.Fiz, CounterpartieType.Ur];

        public CounterpartieViewModel() : 
            base(new DatabaseContext(Tools.AppSession.CurrentUserIsReadOnly)) 
        { TableName = "Контрагенты"; }

        protected override Counterpartie CreateNewItem() => new()
        {
            Name = "Новый контрагент",
            LegalAddress = "Юридический адрес",
            PhoneNomber = "Номер телефона",
            PostalAddress = "Почтовый адрес",
            Unp = "УНП",
            BankAccount = "Счёт"
        };

        protected override bool FilterItem(Counterpartie item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;
    }
}
