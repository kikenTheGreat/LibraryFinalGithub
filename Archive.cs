using Library_Final;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

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
                            GlobalEvents.RaiseBooksDataChanged();
                            GlobalEvents.RaiseArchivedDataChanged();
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
            GlobalEvents.RaiseArchivedDataChanged();

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



                DataGridTotalBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
        }

        private void arthanPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Archive_Load_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            SetupArchiveGrid();

            // 🟢 Load data from database
            LoadBooksGrid();

        }

        private void SetupArchiveGrid()
        {
            DataGridTotalBooks.Columns.Clear();
            DataGridTotalBooks.AutoGenerateColumns = false;
            DataGridTotalBooks.ReadOnly = true;
            DataGridTotalBooks.RowHeadersVisible = false;
            DataGridTotalBooks.BorderStyle = BorderStyle.None;
            DataGridTotalBooks.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            DataGridTotalBooks.EnableHeadersVisualStyles = false;

            // --- Book ID (hidden) ---
            var colBookID = new DataGridViewTextBoxColumn();
            colBookID.HeaderText = "Book ID";
            colBookID.DataPropertyName = "BookID";
            colBookID.Name = "BookID";     // 🔹 make sure name matches row.Cells["BookID"]
            colBookID.Visible = false;
            DataGridTotalBooks.Columns.Add(colBookID);

            // --- Book Title ---
            var colBookTitle = new DataGridViewTextBoxColumn();
            colBookTitle.HeaderText = "Book Title";
            colBookTitle.DataPropertyName = "BookTitle";
            colBookTitle.Name = "BookTitle";
            colBookTitle.Width = 200;
            DataGridTotalBooks.Columns.Add(colBookTitle);

            // --- Author ---
            var colAuthor = new DataGridViewTextBoxColumn();
            colAuthor.HeaderText = "Author";
            colAuthor.DataPropertyName = "Author";
            colAuthor.Name = "Author";
            colAuthor.Width = 150;
            DataGridTotalBooks.Columns.Add(colAuthor);

            // --- ISBN ---
            var colISBN = new DataGridViewTextBoxColumn();
            colISBN.HeaderText = "ISBN";
            colISBN.DataPropertyName = "ISBN";
            colISBN.Name = "ISBN";
            colISBN.Width = 120;
            DataGridTotalBooks.Columns.Add(colISBN);

            // --- Publisher ---
            var colPublisher = new DataGridViewTextBoxColumn();
            colPublisher.HeaderText = "Publisher";
            colPublisher.DataPropertyName = "Publisher";
            colPublisher.Name = "Publisher";
            colPublisher.Width = 150;
            DataGridTotalBooks.Columns.Add(colPublisher);

            // --- Source ---
            var colSource = new DataGridViewTextBoxColumn();
            colSource.HeaderText = "Source";
            colSource.DataPropertyName = "Source";
            colSource.Name = "Source";
            colSource.Width = 100;
            DataGridTotalBooks.Columns.Add(colSource);

            // --- Quantity ---
            var colQuantity = new DataGridViewTextBoxColumn();
            colQuantity.HeaderText = "Quantity";
            colQuantity.DataPropertyName = "Quantity";
            colQuantity.Name = "Quantity";
            colQuantity.Width = 80;
            DataGridTotalBooks.Columns.Add(colQuantity);

            // --- Published ---
            var colPublished = new DataGridViewTextBoxColumn();
            colPublished.HeaderText = "Published";
            colPublished.DataPropertyName = "Published";
            colPublished.Name = "Published";
            colPublished.Width = 120;
            DataGridTotalBooks.Columns.Add(colPublished);

            // --- Category ---
            var colCategory = new DataGridViewTextBoxColumn();
            colCategory.HeaderText = "Category";
            colCategory.DataPropertyName = "Category";
            colCategory.Name = "Category";
            colCategory.Width = 150;
            DataGridTotalBooks.Columns.Add(colCategory);

            // --- Archived Date ---
            var colArchivedDate = new DataGridViewTextBoxColumn();
            colArchivedDate.HeaderText = "Archived Date";
            colArchivedDate.DataPropertyName = "ArchivedDate";
            colArchivedDate.Name = "ArchivedDate";
            colArchivedDate.Width = 120;
            DataGridTotalBooks.Columns.Add(colArchivedDate);

            // --- Styling (same as Book_Acquire) ---
            DataGridTotalBooks.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridTotalBooks.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridTotalBooks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(253, 242, 194);
            DataGridTotalBooks.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            DataGridTotalBooks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            DataGridTotalBooks.DefaultCellStyle.BackColor = Color.White;
            DataGridTotalBooks.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 250, 230);
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

        private void arthanButton2_Load(object sender, EventArgs e)
        {

        }

        private void btnRestoreBook_Load(object sender, EventArgs e)
        {

        }

        private void btnRestoreBook_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtISBNRestore.Text))
            {
                MessageBox.Show("⚠️ Please enter a Book ID to restore.");
                return;
            }

            int bookId;
            if (!int.TryParse(txtISBNRestore.Text, out bookId))
            {
                MessageBox.Show("⚠️ Invalid Book ID. Please enter a number.");
                return;
            }

            RestoreBookByID(bookId);
        }


        private void RestoreBookByID(int bookId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
                {
                    con.Open();

                    // Retrieve the archived book details
                    SqlCommand selectCmd = new SqlCommand("SELECT * FROM BooksArchive WHERE BookID = @BookID", con);
                    selectCmd.Parameters.AddWithValue("@BookID", bookId);

                    SqlDataReader reader = selectCmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        MessageBox.Show("❌ Book ID not found in archive.");
                        return;
                    }

                    // Extract values safely
                    string title = reader["BookTitle"].ToString();
                    string author = reader["Author"].ToString();
                    string isbn = reader["ISBN"].ToString();
                    string publisher = reader["Publisher"].ToString();
                    string source = reader["Source"].ToString();
                    int quantity = Convert.ToInt32(reader["Quantity"]);
                    string published = reader["Published"].ToString();
                    string category = reader["Category"].ToString();

                    reader.Close();

                    // Insert into BooksAcq
                    SqlCommand insertCmd = new SqlCommand(@"
                INSERT INTO BooksAcq (BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category)
                VALUES (@BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category)", con);

                    insertCmd.Parameters.AddWithValue("@BookTitle", title);
                    insertCmd.Parameters.AddWithValue("@Author", author);
                    insertCmd.Parameters.AddWithValue("@ISBN", isbn);
                    insertCmd.Parameters.AddWithValue("@Publisher", publisher);
                    insertCmd.Parameters.AddWithValue("@Source", source);
                    insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                    insertCmd.Parameters.AddWithValue("@Published", published);
                    insertCmd.Parameters.AddWithValue("@Category", category);

                    int result = insertCmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        // Delete from archive
                        SqlCommand deleteCmd = new SqlCommand("DELETE FROM BooksArchive WHERE BookID = @BookID", con);
                        deleteCmd.Parameters.AddWithValue("@BookID", bookId);
                        deleteCmd.ExecuteNonQuery();

                        MessageBox.Show("✅ Book restored successfully!");
                        LoadBooksGrid(); // Refresh grid
                        txtISBNRestore.Clear();
                        GlobalEvents.RaiseBooksDataChanged();
                        GlobalEvents.RaiseArchivedDataChanged();
                    }
                    else
                    {
                        MessageBox.Show("⚠️ Restore failed. Please check the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restoring book: " + ex.Message);
            }
        }

        private void btnRestoreBook_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtISBNRestore.Text))
            {
                MessageBox.Show("⚠️ Please enter the ISBN to restore.");
                return;
            }

            string isbn = txtISBNRestore.Text.Trim();
            RestoreBookByISBN(isbn);
        }

        private void RestoreBookByISBN(string isbn)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
                {
                    con.Open();

                    // Find the book in the archive by ISBN
                    SqlCommand selectCmd = new SqlCommand("SELECT * FROM BooksArchive WHERE ISBN = @ISBN", con);
                    selectCmd.Parameters.AddWithValue("@ISBN", isbn);

                    SqlDataReader reader = selectCmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        MessageBox.Show("❌ No archived book found with that ISBN.");
                        return;
                    }

                    // Extract book data
                    string title = reader["BookTitle"].ToString();
                    string author = reader["Author"].ToString();
                    string publisher = reader["Publisher"].ToString();
                    string source = reader["Source"].ToString();
                    int quantity = Convert.ToInt32(reader["Quantity"]);
                    string published = reader["Published"].ToString();
                    string category = reader["Category"].ToString();
                    int bookId = Convert.ToInt32(reader["BookID"]);

                    reader.Close();

                    // Insert back into BooksAcq
                    SqlCommand insertCmd = new SqlCommand(@"
                INSERT INTO BooksAcq (BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category)
                VALUES (@BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category)", con);

                    insertCmd.Parameters.AddWithValue("@BookTitle", title);
                    insertCmd.Parameters.AddWithValue("@Author", author);
                    insertCmd.Parameters.AddWithValue("@ISBN", isbn);
                    insertCmd.Parameters.AddWithValue("@Publisher", publisher);
                    insertCmd.Parameters.AddWithValue("@Source", source);
                    insertCmd.Parameters.AddWithValue("@Quantity", quantity);
                    insertCmd.Parameters.AddWithValue("@Published", published);
                    insertCmd.Parameters.AddWithValue("@Category", category);

                    int result = insertCmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        // Delete from archive
                        SqlCommand deleteCmd = new SqlCommand("DELETE FROM BooksArchive WHERE BookID = @BookID", con);
                        deleteCmd.Parameters.AddWithValue("@BookID", bookId);
                        deleteCmd.ExecuteNonQuery();

                        MessageBox.Show("✅ Book restored successfully!");
                        txtISBNRestore.Clear();
                        LoadBooksGrid(); // Refresh DataGrid
                        GlobalEvents.RaiseBooksDataChanged();
                        GlobalEvents.RaiseArchivedDataChanged();
                    }
                    else
                    {
                        MessageBox.Show("⚠️ Restore failed. Please check the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restoring book: " + ex.Message);
            }
        }

        private void btnIssueBooks_Click(object sender, EventArgs e)
        {
     
        }

        private void btnReturnBooks_Click(object sender, EventArgs e)
        {
      
        }
    }
}
