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
            colBookID.HeaderText = "Accession Number";
            colBookID.DataPropertyName = "ArchiveID";
            colBookID.Name = "ArchiveID";     // 🔹 make sure name matches row.Cells["BookID"]

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

            // --- Book Type ---
            var colBookType = new DataGridViewTextBoxColumn();
            colBookType.HeaderText = "Book Type";
            colBookType.DataPropertyName = "BookType";
            colBookType.Name = "BookType";
            colBookType.Width = 130;
            DataGridTotalBooks.Columns.Add(colBookType);

            // --- Book Condition ---
            var bookCondition = new DataGridViewTextBoxColumn();
            bookCondition.HeaderText = "Book Condition";
            bookCondition.DataPropertyName = "BookCondition";
            bookCondition.Name = "Book Condition";
            bookCondition.Width = 130;
            DataGridTotalBooks.Columns.Add(bookCondition);


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

            Form1 form1 = new Form1(SessionData.CurrentEmployeeID);
            form1.Show();
            this.Hide();
        }




        private void arthanButton2_Load(object sender, EventArgs e)
        {

        }

        private void btnRestoreBook_Load(object sender, EventArgs e)
        {

        }

        private void btnRestoreBook_Click_1(object sender, EventArgs e)
        {

        }



        private void btnIssueBooks_Click(object sender, EventArgs e)
        {

        }

        private void btnReturnBooks_Click(object sender, EventArgs e)
        {

        }

        private void btnRestoreBook_Click(object sender, EventArgs e)
        {
            string isbn = txtArchiveISBN.Text.Trim();
            int restoreQty = (int)RestoreQty.Value; // 🔹 Add RestoreQty NumericUpDown control to your form

            if (string.IsNullOrEmpty(isbn))
            {
                MessageBox.Show("⚠️ Please enter an ISBN.", "Missing ISBN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (restoreQty <= 0)
            {
                MessageBox.Show("⚠️ Please enter a valid quantity to restore.", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
                {
                    con.Open();

                    // 1️⃣ Find the book in BooksArchive
                    string selectQuery = "SELECT * FROM BooksArchive WHERE ISBN = @ISBN";
                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, con))
                    {
                        selectCmd.Parameters.AddWithValue("@ISBN", isbn);
                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("❌ No archived book found with that ISBN.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            int archiveId = Convert.ToInt32(reader["ArchiveID"]);
                            string title = reader["BookTitle"]?.ToString() ?? "";
                            string author = reader["Author"]?.ToString() ?? "";
                            string publisher = reader["Publisher"]?.ToString() ?? "";
                            string source = reader["Source"]?.ToString() ?? "";
                            int currentArchivedQty = reader["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Quantity"]);
                            string published = reader["Published"]?.ToString() ?? "";
                            string category = reader["Category"]?.ToString() ?? "";
                            string bookType = reader["BookType"]?.ToString() ?? "Book";
                            string bookCondition = reader["BookCondition"]?.ToString() ?? "Good";

                            reader.Close();

                            // 2️⃣ Validate restore quantity
                            if (restoreQty > currentArchivedQty)
                            {
                                MessageBox.Show($"⚠️ Cannot restore {restoreQty} copies. Only {currentArchivedQty} archived.",
                                    "Insufficient Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 3️⃣ Confirm action
                            var confirm = MessageBox.Show(
                                $"Restore {restoreQty} of '{title}' back to active inventory?\n\n" +
                                $"Archived quantity: {currentArchivedQty}\n" +
                                $"Remaining in archive: {currentArchivedQty - restoreQty}",
                                "Confirm Restore",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (confirm != DialogResult.Yes)
                                return;

                            // 4️⃣ Check if book already exists in BooksAcq
                            string checkExistingQuery = "SELECT Quantity FROM BooksAcq WHERE ISBN = @ISBN";
                            using (SqlCommand checkCmd = new SqlCommand(checkExistingQuery, con))
                            {
                                checkCmd.Parameters.AddWithValue("@ISBN", isbn);
                                object existingQtyObj = checkCmd.ExecuteScalar();

                                if (existingQtyObj != null)
                                {
                                    // ✅ Book exists - just update quantity
                                    int existingQty = Convert.ToInt32(existingQtyObj);
                                    int newQty = existingQty + restoreQty;

                                    SqlCommand updateCmd = new SqlCommand(
                                        "UPDATE BooksAcq SET Quantity = @Quantity WHERE ISBN = @ISBN", con);
                                    updateCmd.Parameters.AddWithValue("@Quantity", newQty);
                                    updateCmd.Parameters.AddWithValue("@ISBN", isbn);
                                    updateCmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    // 🆕 Book doesn't exist - insert new record
                                    SqlCommand insertCmd = new SqlCommand(@"
                                    INSERT INTO BooksAcq 
                                    (BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category, BookType, BookCondition)
                                    VALUES 
                                    (@BookTitle, @Author, @ISBN, @Publisher, @Source, @Quantity, @Published, @Category, @BookType, @BookCondition)", con);

                                    insertCmd.Parameters.AddWithValue("@BookTitle", title);
                                    insertCmd.Parameters.AddWithValue("@Author", author);
                                    insertCmd.Parameters.AddWithValue("@ISBN", isbn);
                                    insertCmd.Parameters.AddWithValue("@Publisher", publisher);
                                    insertCmd.Parameters.AddWithValue("@Source", source);
                                    insertCmd.Parameters.AddWithValue("@Quantity", restoreQty);
                                    insertCmd.Parameters.AddWithValue("@Published", published);
                                    insertCmd.Parameters.AddWithValue("@Category", category);
                                    insertCmd.Parameters.AddWithValue("@BookType", bookType);
                                    insertCmd.Parameters.AddWithValue("@BookCondition", bookCondition);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }

                            // 5️⃣ Update or delete from BooksArchive
                            if (restoreQty < currentArchivedQty)
                            {
                                // Partial restore - update archived quantity
                                int remainingArchived = currentArchivedQty - restoreQty;
                                SqlCommand updateArchiveCmd = new SqlCommand(
                                    "UPDATE BooksArchive SET Quantity = @Quantity WHERE ArchiveID = @ArchiveID", con);
                                updateArchiveCmd.Parameters.AddWithValue("@Quantity", remainingArchived);
                                updateArchiveCmd.Parameters.AddWithValue("@ArchiveID", archiveId);
                                updateArchiveCmd.ExecuteNonQuery();

                                MessageBox.Show($"✅ Restored {restoreQty} copies of '{title}'!\n\nRemaining in archive: {remainingArchived}",
                                    "Restore Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                // Full restore - delete from archive
                                SqlCommand deleteCmd = new SqlCommand(
                                    "DELETE FROM BooksArchive WHERE ArchiveID = @ArchiveID", con);
                                deleteCmd.Parameters.AddWithValue("@ArchiveID", archiveId);
                                deleteCmd.ExecuteNonQuery();

                                MessageBox.Show($"✅ Restored all {restoreQty} copies of '{title}'!\n\nBook removed from archive.",
                                    "Restore Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            // 6️⃣ Activity Log + Refresh
                            ActivityLog.RecordActivity(
                                SessionData.CurrentUserName,
                                "Restore Book",
                                "Archived Books",
                                $"Restored {restoreQty} copies of '{title}' (ISBN: {isbn})");

                            txtArchiveISBN.Clear();
                            RestoreQty.Value = 1;
                            LoadBooksGrid();
                            GlobalEvents.RaiseBooksDataChanged();
                            GlobalEvents.RaiseArchivedDataChanged();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error restoring book: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtArchiveISBN_TextChanged(object sender, EventArgs e)
        {
            string isbn = txtArchiveISBN.Text.Trim();

            // Only search when ISBN length > 10 (means user is done typing)
            if (isbn.Length > 10)
            {
                bool found = false;

                // Loop through DataGridView rows
                foreach (DataGridViewRow row in DataGridTotalBooks.Rows)
                {
                    if (row.Cells["ISBN"].Value != null &&
                        row.Cells["ISBN"].Value.ToString().Equals(isbn, StringComparison.OrdinalIgnoreCase))
                    {
                        // Highlight found row
                        row.Selected = true;
                        DataGridTotalBooks.CurrentCell = row.Cells[0];

                        // Optionally scroll to it
                        DataGridTotalBooks.FirstDisplayedScrollingRowIndex = row.Index;

                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    MessageBox.Show("❌ Book not found in the archive.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
