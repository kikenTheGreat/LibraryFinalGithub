using Library_Final;
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
using System.Xml.Serialization;


namespace LibraryCGC
{


    public partial class Issue : Form
    {



        public Issue()
        {
            InitializeComponent();
            LoadIssueBooks(); // Refresh DataGridView
            SetupBorrowListGrid(); // Setup borrow list grid






        }
        private List<(string BookID, string BookTitle, string Source)> borrowList = new List<(string, string, string)>();


        private void LoadIssueBooks()
        {
            string query = @"
        SELECT 
            IssueID,
            Status,
            DueDate,
            IssueDate,
            StudentName,
            Source,
            BookTitle,
            OverdueDays,
            Penalty,
            Quantity,
            ClientID
        FROM IssueBooks
        ORDER BY IssueID DESC"; // latest entries first

            using (SqlConnection con = new SqlConnection(@"  Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;


"))

            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                IssueBooksDataGrid.DataSource = dt;

                // Clean and user-friendly appearance
                IssueBooksDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                IssueBooksDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                IssueBooksDataGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                IssueBooksDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                IssueBooksDataGrid.MultiSelect = false;
                IssueBooksDataGrid.ReadOnly = true;
                IssueBooksDataGrid.RowHeadersVisible = false;
            }

            // ✅ Auto layout and scaling
            IssueBooksDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            IssueBooksDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            IssueBooksDataGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // ✅ Responsive resizing
            IssueBooksDataGrid.Dock = DockStyle.Fill;
            // (If you have other controls in the same panel, use Anchors instead:)
            // IssueBooksDataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // 🎨 Bonus — Clean, user-friendly visual settings
            IssueBooksDataGrid.RowHeadersVisible = false;
            IssueBooksDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            IssueBooksDataGrid.MultiSelect = false;
            IssueBooksDataGrid.ReadOnly = true;
            IssueBooksDataGrid.AllowUserToResizeRows = false;
            IssueBooksDataGrid.AllowUserToResizeColumns = false;

            // Optional: center column headers
            IssueBooksDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;




        }







        private void ClientID_TextChanged(object sender, EventArgs e)
        {

            string clientID = ClientID.Text.Trim();

            if (clientID.Length >= 4)
            {
                string connectionString = "  Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";
                string query = "SELECT Name FROM AddStudentAcc WHERE ClientID = @ClientID";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", clientID);

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string name = reader["Name"].ToString();
                            ClientName.Items.Clear();
                            ClientName.Items.Add(name);
                            ClientName.SelectedIndex = 0;
                        }
                        else
                        {
                            ClientName.Items.Clear();
                        }
                    }
                }
            }
            else
            {
                // Clear ComboBox if clientID is less than 4 characters
                ClientName.Items.Clear();
            }

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Issue_Load(object sender, EventArgs e)
        {
            // Status combobox setup
            Status.Items.Add("Issued");
            Status.SelectedIndex = 0;

            // Prepare borrow list grid
            dgvBorrowList.Columns.Add("BookID", "Book ID");
            dgvBorrowList.Columns.Add("BookTitle", "Book Title");
            dgvBorrowList.Columns.Add("Source", "Source");

            overdueTimer = new System.Windows.Forms.Timer();
            overdueTimer.Interval = 1000; // every 1 sec (adjust as you like)
            overdueTimer.Tick += overdueTimer_Tick;
            overdueTimer.Start();

            UpdateTotalOverdueLabel();



        }






        private void BookID_TextChanged(object sender, EventArgs e)
        {
            string bookID = BookID.Text.Trim();

            if (bookID.Length >= 4)
            {
                string connectionString = "  Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";
                string query = "SELECT BookTitle, Source FROM BooksAcq WHERE BookID = @BookID";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BookID", bookID);

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string title = reader["BookTitle"].ToString();
                            BookTitle.Items.Clear();
                            BookTitle.Items.Add(title);
                            BookTitle.SelectedIndex = 0;
                            BookTitle.Text = title;

                            // ✅ For TextBox:
                            string src = reader["Source"].ToString();
                            Source.Text = src; // Just set the text directly
                        }
                        else
                        {
                            BookTitle.Items.Clear();
                            Source.Text = string.Empty; // clear text
                        }
                    }
                }
            }
            else
            {
                BookTitle.Items.Clear();
                Source.Text = string.Empty;
            }
        }




        private void btnAddToList_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BookID.Text) || string.IsNullOrWhiteSpace(BookTitle.Text))
            {
                MessageBox.Show("Please select a valid book.");
                return;
            }

            // ✅ Add Source along with BookID and BookTitle
            borrowList.Add((BookID.Text, BookTitle.Text, Source.Text));

            // ✅ Add Source as a column in the DataGridView
            if (dgvBorrowList.Columns.Count < 3)
            {
                dgvBorrowList.Columns.Clear();
                dgvBorrowList.Columns.Add("BookID", "Book ID");
                dgvBorrowList.Columns.Add("BookTitle", "Book Title");
                dgvBorrowList.Columns.Add("Source", "Source");
            }

            //remove later yah
            MessageBox.Show($"Borrow list contains {borrowList.Count} books.");


            dgvBorrowList.Rows.Add(BookID.Text, BookTitle.Text, Source.Text);

            // Clear fields for next entry
            BookID.Clear();
            BookTitle.Items.Clear();
            BookTitle.Text = "";
            Source.Text = "";
        }


        private void btnConfirmBorrow_Click_1(object sender, EventArgs e)
        {
            if (borrowList.Count == 0)
            {
                MessageBox.Show("No books selected to borrow.");
                return;
            }

            // Fix for DateTimePicker values
            DateTime issueDateValue = issueDate.Value;
            DateTime dueDateValue = dueDate.Value;

            try
            {
                using (SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;


"))

                {
                    con.Open();

                    // ✅ STEP 1: Check if the user already has active borrowed books (limit 3)
                    string checkQuery = @"SELECT COUNT(*) FROM IssueBooks 
                                  WHERE ClientID = @ClientID AND (Status = 'Issued' OR Status = 'Overdue')";
                    using (SqlCommand cmdCheck = new SqlCommand(checkQuery, con))
                    {
                        cmdCheck.Parameters.AddWithValue("@ClientID", ClientID.Text);
                        int currentBorrowed = (int)cmdCheck.ExecuteScalar();

                        int totalAfterBorrow = currentBorrowed + borrowList.Count;
                        if (totalAfterBorrow > 3)
                        {
                            MessageBox.Show("Borrow limit exceeded! Each user can only borrow up to 3 books at a time.",
                                "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // ✅ STEP 2: Insert each selected book as its own row in IssueBooks
                    string insertQuery = @"INSERT INTO IssueBooks 
                                   (Status, StudentName, BookTitle, Source, IssueDate, DueDate, Quantity, ClientID)
                                   VALUES (@Status, @StudentName, @BookTitle, @Source, @IssueDate, @DueDate, @Quantity, @ClientID)";

                    foreach (var item in borrowList)
                    {
                        using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@Status", "Issued"); // fixed
                            cmd.Parameters.AddWithValue("@StudentName", ClientName.Text);
                            cmd.Parameters.AddWithValue("@BookTitle", item.BookTitle);
                            cmd.Parameters.AddWithValue("@Source", item.Source);
                            cmd.Parameters.AddWithValue("@IssueDate", issueDateValue);
                            cmd.Parameters.AddWithValue("@DueDate", dueDateValue);
                            cmd.Parameters.AddWithValue("@Quantity", 1); // each book = 1 copy
                            cmd.Parameters.AddWithValue("@ClientID", ClientID.Text);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Book(s) issued successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // ✅ STEP 3: Refresh and clear borrow list
                borrowList.Clear();
                dgvBorrowList.Rows.Clear();

                var dashboardForm = Application.OpenForms["Form1"] as Form1;
                if (dashboardForm != null)
                {
                    dashboardForm.UpdateTotalBorrowedLabel();
                }

                LoadIssueBooks(); // refresh DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error issuing books: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LoadIssueBooks(); // refresh DataGridView
            IssueBooksDataGrid.Refresh();
            IssueBooksDataGrid.Update();


        }





        private void IssueBooksDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvBorrowList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


        }

        private void SetupBorrowListGrid()
        {
            // Fit columns proportionally to the grid width
            dgvBorrowList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Allow wrapping if text is long
            dgvBorrowList.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Optional neat settings
            dgvBorrowList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBorrowList.MultiSelect = false;
            dgvBorrowList.ReadOnly = true;
            dgvBorrowList.RowHeadersVisible = false;
            dgvBorrowList.AllowUserToResizeRows = false;
            dgvBorrowList.AllowUserToResizeColumns = false;
            dgvBorrowList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }




        private void arthanButton1_Click(object sender, EventArgs e)
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

        private void arthanButton1_Load(object sender, EventArgs e)
        {

        }

        private void issueDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void overdueTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(
                    @"Data Source=(LocalDB)\MSSQLLocalDB;
            Initial Catalog=LibraryDB;
            Integrated Security=True;
            Encrypt=True;
            Trust Server Certificate=True;"))
                {
                    con.Open();

                    string query = @"
                UPDATE IssueBooks
                SET 
                    OverdueDays = DATEDIFF(DAY, DueDate, GETDATE()),
                    Penalty = CASE 
                                WHEN DATEDIFF(DAY, DueDate, GETDATE()) > 0 
                                THEN DATEDIFF(DAY, DueDate, GETDATE()) * 5 
                                ELSE 0 
                              END,
                    Status = CASE 
                                WHEN DATEDIFF(DAY, DueDate, GETDATE()) > 0 THEN 'Overdue'
                                ELSE 'Issued'
                             END
                WHERE 
                    (Status = 'Issued' OR Status = 'Overdue')  -- ✅ don't touch Returned
                    AND GETDATE() >= IssueDate;
            ";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadIssueBooks();
                UpdateTotalOverdueLabel();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating overdue penalties: " + ex.Message);
            }
        }


        public void UpdateTotalOverdueLabel()
        {
            string connectionString = @" Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;


";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // ✅ Only count rows where Status = 'Overdue'
                    // and OverdueDays > 0 or Penalty > 0
                    string query = @"
    SELECT COUNT(*) 
    FROM IssueBooks
    WHERE 
        Status = 'Overdue'
        AND (OverdueDays IS NOT NULL AND OverdueDays > 0)
";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        int totalOverdue = (int)cmd.ExecuteScalar();

                        // ✅ If no overdue books, display 0
                        lblOverdueCount.Text = totalOverdue > 0
                            ? $"Overdue Books: {totalOverdue}"
                            : "Overdue Books: 0";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error counting overdue books: " + ex.Message);
                }
            }
        }




        private void arthanPanel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblOverdueCount_Click(object sender, EventArgs e)
        {

        }
    }
}
