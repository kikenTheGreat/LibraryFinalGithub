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
    public partial class ArchiveBooks : Form
    {
        public ArchiveBooks()
        {
            InitializeComponent();

            LoadBooksGrid();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // If clicked column is the Restore button
            if (e.RowIndex >= 0 && DataGridTotalBooks.Columns[e.ColumnIndex].Name == "RestoreButton")
            {
                DataGridViewRow row = DataGridTotalBooks.Rows[e.RowIndex];
                RestoreBookFromArchive(row);
            }

        }


        private void RestoreBookFromArchive(DataGridViewRow row) // RESTORE METHOD
        {
            using (SqlConnection con = new SqlConnection(
     " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n"))
            {
                con.Open();

                // Insert back into BooksAcq (NO BookID because it's identity)
                SqlCommand insertCmd = new SqlCommand(@"
        INSERT INTO BooksAcq (BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category)
        VALUES (@BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category)", con);

                insertCmd.Parameters.AddWithValue("@BookTitle", row.Cells["BookTitle"].Value ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Author", row.Cells["Author"].Value ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@ISBN", row.Cells["ISBN"].Value ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Publisher", row.Cells["Publisher"].Value ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Source", row.Cells["Source"].Value ?? DBNull.Value);

                if (int.TryParse(row.Cells["Quantity"].Value?.ToString(), out int qty))
                    insertCmd.Parameters.AddWithValue("@Quantity", qty);
                else
                    insertCmd.Parameters.AddWithValue("@Quantity", DBNull.Value);

                insertCmd.Parameters.AddWithValue("@Published", row.Cells["Published"].Value ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Category", row.Cells["Category"].Value ?? DBNull.Value);

                insertCmd.ExecuteNonQuery();

                // Now delete from archive using BookID
                SqlCommand deleteCmd = new SqlCommand(
                    "DELETE FROM BooksArchive WHERE BookID = @BookID", con);
                deleteCmd.Parameters.AddWithValue("@BookID", row.Cells["BookID"].Value ?? DBNull.Value);
                deleteCmd.ExecuteNonQuery();

                MessageBox.Show("Book restored to acquisition list!");
            }


            LoadBooksGrid(); // Reload your DataGridView
        }


        private void LoadBooksGrid()          //output the datagrid 
        {
            using (SqlConnection con = new SqlConnection(" Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n"))
            {
                string query = "SELECT * FROM BooksArchive";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataGridTotalBooks.DataSource = dt;

                // Scroll to top
                if (DataGridTotalBooks.Rows.Count > 0)
                {
                    DataGridTotalBooks.FirstDisplayedScrollingRowIndex = 0;
                    DataGridTotalBooks.ClearSelection(); // Optional
                }



                if (!DataGridTotalBooks.Columns.Contains("RestoreButton"))
                {
                    DataGridViewButtonColumn restoreButton = new DataGridViewButtonColumn();
                    restoreButton.HeaderText = "Action";
                    restoreButton.Text = "Restore";
                    restoreButton.Name = "RestoreButton";
                    restoreButton.UseColumnTextForButtonValue = true;
                    DataGridTotalBooks.Columns.Add(restoreButton);
                }




            }







        }





      


        private void ArchiveBooks_Load(object sender, EventArgs e)
        {

        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.Show();
        }
    }
}
