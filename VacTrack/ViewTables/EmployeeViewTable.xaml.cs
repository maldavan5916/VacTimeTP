﻿using DatabaseManager;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VacTrack.ViewTables
{
    /// <summary>
    /// Логика взаимодействия для EmployeeViewTable.xaml
    /// </summary>
    public partial class EmployeeViewTable : Page, ICachedPage
    {
        private EmployeeViewModel ThisViewModel => (EmployeeViewModel)DataContext;
        public EmployeeViewTable() => InitializeComponent();
        public void OnNavigatedFromCache() => ThisViewModel.Refresh();
    }

    public class EmployeeViewModel : BaseViewModel<Employee>
    {
        public ObservableCollection<Division>? EmployDivisions { get; set; }
        public ObservableCollection<Post>? EmployPosts { get; set; }

        public EmployeeViewModel() : 
            base(new DatabaseContext(Tools.AppSession.CurrentUserIsReadOnly)) 
        { }

        protected override void LoadData()
        {
            TableName = "Сотрудники";
            EmployDivisions = new ObservableCollection<Division>([.. Db.Divisions]);
            EmployPosts = new ObservableCollection<Post>([.. Db.Posts]);

            DbSet = Db.Set<Employee>();
            DbSet.Include(e => e.Division).Include(e => e.Post).Load();

            Items = DbSet.Local.ToObservableCollection();
        }

        protected override Employee CreateNewItem() => new()
        {
            Fio = "Новый сотрудник",
            DateHire = DateTime.Now,
            DateOfBirth = DateTime.Now,
            PhoneNumber = "+375",
            Address = "Адрес",
            BankDetails = "номер счёта",
            PassportData = "паспорт"
        };

        protected override bool FilterItem(Employee item, string? searchText) =>
            string.IsNullOrWhiteSpace(searchText) ||
            item.PersonnelNumber?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Fio?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Division?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true ||
            item.Post?.Name?.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) == true;
    }
}
