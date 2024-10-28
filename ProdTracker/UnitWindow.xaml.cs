using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProdTracker
{
    /// <summary>
    /// Логика взаимодействия для UnitWindow.xaml
    /// </summary>
    public partial class UnitWindow : Window
    {
        public Unit Unit {  get; private set; }
        public UnitWindow(Unit unit)
        {
            InitializeComponent();
            Unit = unit;
            DataContext = Unit;
        }
        void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
