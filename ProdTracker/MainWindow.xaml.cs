using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.System;

namespace ProdTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ApplicationContext VacDB = new ApplicationContext();
        public MainWindow()
        {
            InitializeComponent();
            VacDB.Database.EnsureCreated();
        }

        private void Exit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OpenAboutProgramm(object sender, EventArgs e)
        {
            new AboutWindow().ShowDialog();
        }
    }
}