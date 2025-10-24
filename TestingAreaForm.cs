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
    public partial class TestingAreaForm : Form
    {
        public TestingAreaForm()
        {
            InitializeComponent();
        }

        private void btnIssueBooks_Click(object sender, EventArgs e)
        {
            // Show Issue Books panel
            panelIssueBooks.Visible = true;

            // Hide Return Books panel
            panelReturnBooks.Visible = false;
        }

        private void btnReturnBooks_Click(object sender, EventArgs e)
        {
            // Show Issue Books panel
            panelIssueBooks.Visible = false;

            // Hide Return Books panel
            panelReturnBooks.Visible = true;
        }

        private void TestingAreaForm_Load(object sender, EventArgs e)
        {
            // Show Issue Books panel
            panelIssueBooks.Visible = true;

            // Hide Return Books panel
            panelReturnBooks.Visible = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }
    }
}
