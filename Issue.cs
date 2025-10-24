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
            LoadReturnedBooks();






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

        private void LoadReturnedBooks()
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
                SELECT 
                    ReturnID,
                    IssueID,
                    ClientID,
                    ClientName,
                    ClientType,
                    BookTitle,
                    Quantity,
                    Source,
                    IssueDate,
                    DueDate,
                    ReturnDate,
                    Status
                FROM ReturnedBooks
                ORDER BY ReturnID DESC"; // latest entries first

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // ✅ Assign data to DataGridView
                    returnDatagrid.DataSource = dt;

                    // ✅ Styling and layout (same as IssueBooks)
                    returnDatagrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                 returnDatagrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                  returnDatagrid.MultiSelect = false;
                   returnDatagrid.ReadOnly = true;
                   returnDatagrid.RowHeadersVisible = false;
                   returnDatagrid.AllowUserToResizeRows = false;
                   returnDatagrid.AllowUserToResizeColumns = false;
                    returnDatagrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    // ✅ Make it responsive inside ArthanPanel
                    returnDatagrid.Dock = DockStyle.Fill;
                }

     


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading returned books: " + ex.Message,
                                "Database Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
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
        private void SetupIssueBooksGrid()
        {
            IssueBooksDataGrid.Columns.Clear();
            IssueBooksDataGrid.AutoGenerateColumns = false;
            IssueBooksDataGrid.ReadOnly = true;
            IssueBooksDataGrid.RowHeadersVisible = false;
            IssueBooksDataGrid.BorderStyle = BorderStyle.None;
            IssueBooksDataGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            IssueBooksDataGrid.EnableHeadersVisualStyles = false;

            // --- Issue ID (hidden or visible, your choice) ---
            var colIssueID = new DataGridViewTextBoxColumn();
            colIssueID.HeaderText = "Issue ID";
            colIssueID.DataPropertyName = "IssueID";
            colIssueID.Name = "IssueID";
            colIssueID.Visible = false; // set to true if you want to see the ID
            IssueBooksDataGrid.Columns.Add(colIssueID);

            // --- Student Name ---
            var colStudentName = new DataGridViewTextBoxColumn();
            colStudentName.HeaderText = "Student Name";
            colStudentName.DataPropertyName = "StudentName";
            colStudentName.Name = "StudentName";
            colStudentName.Width = 180;
            IssueBooksDataGrid.Columns.Add(colStudentName);

            // --- Client ID ---
            var colClientID = new DataGridViewTextBoxColumn();
            colClientID.HeaderText = "Client ID";
            colClientID.DataPropertyName = "ClientID";
            colClientID.Name = "ClientID";
            colClientID.Width = 100;
            IssueBooksDataGrid.Columns.Add(colClientID);

            // --- Book Title ---
            var colBookTitle = new DataGridViewTextBoxColumn();
            colBookTitle.HeaderText = "Book Title";
            colBookTitle.DataPropertyName = "BookTitle";
            colBookTitle.Name = "BookTitle";
            colBookTitle.Width = 200;
            IssueBooksDataGrid.Columns.Add(colBookTitle);

            // --- Source ---
            var colSource = new DataGridViewTextBoxColumn();
            colSource.HeaderText = "Source";
            colSource.DataPropertyName = "Source";
            colSource.Name = "Source";
            colSource.Width = 100;
            IssueBooksDataGrid.Columns.Add(colSource);

            // --- Quantity ---
            var colQuantity = new DataGridViewTextBoxColumn();
            colQuantity.HeaderText = "Quantity";
            colQuantity.DataPropertyName = "Quantity";
            colQuantity.Name = "Quantity";
            colQuantity.Width = 80;
            IssueBooksDataGrid.Columns.Add(colQuantity);

            // --- Issue Date ---
            var colIssueDate = new DataGridViewTextBoxColumn();
            colIssueDate.HeaderText = "Issue Date";
            colIssueDate.DataPropertyName = "IssueDate";
            colIssueDate.Name = "IssueDate";
            colIssueDate.Width = 120;
            IssueBooksDataGrid.Columns.Add(colIssueDate);

            // --- Due Date ---
            var colDueDate = new DataGridViewTextBoxColumn();
            colDueDate.HeaderText = "Due Date";
            colDueDate.DataPropertyName = "DueDate";
            colDueDate.Name = "DueDate";
            colDueDate.Width = 120;
            IssueBooksDataGrid.Columns.Add(colDueDate);

            // --- Overdue Days ---
            var colOverdue = new DataGridViewTextBoxColumn();
            colOverdue.HeaderText = "Overdue Days";
            colOverdue.DataPropertyName = "OverdueDays";
            colOverdue.Name = "OverdueDays";
            colOverdue.Width = 120;
            IssueBooksDataGrid.Columns.Add(colOverdue);

            // --- Penalty ---
            var colPenalty = new DataGridViewTextBoxColumn();
            colPenalty.HeaderText = "Penalty";
            colPenalty.DataPropertyName = "Penalty";
            colPenalty.Name = "Penalty";
            colPenalty.Width = 100;
            IssueBooksDataGrid.Columns.Add(colPenalty);

            // --- Status ---
            var colStatus = new DataGridViewTextBoxColumn();
            colStatus.HeaderText = "Status";
            colStatus.DataPropertyName = "Status";
            colStatus.Name = "Status";
            colStatus.Width = 100;
            IssueBooksDataGrid.Columns.Add(colStatus);

            // --- Styling (yellow theme) ---
            IssueBooksDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            IssueBooksDataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            IssueBooksDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(253, 242, 194);
            IssueBooksDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            IssueBooksDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            IssueBooksDataGrid.DefaultCellStyle.BackColor = Color.White;
            IssueBooksDataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 250, 230);
        }


        private void Issue_Load(object sender, EventArgs e)
        {
            SetupIssueBooksGrid();
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

            //Issue and Return Panel Visibility
            panelIssueBooks.Visible = true;
            panel1IssueDataGrid.Visible = true;

            panelReturnBooks.Visible = false;
            ReturnPANEL.Visible = false;

            

           
        

            // Add multiple items at once
            BookCondition.Items.AddRange(new string[] { "Good", "Damaged", "Minor Damaged", "Lost" });
            BookCondition.SelectedIndex = 0;

            LoadReturnedBooks();

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


        private void btnConfirmBorrow_Click_1(object sender, EventArgs e)//test
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

                    string insertQuery = @"INSERT INTO IssueBooks 
   (BookID, Status, StudentName, BookTitle, Source, IssueDate, DueDate, Quantity, ClientID)
   VALUES (@BookID, @Status, @StudentName, @BookTitle, @Source, @IssueDate, @DueDate, @Quantity, @ClientID)";



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
                            cmd.Parameters.AddWithValue("@BookID", item.BookID);

                            cmd.ExecuteNonQuery();
                        }
                    }


                    MessageBox.Show("Book(s) issued successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GlobalEvents.RaiseBorrowedDataChanged();
                    GlobalEvents.RaiseOverdueDataChanged();
                    GlobalEvents.RaisePenaltiesDataChanged();




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
                LoadReturnedBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error issuing books: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LoadIssueBooks(); // refresh DataGridView
            IssueBooksDataGrid.Refresh();
            IssueBooksDataGrid.Update();
            GlobalEvents.RaiseBorrowedDataChanged();
            LoadReturnedBooks();

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
                LoadReturnedBooks();
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

        private void btnIssueBooks_Click(object sender, EventArgs e)
        {
            // Show Issue Books panel
            panelIssueBooks.Visible = true;
            panel1IssueDataGrid.Visible = true;

            // Hide Return Books panel
            panelReturnBooks.Visible = false;
            ReturnPANEL.Visible = false;



        }

        private void btnReturnBooks_Click(object sender, EventArgs e)
        {
            // Show Return Books panel
            panelReturnBooks.Visible = true;
            ReturnPANEL.Visible = true;


            // Hide Issue Books panel
            panelIssueBooks.Visible = false;
            panel1IssueDataGrid.Visible = false;
        }



        private void ReturnClientID_TextChanged(object sender, EventArgs e)
        {
            string clientID = ReturnClientID.Text.Trim();

            if (clientID.Length >= 4)
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // ✅ 1. Get Student Name from IssueBooks
                    string queryName = @"
                SELECT TOP 1 StudentName 
                FROM IssueBooks 
                WHERE ClientID = @ClientID 
                ORDER BY IssueDate DESC";

                    using (SqlCommand cmdName = new SqlCommand(queryName, con))
                    {
                        cmdName.Parameters.AddWithValue("@ClientID", clientID);
                        object result = cmdName.ExecuteScalar();

                        ReturnClientName.Items.Clear();
                        if (result != null)
                        {
                            ReturnClientName.Items.Add(result.ToString());
                            ReturnClientName.SelectedIndex = 0;
                        }
                    }

                    // ✅ 2. Get Borrowed Books Info
                    string queryBorrow = @"
                SELECT 
                    BookID,       -- ✅ Added this
                    BookTitle,
                    Status,
                    Penalty,
                    Quantity,
                    IssueID,
                    Source,
                    ClientID
                FROM IssueBooks
                WHERE ClientID = @ClientID
                  AND (Status = 'Issued' OR Status = 'Overdue')";

                    using (SqlCommand cmdBorrow = new SqlCommand(queryBorrow, con))
                    {
                        cmdBorrow.Parameters.AddWithValue("@ClientID", clientID);

                        using (SqlDataReader reader = cmdBorrow.ExecuteReader())
                        {
                            // Prepare holders
                            int bookCount = 0;
                            double totalPenalty = 0;
                            List<string> bookIDsAndTitles = new List<string>();
                            List<string> statuses = new List<string>();

                            // ✅ Clear combo boxes before filling
                            ReturnedBookID.Items.Clear();

                            while (reader.Read())
                            {
                                bookCount++;

                                // ✅ Safe read (avoid null values)
                                string bookID = reader["BookID"] != DBNull.Value ? reader["BookID"].ToString() : "N/A";
                                string bookTitle = reader["BookTitle"] != DBNull.Value ? reader["BookTitle"].ToString() : "Unknown";

                                bookIDsAndTitles.Add($"{bookID} - {bookTitle}");
                                statuses.Add(reader["Status"].ToString());

                                if (double.TryParse(reader["Penalty"].ToString(), out double penalty))
                                    totalPenalty += penalty;
                            }

                            // ✅ Fill ComboBoxes and TextBox
                            ReturnBookQty.Items.Clear();
                            ReturnBookQty.Items.Add(bookCount.ToString());
                            ReturnBookQty.SelectedIndex = 0;

                            foreach (var entry in bookIDsAndTitles)
                                ReturnedBookID.Items.Add(entry);
                            if (ReturnedBookID.Items.Count > 0)
                                ReturnedBookID.SelectedIndex = 0;

                            ReturnBookStatus.Items.Clear();
                            foreach (var status in statuses.Distinct())
                                ReturnBookStatus.Items.Add(status);
                            if (ReturnBookStatus.Items.Count > 0)
                                ReturnBookStatus.SelectedIndex = 0;

                            ReturnPenalty.Text = totalPenalty.ToString("0.00");
                        }
                    }
                }
            }
            else
            {
                // Clear all if ClientID too short
                ReturnClientName.Items.Clear();
                ReturnBookQty.Items.Clear();
                ReturnedBookID.Items.Clear();
                ReturnBookStatus.Items.Clear();
                ReturnPenalty.Clear();
            }
        }



        private void ReturnBookID_TextChanged(object sender, EventArgs e)
        {
            string bookID = ReturnedBookID.Text.Trim();

            if (bookID.Length >= 4)
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT BookTitle FROM BooksAcq WHERE BookID = @BookID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BookID", bookID);
                        con.Open();
                        var result = cmd.ExecuteScalar();

                    }
                }
            }
            else
            {

            }
        }

        private void LoadBorrowedBooksForReturn(string clientID)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                BookTitle, Source, IssueDate, DueDate, Status, Penalty 
            FROM IssueBooks
            WHERE ClientID = @ClientID 
              AND (Status = 'Issued' OR Status = 'Overdue')";

                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@ClientID", clientID);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                }
            }
        }


        private void dgvReturnList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void ReturnButton_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
    Initial Catalog=LibraryDB;
    Integrated Security=True;
    Encrypt=True;
    Trust Server Certificate=True;";

            if (ReturnedBookID.SelectedItem == null)
            {
                MessageBox.Show("Please select a book to return.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Extract values from UI
            string selectedBook = ReturnedBookID.SelectedItem.ToString();
            string bookID = selectedBook.Split('-')[0].Trim();
            string clientID = ReturnClientID.Text.Trim();
            string clientName = ReturnClientName.Text;
            string role = ReturnBookStatus.Text; // assuming this shows Role or Status
            string source = "Library";
            string bookTitle = selectedBook.Split('-').Length > 1 ? selectedBook.Split('-')[1].Trim() : "Unknown";
            int quantity = int.TryParse(ReturnBookQty.Text, out int q) ? q : 1;
            decimal penalty = decimal.TryParse(ReturnPenalty.Text, out decimal p) ? p : 0;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // 1️⃣ Get IssueID, IssueDate, and DueDate
                    int issueID = 0;
                    DateTime issueDate = DateTime.Now;
                    DateTime dueDate = DateTime.Now;

                    string getIssueQuery = @"
                SELECT TOP 1 IssueID, IssueDate, DueDate, Source
                FROM IssueBooks
                WHERE ClientID = @ClientID AND BookID = @BookID
                AND (Status = 'Issued' OR Status = 'Overdue')
                ORDER BY IssueDate DESC";

                    using (SqlCommand cmdGet = new SqlCommand(getIssueQuery, con, transaction))
                    {
                        cmdGet.Parameters.AddWithValue("@ClientID", clientID);
                        cmdGet.Parameters.AddWithValue("@BookID", bookID);

                        using (SqlDataReader reader = cmdGet.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                issueID = Convert.ToInt32(reader["IssueID"]);
                                issueDate = Convert.ToDateTime(reader["IssueDate"]);
                                dueDate = Convert.ToDateTime(reader["DueDate"]);
                                source = reader["Source"].ToString();
                            }
                        }
                    }

                    // 2️⃣ Update IssueBooks table
                    DateTime returnDate = DateTime.Now;
                    string updateIssueQuery = @"
                UPDATE IssueBooks
                SET Status = 'Returned', ReturnDate = @ReturnDate
                WHERE IssueID = @IssueID";

                    using (SqlCommand cmdUpdate = new SqlCommand(updateIssueQuery, con, transaction))
                    {
                        cmdUpdate.Parameters.AddWithValue("@ReturnDate", returnDate);
                        cmdUpdate.Parameters.AddWithValue("@IssueID", issueID);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    // 3️⃣ Insert record into ReturnedBooks table
                    string insertReturnQuery = @"
                INSERT INTO ReturnedBooks 
                    (IssueID, ClientID, ClientName, ClientType, BookTitle, Quantity, Source, IssueDate, DueDate, ReturnDate, Status)
                VALUES 
                    (@IssueID, @ClientID, @ClientName, @ClientType, @BookTitle, @Quantity, @Source, @IssueDate, @DueDate, @ReturnDate, @Status)";

                    using (SqlCommand cmdInsert = new SqlCommand(insertReturnQuery, con, transaction))
                    {
                        cmdInsert.Parameters.AddWithValue("@IssueID", issueID);
                        cmdInsert.Parameters.AddWithValue("@ClientID", clientID);
                        cmdInsert.Parameters.AddWithValue("@ClientName", clientName);
                        cmdInsert.Parameters.AddWithValue("@ClientType", role);
                        cmdInsert.Parameters.AddWithValue("@BookTitle", bookTitle);
                        cmdInsert.Parameters.AddWithValue("@Quantity", quantity);
                        cmdInsert.Parameters.AddWithValue("@Source", source);
                        cmdInsert.Parameters.AddWithValue("@IssueDate", issueDate);
                        cmdInsert.Parameters.AddWithValue("@DueDate", dueDate);
                        cmdInsert.Parameters.AddWithValue("@ReturnDate", returnDate);
                        cmdInsert.Parameters.AddWithValue("@Status", "Returned");

                        cmdInsert.ExecuteNonQuery();
                    }






                    // ✅ Commit
                    transaction.Commit();

                    // ✅ Retrieve actual saved ReturnDate from ReturnedBooks (not IssueBooks)
                    string selectQuery = @"
    SELECT TOP 1 ReturnDate
    FROM ReturnedBooks
    WHERE IssueID = @IssueID
    ORDER BY ReturnID DESC"; // ensures we get the latest return record

                    using (SqlCommand cmdSelect = new SqlCommand(selectQuery, con))
                    {
                        cmdSelect.Parameters.AddWithValue("@IssueID", issueID);
                        object result = cmdSelect.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            DateTime actualReturnDate = Convert.ToDateTime(result);
                            lblReturnDate.Text = $"Returned on: {actualReturnDate:MMMM dd, yyyy hh:mm tt}";
                        }
                        else
                        {
                            // fallback if ReturnDate not found
                            lblReturnDate.Text = $"Returned on: {DateTime.Now:MMMM dd, yyyy hh:mm tt}";
                        }
                    }

                    // ✅ Success message
                    MessageBox.Show("Book returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    GlobalEvents.RaiseBorrowedDataChanged();
                    GlobalEvents.RaiseOverdueDataChanged();
                    GlobalEvents.RaisePenaltiesDataChanged();

                    // ✅ Optional: clear other fields but keep the Return Date visible
                    ReturnClientID.Clear();
                    ReturnClientName.Items.Clear();
                    ReturnedBookID.Items.Clear();
                    ReturnBookStatus.Items.Clear();
                    ReturnPenalty.Clear();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error returning book:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            GlobalEvents.RaiseBorrowedDataChanged();
            GlobalEvents.RaiseOverdueDataChanged();
            GlobalEvents.RaisePenaltiesDataChanged();
        }


        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void arthanPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void returnDatagrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
