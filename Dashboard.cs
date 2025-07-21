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
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void kryptonPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            BookAcq bookAcq = new BookAcq();
            bookAcq.Show();
        }

        private void kryptonLabel3_Click(object sender, EventArgs e)
        {

        }

        private void kryptonButton5_Click(object sender, EventArgs e)
        {
            ManageBooks manageBooks = new ManageBooks();
            manageBooks.Show();
        }

        private void kryptonButton7_Click(object sender, EventArgs e)
        {
            BorrowBooks borrowBooks = new BorrowBooks();
            borrowBooks.Show();
        }

        private void kryptonButton6_Click(object sender, EventArgs e)
        {
            Return @return = new Return();
            @return.Show();
        }
    }
}
