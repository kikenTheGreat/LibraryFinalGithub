using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace LibraryCGC
{
    public partial class Archive : Form
    {
        public Archive()
        {
            InitializeComponent();
            LoadBooksGrid();
            var dashboardForm = Application.OpenForms["Form1"] as Form1;
            if (dashboardForm != null)
            {
                dashboardForm.UpdateTotalArchivedLabel();
            }
        }

        private void Archive_Load(object sender, EventArgs e)
        {
            LoadBooksGrid();              // load the grid data
            DataGridTotalBooks.BringToFront();  // make buttons clickable
        }

        private void pnlActiveBooks_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlArchivedBooks_Paint(object sender, PaintEventArgs e)
        {

        }

        private void arthanButton1_Load(object sender, EventArgs e)
        {

        }

        private void arthanButton1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            this.Hide();

        }

        private void DataGridTotalBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // If clicked column is the Restore button
            if (e.RowIndex >= 0 && DataGridTotalBooks.Columns[e.ColumnIndex].Name == "RestoreButton")
            {
                DataGridViewRow row = DataGridTotalBooks.Rows[e.RowIndex];
                RestoreBookFromArchive(row);
            }
        }


        private void RestoreBookFromArchive(DataGridViewRow row)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(
                    "  Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n"))
                {
                    con.Open();



                    // Insert into BooksAcq
                    using (SqlCommand insertCmd = new SqlCommand(@"
                INSERT INTO BooksAcq (BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category)
                VALUES (@BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category)", con))
                    {
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

                        int result = insertCmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            // Delete from archive
                            using (SqlCommand deleteCmd = new SqlCommand("DELETE FROM BooksArchive WHERE BookID = @BookID", con))
                            {
                                deleteCmd.Parameters.AddWithValue("@BookID", row.Cells["BookID"].Value ?? DBNull.Value);
                                deleteCmd.ExecuteNonQuery();
                            }

                            MessageBox.Show("✅ Book restored successfully!");
                            LoadBooksGrid(); // Refresh grid only after success
                        }
                        else
                        {
                            MessageBox.Show("⚠️ No rows inserted. Check your database columns.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restoring book: " + ex.Message);
            }
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

                DataGridTotalBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
        }

        private void arthanPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Archive_Load_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

        }

        private void arthanPanel9_Paint(object sender, PaintEventArgs e)
        {

        }


        // to delete nowwww



        private void flowLayoutPanelPenalties_Paint(object sender, PaintEventArgs e)
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
    }
}
