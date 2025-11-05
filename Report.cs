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
    public partial class Report : Form
    {
        SqlConnection con = new SqlConnection(@"  Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;
");

        private string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";

        public Report()
        {
            InitializeComponent();
        }

        private void BorrowedBUTTON_Click(object sender, EventArgs e)
        {
            BorrowedPANEL.Visible = true;
            ReturnedPANEL.Visible = false;
            InventoryPANEL.Visible = false;
        }

        private void Report_Load(object sender, EventArgs e)
        {
            //live data
            lblTotalReturned.Text = GlobalEvents.GetTotalReturnedBooks().ToString();
            lblWithPenalties.Text = GlobalEvents.GetStudentsWithPenalties().ToString();
            lblTotalPenalties.Text = "₱" + GlobalEvents.GetTotalPenalties().ToString("N2");

            //load combo boxes for inventory filter
            LoadCategoryCombo();
            LoadStatusCombo();

            LoadBookStats();
            LoadReturnStats();

            // ✅ Set date range to show ALL data (e.g., last 30 days or all time)
            dtpReturnStart.Value = DateTime.Now.AddMonths(-6); // Last 6 months
            dtpReturnEnd.Value = DateTime.Now;

            dtpStart.Value = DateTime.Now;
            dtpEnd.Value = DateTime.Now;

            BorrowedPANEL.Visible = true;
            ReturnedPANEL.Visible = false;
            InventoryPANEL.Visible = false;

            LoadAllBorrowedBooks();
            LoadAllReturnedBooks();
            LoadAllBooks();

            StyleDataGrid(dgvBorrowedBooks);
            StyleDataGrid(dgvReturnedBooks);
            StyleDataGrid(dataGridBooks);

            // ✅ Setup combo boxes AFTER loading data
            cmbClientType.Items.Add("All");
            cmbClientType.Items.Add("Student");
            cmbClientType.Items.Add("Faculty");
            cmbClientType.SelectedIndex = 0;



            cmbSource.Items.Add("All");
            cmbSource.Items.Add("Purchased");
            cmbSource.Items.Add("Donate");
            cmbSource.SelectedIndex = 0;
        }

        private void ApplyReturnedBookFilters()
        {
            try
            {
                con.Open();

                string query = "SELECT * FROM ReturnedBooks WHERE ReturnDate BETWEEN @start AND @end";

                // Name filter
                if (!string.IsNullOrEmpty(txtStudentName.Text))
                {
                    query += " AND ClientName LIKE @clientName";

                }

                // Client Type filter
                if (cmbClientType.SelectedItem != null && cmbClientType.SelectedItem.ToString() != "All")
                {
                    query += " AND ClientType = @clientType";
                }

                // Overdue filter


                // Source filter
                if (cmbSource.SelectedItem != null && cmbSource.SelectedItem.ToString() != "All")
                {
                    query += " AND Source = @source";
                }

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@start", dtpReturnStart.Value.Date);
                cmd.Parameters.AddWithValue("@end", dtpReturnEnd.Value.Date);

                if (!string.IsNullOrEmpty(txtStudentName.Text))
                    cmd.Parameters.AddWithValue("@clientName", "%" + txtStudentName.Text + "%");

                if (cmbClientType.SelectedItem != null && cmbClientType.SelectedItem.ToString() != "All")
                    cmd.Parameters.AddWithValue("@clientType", cmbClientType.SelectedItem.ToString());

                if (cmbSource.SelectedItem != null && cmbSource.SelectedItem.ToString() != "All")
                    cmd.Parameters.AddWithValue("@source", cmbSource.SelectedItem.ToString());

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvReturnedBooks.DataSource = dt;
            }
            finally
            {
                con.Close();
            }
        }



        // 🧩 Load Unique Categories into ComboBox
        private void LoadCategoryCombo()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT DISTINCT Category FROM BooksAcq WHERE Category IS NOT NULL AND Category <> ''";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                cmbCategory.Items.Clear();
                while (reader.Read())
                {
                    cmbCategory.Items.Add(reader["Category"].ToString());
                }
                reader.Close();
            }
        }

        // 🧩 Load Unique Status (BookType) into ComboBox
        private void LoadStatusCombo()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT DISTINCT BookType FROM BooksAcq WHERE BookType IS NOT NULL AND BookType <> ''";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                cmbStatus.Items.Clear();
                while (reader.Read())
                {
                    cmbStatus.Items.Add(reader["BookType"].ToString());
                }
                reader.Close();
            }
        }






        // 📚 Load All Books (default)
        private void LoadAllBooks()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT BookID, BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category, BookType FROM BooksAcq";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridBooks.DataSource = dt;
            }
        }


        // 📚 BOOK INVENTORY (Already in your design)
        private void LoadBookStats()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();


                // ✅ Total Books - Sum quantities instead of counting rows
                string totalBooksQuery = "SELECT ISNULL(SUM(Quantity), 0) FROM BooksAcq";
                SqlCommand cmdTotal = new SqlCommand(totalBooksQuery, conn);
                TotalBooks.Text = cmdTotal.ExecuteScalar().ToString();

                // Available (Quantity > 5 for example)
                string availableQuery = "SELECT COUNT(*) FROM BooksAcq WHERE Quantity > 5";
                SqlCommand cmdAvailable = new SqlCommand(availableQuery, conn);
                Available.Text = cmdAvailable.ExecuteScalar().ToString();

                // Low Stock (Quantity between 1–5)
                string lowStockQuery = "SELECT COUNT(*) FROM IssueBooks WHERE Status IN ('Issued', 'Overdue', 'Report filed by librarian')";
                SqlCommand cmdLow = new SqlCommand(lowStockQuery, conn);
                Lowstack.Text = cmdLow.ExecuteScalar().ToString();

                // Archived (BookType = 'Archived')

                string archivedQuery = "SELECT ISNULL(SUM(Quantity), 0) FROM BooksArchive";
                SqlCommand cmdArchived = new SqlCommand(archivedQuery, conn);
                Archived.Text = cmdArchived.ExecuteScalar().ToString();
            }
        }

        // 🔁 RETURNED BOOKS PANEL
        private void LoadReturnStats()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Total Returned Books
                string totalReturnedQuery = "SELECT COUNT(*) FROM ReturnedBooks";
                SqlCommand cmdReturned = new SqlCommand(totalReturnedQuery, conn);
                lblTotalReturned.Text = cmdReturned.ExecuteScalar().ToString();

                // With Penalties (distinct students)
                string withPenaltiesQuery = "SELECT COUNT(DISTINCT ClientID) FROM IssueBooks WHERE Penalty > 0";
                SqlCommand cmdWithPenalties = new SqlCommand(withPenaltiesQuery, conn);
                lblWithPenalties.Text = cmdWithPenalties.ExecuteScalar().ToString();

                // Total Penalties (sum)
                string totalPenaltyQuery = "SELECT ISNULL(SUM(Penalty), 0) FROM IssueBooks";
                SqlCommand cmdTotalPenalty = new SqlCommand(totalPenaltyQuery, conn);
                decimal totalPenalty = Convert.ToDecimal(cmdTotalPenalty.ExecuteScalar());
                lblTotalPenalties.Text = "₱" + totalPenalty.ToString("N2");
            }
        }



        private void LoadAllBorrowedBooks()
        {
            try
            {
                con.Open();
                string query = "SELECT * FROM IssueBooks";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvBorrowedBooks.DataSource = dt;
            }
            finally
            {
                con.Close();
            }
        }

        private void LoadAllReturnedBooks()
        {
            try
            {
                con.Open();
                string query = "SELECT * FROM ReturnedBooks";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvReturnedBooks.DataSource = dt;
            }
            finally
            {
                con.Close();
            }
        }



        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            if (rbAllBorrowed.Checked)
            {
                LoadAllBorrowedBooks();
            }
            else if (rbOverdue.Checked)
            {
                LoadOverdueBooks();
            }
            else if (rbDateRange.Checked)
            {
                LoadBorrowedWithinDateRange();
            }
        }

        private void LoadOverdueBooks()
        {
            try
            {
                con.Open();
                string query = "SELECT * FROM IssueBooks WHERE DueDate < GETDATE()";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvBorrowedBooks.DataSource = dt;
            }
            finally
            {
                con.Close();
            }
        }

        private void LoadBorrowedWithinDateRange()
        {
            try
            {
                con.Open();
                string query = "SELECT * FROM IssueBooks WHERE IssueDate BETWEEN @start AND @end";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@start", dtpStart.Value.Date);
                cmd.Parameters.AddWithValue("@end", dtpEnd.Value.Date);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvBorrowedBooks.DataSource = dt;
            }
            finally
            {
                con.Close();
            }
        }



        private void Decoy_Click(object sender, EventArgs e)
        {
            ReturnedPANEL.Visible = true;
            BorrowedPANEL.Visible = false;
            InventoryPANEL.Visible = false;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }






        private void StyleDataGrid(DataGridView dgv) // dgv usable method for styling any datagridviewvvvvvvvvvvvvvvvvvvvvvvvvv
        {
            // 🧭 General layout
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.GridColor = Color.LightGray;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            // 🧱 Header style
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 42;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // 📘 Row style — add padding and center vertically
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // ✨ Center vertically + top & bottom padding (8px total)
            dgv.DefaultCellStyle.Padding = new Padding(5, 6, 5, 6); // left, top, right, bottom
            dgv.RowTemplate.Height = 38; // Adjust height for padding

            // 🪶 Alternating row style
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgv.AlternatingRowsDefaultCellStyle.Padding = new Padding(5, 6, 5, 6);

            if (dgv is Guna.UI2.WinForms.Guna2DataGridView gunaGrid)
            {
                gunaGrid.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(250, 250, 250);
                gunaGrid.ThemeStyle.RowsStyle.BackColor = Color.White;
                gunaGrid.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(240, 240, 240);
                gunaGrid.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                gunaGrid.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            }


        }
        private void guna2Button1_Click(object sender, EventArgs e) // button for applying filters on returned books
        {
            ApplyReturnedBookFilters();
            try
            {
                con.Open();

                // Base query - filter by date range
                string query = "SELECT * FROM ReturnedBooks WHERE ReturnDate BETWEEN @start AND @end";

                // Student name filter
                if (!string.IsNullOrEmpty(txtStudentName.Text))
                {
                    query += " AND ClientName LIKE @clientName";
                }

                // Client Type filter
                if (cmbClientType.SelectedItem != null && cmbClientType.SelectedItem.ToString() != "All")
                {
                    query += " AND ClientType = @clientType";
                }



                // Source filter
                if (cmbSource.SelectedItem != null && cmbSource.SelectedItem.ToString() != "All")
                {
                    query += " AND Source = @source";
                }

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@start", dtpReturnStart.Value.Date);
                cmd.Parameters.AddWithValue("@end", dtpReturnEnd.Value.Date);

                if (!string.IsNullOrEmpty(txtStudentName.Text))
                    cmd.Parameters.AddWithValue("@clientName", "%" + txtStudentName.Text + "%");

                if (cmbClientType.SelectedItem != null && cmbClientType.SelectedItem.ToString() != "All")
                    cmd.Parameters.AddWithValue("@clientType", cmbClientType.SelectedItem.ToString());

                if (cmbSource.SelectedItem != null && cmbSource.SelectedItem.ToString() != "All")
                    cmd.Parameters.AddWithValue("@source", cmbSource.SelectedItem.ToString());

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvReturnedBooks.DataSource = dt;
            }
            finally
            {
                con.Close();
            }
        }




        private void btnReset_Click(object sender, EventArgs e)
        {
            txtStudentName.Clear();

            dtpReturnStart.Value = DateTime.Now;
            dtpReturnEnd.Value = DateTime.Now;
            LoadAllReturnedBooks();
        }

        private void btnSearchName_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                string query;

                if (string.IsNullOrEmpty(txtStudentName.Text))
                {
                    // If the textbox is empty, show all data
                    query = "SELECT * FROM ReturnedBooks";
                }
                else
                {
                    // Search by both ClientName and ClientID
                    query = "SELECT * FROM ReturnedBooks WHERE ClientName LIKE @searchText OR CAST(ClientID AS NVARCHAR) LIKE @searchText";
                }

                SqlCommand cmd = new SqlCommand(query, con);

                if (!string.IsNullOrEmpty(txtStudentName.Text))
                {
                    cmd.Parameters.AddWithValue("@searchText", "%" + txtStudentName.Text + "%");
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvReturnedBooks.DataSource = dt;

                // Optional: show a message if no results
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No records found for that name or ID.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                con.Close();
            }
        }

        private void txtStudentName_TextChanged(object sender, EventArgs e)
        {
            btnSearchName_Click(sender, e);
        }


        public void UpdateTotalReturnsLabel()
        {

            SqlConnection con = new SqlConnection(@"  Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;

");
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ReturnedBooks", con);
                int totalarchived = (int)cmd.ExecuteScalar();
                lblTotalReturned.Text = totalarchived.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void InventoryBUTTON_Click(object sender, EventArgs e)
        {
            InventoryPANEL.Visible = true;
            ReturnedPANEL.Visible = false;
            BorrowedPANEL.Visible = false;
        }

        private void guna2CustomGradientPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void InventoryFilter_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"SELECT BookID, BookTitle, Author, ISBN, Publisher, Source, Quantity, Published, Category, BookType 
                                 FROM BooksAcq 
                                 WHERE 1=1";

                // Build dynamic filter
                if (!string.IsNullOrWhiteSpace(txtSearchBook.Text))
                    query += " AND BookTitle LIKE @BookTitle";

                if (!string.IsNullOrWhiteSpace(txtAuthor.Text))
                    query += " AND Author LIKE @Author";

                if (cmbCategory.SelectedItem != null && cmbCategory.SelectedItem.ToString() != "")
                    query += " AND Category = @Category";

                if (cmbStatus.SelectedItem != null && cmbStatus.SelectedItem.ToString() != "")
                    query += " AND BookType = @BookType";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Add parameters only if used
                    if (!string.IsNullOrWhiteSpace(txtSearchBook.Text))
                        cmd.Parameters.AddWithValue("@BookTitle", "%" + txtSearchBook.Text + "%");

                    if (!string.IsNullOrWhiteSpace(txtAuthor.Text))
                        cmd.Parameters.AddWithValue("@Author", "%" + txtAuthor.Text + "%");

                    if (cmbCategory.SelectedItem != null && cmbCategory.SelectedItem.ToString() != "")
                        cmd.Parameters.AddWithValue("@Category", cmbCategory.SelectedItem.ToString());

                    if (cmbStatus.SelectedItem != null && cmbStatus.SelectedItem.ToString() != "")
                        cmd.Parameters.AddWithValue("@BookType", cmbStatus.SelectedItem.ToString());

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridBooks.DataSource = dt;
                }
            }
        }

        private void InventoryReset_Click(object sender, EventArgs e)
        {
            txtSearchBook.Clear();
            txtAuthor.Clear();
            cmbCategory.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
            LoadAllBooks();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            report_module_4 report_Module_4 = new report_module_4();
            report_Module_4.Show();
            this.Hide();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            report_module_5 r = new report_module_5();
            r.Show();
            this.Hide();
        }

        private void cmbClientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyReturnedBookFilters();
        }

        private void cmbOverdue_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyReturnedBookFilters();
        }

        private void cmbSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyReturnedBookFilters();
        }

        private void arthanButton2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
            this.Hide();
        }
    }
}
