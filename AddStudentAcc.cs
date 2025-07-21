using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Final
{
    public partial class AddStudentAcc : Form
    {
        public AddStudentAcc()
        {
            InitializeComponent();
        }

        private void kryptonLabel1_Click(object sender, EventArgs e)
        {

        }

        private void AddStudentAcc_Load(object sender, EventArgs e)
        {

        }

        private void kryptonButton5_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.ShowDialog();
            this.Close();
        }
    }
}
