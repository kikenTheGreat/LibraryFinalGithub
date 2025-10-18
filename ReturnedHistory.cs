using LibraryCGC;
using Microsoft.Data.SqlClient;
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
    public partial class ReturnedHistory : Form
    {
        public ReturnedHistory()
        {
            InitializeComponent();
            LoadBooksGrid();
        }




        private void LoadBooksGrid()          //output the datagrid 
        {
            using (SqlConnection con = new SqlConnection(" Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n"))
            {
                string query = "SELECT * FROM ReturnedBooks";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataGridReturnBooks.DataSource = dt;

                // Scroll to top
                if (DataGridReturnBooks.Rows.Count > 0)
                {
                    DataGridReturnBooks.FirstDisplayedScrollingRowIndex = 0;
                    DataGridReturnBooks.ClearSelection(); // Optional
                }



                DataGridReturnBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
        }

        private void DataGridReturnBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void arthanButton2_Click(object sender, EventArgs e)
        {
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is Form1)
                {
                    openForm.Show();
                    this.Hide();
                    return;
                }
            }

            // If not open, create it
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void arthanButton2_Load(object sender, EventArgs e)
        {

        }

        private void ReturnedHistory_Load(object sender, EventArgs e)
        {

        }
    }
}
