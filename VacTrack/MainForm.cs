using DatabaseManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VacTrack
{
    public partial class MainForm : Form
    {
        DatabaseContext DB = new();

        public MainForm()
        {
            DB.Database.EnsureCreated();
            InitializeComponent();
        }

        private void AboutProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }
    }
}
