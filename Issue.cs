using Library_Final;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace LibraryCGC
{


    public partial class Issue : Form
    {
        private int currentEmployeeID;


        public Issue()
        {
            InitializeComponent();
            LoadIssueBooks(); // Refresh DataGridView
            SetupBorrowListGrid(); // Setup borrow list grid
            LoadReturnedBooks();
        }
        private List<(string ISBN, string BookTitle, string Source, string BookCondition)> borrowList = new List<(string, string, string, string)>();

        public Issue(int employeeId)
        {
            InitializeComponent();
            currentEmployeeID = employeeId;
        }



        // Replace your LoadIssueBooks() method in Issue.cs with this updated version:

        // AFTER (public - accessible from other forms):
        // Replace the LoadIssueBooks() method in Issue.cs with this corrected version:

        public void LoadIssueBooks()
        {
            // 🟡 Save current scroll position (if any)
            int firstDisplayedRow = 0;
            if (IssueBooksDataGrid.FirstDisplayedScrollingRowIndex >= 0)
                firstDisplayedRow = IssueBooksDataGrid.FirstDisplayedScrollingRowIndex;

            // ✅ Query that checks BOTH AddStudentAcc AND InactiveStudents tables
            string query = @"
        SELECT 
            i.IssueID,
            i.Status,
            i.DueDate,
            i.IssueDate,
            i.StudentName,
            i.Source,
            i.BookTitle,
            i.OverdueDays,
            i.Penalty,
            i.Quantity,
            i.ClientID
        FROM IssueBooks i
        WHERE 
            -- Show if student is active in AddStudentAcc
            EXISTS (SELECT 1 FROM AddStudentAcc a WHERE a.ClientID = i.ClientID AND a.Status = 'Active')
            OR
            -- OR show if they have penalties/issues (regardless of where they are)
            (i.Penalty > 0 OR i.Status = 'Overdue' OR i.Status = 'Report filed by librarian')
        ORDER BY i.IssueID DESC";

            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;"))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                IssueBooksDataGrid.DataSource = dt;



                HighlightOverdueRows();
                HighlightStatusRows();

               
            }

            // 🟢 Restore scroll position (if valid)
            if (firstDisplayedRow >= 0 && firstDisplayedRow < IssueBooksDataGrid.RowCount)
                IssueBooksDataGrid.FirstDisplayedScrollingRowIndex = firstDisplayedRow;

            IssueBooksDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            IssueBooksDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            IssueBooksDataGrid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            IssueBooksDataGrid.Dock = DockStyle.Fill;
            IssueBooksDataGrid.RowHeadersVisible = false;
            IssueBooksDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            IssueBooksDataGrid.MultiSelect = false;
            IssueBooksDataGrid.ReadOnly = true;
            IssueBooksDataGrid.AllowUserToResizeRows = false;
            IssueBooksDataGrid.AllowUserToResizeColumns = false;
            IssueBooksDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewRow row in IssueBooksDataGrid.Rows)
            {
                if (row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();
                    if (status.Equals("Overdue", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                        row.DefaultCellStyle.ForeColor = Color.White;
                    }
                }
            }
        }

        public void LoadReturnedBooks()
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
                ORDER BY ReturnID DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    returnDatagrid.DataSource = dt;

                    returnDatagrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    returnDatagrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    returnDatagrid.MultiSelect = false;
                    returnDatagrid.ReadOnly = true;
                    returnDatagrid.RowHeadersVisible = false;
                    returnDatagrid.AllowUserToResizeRows = false;
                    returnDatagrid.AllowUserToResizeColumns = false;
                    returnDatagrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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







        private void HighlightOverdueRows()
        {
            foreach (DataGridViewRow row in IssueBooksDataGrid.Rows)
            {
                if (row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();
                    if (status.Equals("Overdue", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral; // red
                        row.DefaultCellStyle.ForeColor = Color.White;
                    }
                    else if (status.Equals("Returned", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen; // green
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.White; // default
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void HighlightStatusRows()
        {
            foreach (DataGridViewRow row in IssueBooksDataGrid.Rows)
            {
                if (row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();

                    if (status.Equals("Overdue", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral; // 🔴 red
                        row.DefaultCellStyle.ForeColor = Color.White;
                    }
                    else if (status.Equals("Returned", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.LightBlue; // 🔵 blue
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else if (status.Equals("Report filed by librarian", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGreen; // 🟢 green
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else if (status.Equals("Lost", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.LightGray; // ⚫ gray
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        // default
                        row.DefaultCellStyle.BackColor = Color.White;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                    }
                }
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

            MoveReturnedBooks();

            // populate librarian's selectable condition list
            returnCondition.Items.Clear();
            returnCondition.Items.AddRange(new string[]
            {
    "Good",
    "Minor Damaged",
    "Damaged",
    "Lost"
            });

            returnCondition.SelectedIndex = 0;

            // Prepare borrow list grid
            dgvBorrowList.Columns.Add("ISBN", "ISBN");

            dgvBorrowList.Columns.Add("BookTitle", "Book Title");
            dgvBorrowList.Columns.Add("Source", "Source");

            overdueTimer = new System.Windows.Forms.Timer();
            overdueTimer.Interval = 1000; // every 1 sec (adjust as you like)
            overdueTimer.Tick += overdueTimer_Tick;
            overdueTimer.Start();
            HighlightOverdueRows();
            HighlightStatusRows();

            overdueTimer = new System.Windows.Forms.Timer();
            overdueTimer.Interval = 1000; // every 1 sec (adjust as you like)
            overdueTimer.Tick += overdueTimer_Tick;
            overdueTimer.Start();
            StyleDataGrid(returnDatagrid);

            UpdateTotalOverdueLabel();

            //Issue and Return Panel Visibility
            panelIssueBooks.Visible = true;
            panel1IssueDataGrid.Visible = true;
            PANELdataList.Visible = true;
            PANELoverdue.Visible = true;

            panelReturnBooks.Visible = false;
            ReturnPANEL.Visible = false;









            LoadReturnedBooks();

            issueDate.Value = DateTime.Now;
            issueDate.Format = DateTimePickerFormat.Custom;
            issueDate.CustomFormat = "dddd, MMMM dd, yyyy"; // Example: Friday, October 25, 2025

            StartDateTimeUpdater(); // ✅ Live update the Issue Date picker


        }



        private void StartDateTimeUpdater()
        {
            // ✅ Just format the DateTimePickers (no timer needed)
            issueDate.Format = DateTimePickerFormat.Custom;
            issueDate.CustomFormat = "dddd, MMMM dd, yyyy hh:mm tt";

            dueDate.Format = DateTimePickerFormat.Custom;
            dueDate.CustomFormat = "dddd, MMMM dd, yyyy hh:mm tt";

            // Set initial issue date to now
            issueDate.Value = DateTime.Now;
        }











        private void BookID_TextChanged(object sender, EventArgs e)
        {
            string bookID = ISBN.Text.Trim();

            if (bookID.Length >= 4)
            {
                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;";
                string query = "SELECT BookTitle, Source, BookCondition FROM BooksAcq WHERE BookID = @BookID";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@BookID", bookID);

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            // ✅ Book Title
                            string title = reader["BookTitle"].ToString();
                            BookTitle.Items.Clear();
                            BookTitle.Items.Add(title);
                            BookTitle.SelectedIndex = 0;
                            BookTitle.Text = title;

                            // ✅ Source
                            string src = reader["Source"].ToString();
                            Source.Text = src;

                            // ✅ Book Condition (new ComboBox)
                            string condition = reader["BookCondition"].ToString();
                            issuedCondition.Items.Clear();
                            issuedCondition.Items.Add(condition);
                            issuedCondition.SelectedIndex = 0;
                            issuedCondition.Text = condition;
                        }
                        else
                        {
                            // Clear fields if no record found
                            BookTitle.Items.Clear();
                            Source.Text = string.Empty;
                            issuedCondition.Items.Clear();
                        }
                    }
                }
            }
            else
            {
                // Clear fields if input too short
                BookTitle.Items.Clear();
                Source.Text = string.Empty;
                issuedCondition.Items.Clear();
            }
        }







        private void btnAddToList_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ISBN.Text) || string.IsNullOrWhiteSpace(BookTitle.Text))
            {
                MessageBox.Show("Please select a valid book.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ClientID.Text))
            {
                MessageBox.Show("Please enter a valid Client ID.");
                return;
            }

            // ✅ Local duplicate check (in-memory)
            bool alreadyExists = borrowList.Any(item => item.Item1 == ISBN.Text);
            if (alreadyExists)
            {
                MessageBox.Show("This book (same ISBN) is already in the borrow list.",
                    "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ Database duplicate check
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"; // ← Replace with your actual connection string
            string query = @"
    SELECT COUNT(*) 
    FROM IssueBooks 
    WHERE ClientID = @ClientID 
      AND ISBN = @ISBN 
      AND (Status = 'Issued' OR Status = 'Report filed by librarian')";


            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ClientID", ClientID.Text.Trim());
                cmd.Parameters.AddWithValue("@ISBN", ISBN.Text.Trim());

                conn.Open();
                int existingCount = (int)cmd.ExecuteScalar();
                conn.Close();

                if (existingCount > 0)
                {
                    MessageBox.Show("This client has already borrowed this book and it is still marked as 'Issued' or Report filed by librarian.",
                        "Already Borrowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ClearField();

                    return;
                }
            }

            // ✅ Add to borrow list
            borrowList.Add((ISBN.Text, BookTitle.Text, Source.Text, issuedCondition.Text));

            // ✅ Setup DataGridView columns (only once)
            if (dgvBorrowList.Columns.Count < 4)
            {
                dgvBorrowList.Columns.Clear();
                dgvBorrowList.Columns.Add("ISBN", "ISBN");
                dgvBorrowList.Columns.Add("BookTitle", "Book Title");
                dgvBorrowList.Columns.Add("Source", "Source");

                DataGridViewButtonColumn removeButton = new DataGridViewButtonColumn();
                removeButton.Name = "Remove";
                removeButton.HeaderText = "Action";
                removeButton.Text = "Remove";
                removeButton.UseColumnTextForButtonValue = true;
                removeButton.FlatStyle = FlatStyle.Flat;
                removeButton.DefaultCellStyle.BackColor = Color.OrangeRed;
                removeButton.DefaultCellStyle.ForeColor = Color.White;
                removeButton.Width = 90;
                dgvBorrowList.Columns.Add(removeButton);
            }

            dgvBorrowList.Rows.Add(ISBN.Text, BookTitle.Text, Source.Text);

            ISBN.Focus();

            // Clear fields
            ISBN.Clear();
            BookTitle.Items.Clear();
            BookTitle.Text = "";
            Source.Text = "";
        }



        private void ClearField()
        {
            ISBN.Clear();
            BookTitle.Items.Clear();
            BookTitle.Text = "";
            Source.Text = "";
        }
        // ===== FOR Issue.cs - btnConfirmBorrow_Click_1 =====
        // Replace your existing btnConfirmBorrow_Click_1 method with this updated version:

        private void btnConfirmBorrow_Click_1(object sender, EventArgs e)
        {
            if (borrowList.Count == 0)
            {
                MessageBox.Show("No books selected to borrow.");
                return;
            }

            DateTime issueDateValue = issueDate.Value;
            string clientType = GetClientType(ClientID.Text.Trim());
            DateTime dueDateValue = ComputeDueDate(clientType, issueDateValue);

            try
            {
                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;"))
                {
                    con.Open();

                    // ✅ NEW: Check if student has pending penalties from previous semester
                    string checkPenaltiesQuery = @"
    SELECT COUNT(*), ISNULL(SUM(PenaltyAmount), 0)
    FROM PendingPenalties 
    WHERE ClientID = @ClientID AND IsPaid = 0";


                    // ✅ Check if student has "With Pending Issues" status
                    string checkStatusQuery = "SELECT Status FROM AddStudentAcc WHERE ClientID = @ClientID";
                    using (SqlCommand cmdStatus = new SqlCommand(checkStatusQuery, con))
                    {
                        cmdStatus.Parameters.AddWithValue("@ClientID", ClientID.Text.Trim());
                        object statusResult = cmdStatus.ExecuteScalar();

                        if (statusResult != null && statusResult.ToString() == "With Pending Issues")
                        {
                            MessageBox.Show(
                                "⚠️ BORROWING BLOCKED\n\n" +
                                "This student has pending issues from the previous semester.\n\n" +
                                "The student must resolve all issues before borrowing books.\n" +
                                "Please direct them to the library desk to settle their penalties.",
                                "Pending Issues",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return;
                        }
                    }

                    // Continue with existing penalty check...

                    using (SqlCommand cmdPenalty = new SqlCommand(checkPenaltiesQuery, con))
                    {
                        cmdPenalty.Parameters.AddWithValue("@ClientID", ClientID.Text.Trim());

                        using (SqlDataReader reader = cmdPenalty.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int penaltyCount = reader.GetInt32(0);
                                decimal totalPenalty = reader.GetDecimal(1);

                                if (penaltyCount > 0)
                                {
                                    MessageBox.Show(
                                        $"⚠️ BORROWING BLOCKED\n\n" +
                                        $"This student has {penaltyCount} pending penalty/issue(s) from the previous semester.\n\n" +
                                        $"Total Amount Due: ₱{totalPenalty:N2}\n\n" +
                                        $"The student must settle all penalties before borrowing books.\n" +
                                        $"Please direct them to the library desk to pay their dues.",
                                        "Pending Penalties",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning
                                    );
                                    return; // ❌ Stop the borrow process
                                }
                            }
                        }
                    }

                    // ✅ STEP 1: Check borrow limit (max 3 books for Students, unlimited for Faculty)
                    string checkQuery = @"SELECT COUNT(*) FROM IssueBooks 
              WHERE ClientID = @ClientID AND (Status = 'Issued' OR Status = 'Overdue')";
                    using (SqlCommand cmdCheck = new SqlCommand(checkQuery, con))
                    {
                        cmdCheck.Parameters.AddWithValue("@ClientID", ClientID.Text);
                        int currentBorrowed = (int)cmdCheck.ExecuteScalar();

                        string role = GetUserRole(ClientID.Text.Trim());

                        if (role.Equals("Faculty", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("Faculty member detected — borrow limit is not applied.",
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            int totalAfterBorrow = currentBorrowed + borrowList.Count;
                            if (totalAfterBorrow > 3)
                            {
                                MessageBox.Show("Borrow limit exceeded! Each user can only borrow up to 3 books at a time.",
                                    "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }

                    // ✅ STEP 2: Insert query for IssueBooks
                    string insertQuery = @"INSERT INTO IssueBooks 
(ISBN, Status, StudentName, BookTitle, Source, IssueDate, DueDate, Quantity, ClientID, BookCondition)
VALUES (@ISBN, @Status, @StudentName, @BookTitle, @Source, @IssueDate, @DueDate, @Quantity, @ClientID, @BookCondition)";

                    foreach (var item in borrowList)
                    {
                        // ✅ STEP 3: Check available quantity first
                        string checkQtyQuery = "SELECT Quantity FROM BooksAcq WHERE ISBN = @ISBN";
                        using (SqlCommand checkQtyCmd = new SqlCommand(checkQtyQuery, con))
                        {
                            checkQtyCmd.Parameters.AddWithValue("@ISBN", item.ISBN);
                            object qtyResult = checkQtyCmd.ExecuteScalar();

                            if (qtyResult == null)
                            {
                                MessageBox.Show($"Book with ISBN {item.ISBN} not found in database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                continue;
                            }

                            int availableQty = Convert.ToInt32(qtyResult);
                            if (availableQty <= 0)
                            {
                                MessageBox.Show($"The book '{item.BookTitle}' is currently out of stock.", "Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                continue;
                            }
                        }

                        // ✅ STEP 4: Add record to IssueBooks
                        using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@ISBN", item.ISBN);
                            cmd.Parameters.AddWithValue("@Status", "Issued");
                            cmd.Parameters.AddWithValue("@StudentName", ClientName.Text);
                            cmd.Parameters.AddWithValue("@BookTitle", item.BookTitle);
                            cmd.Parameters.AddWithValue("@Source", item.Source);
                            cmd.Parameters.AddWithValue("@IssueDate", issueDateValue);
                            cmd.Parameters.AddWithValue("@DueDate", dueDateValue);
                            cmd.Parameters.AddWithValue("@Quantity", 1);
                            cmd.Parameters.AddWithValue("@ClientID", ClientID.Text);
                            cmd.Parameters.AddWithValue("@BookCondition", item.BookCondition);

                            cmd.ExecuteNonQuery();
                        }

                        // ✅ STEP 5: Decrease quantity in BooksAcq
                        string updateQtyQuery = "UPDATE BooksAcq SET Quantity = Quantity - 1 WHERE ISBN = @ISBN";
                        using (SqlCommand updateQtyCmd = new SqlCommand(updateQtyQuery, con))
                        {
                            updateQtyCmd.Parameters.AddWithValue("@ISBN", item.ISBN);
                            updateQtyCmd.ExecuteNonQuery();
                        }

                        MessageBox.Show($"Borrower Type: {clientType}\nDue Date: {dueDateValue:dddd, MMMM dd, yyyy}",
                          "Due Date Info",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);

                        // ✅ STEP 6: Log activity for each issued book
                        ActivityLog.RecordActivity(
                            SessionData.CurrentUserName,
                            "Issue Book",
                            "Issue Module",
                            $"Issued book — Title: {item.BookTitle}, ISBN: {item.ISBN}, Borrower: {ClientName.Text}"
                        );
                    }

                    // ✅ Clear all input fields after adding
                    ISBN.Clear();
                    BookTitle.Items.Clear();
                    BookTitle.Text = "";
                    Source.Clear();
                    issuedCondition.SelectedIndex = -1;
                    ClientID.Clear();
                    ClientName.Text = " ";
                    issueDate.Value = DateTime.Now;
                    Source.Clear();

                    ClientID.Focus();

                    GlobalEvents.RaiseBorrowedDataChanged();
                    GlobalEvents.RaiseOverdueDataChanged();
                    GlobalEvents.RaisePenaltiesDataChanged();
                }

                // ✅ STEP 7: Refresh UI and clear borrow list
                borrowList.Clear();
                dgvBorrowList.Rows.Clear();

                var dashboardForm = Application.OpenForms["Form1"] as Form1;
                if (dashboardForm != null)
                {
                    dashboardForm.UpdateTotalBorrowedLabel();
                }

                LoadIssueBooks();
                LoadReturnedBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error issuing books: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LoadIssueBooks();
            IssueBooksDataGrid.Refresh();
            IssueBooksDataGrid.Update();
            GlobalEvents.RaiseBorrowedDataChanged();
            LoadReturnedBooks();
        }

     

        private string GetUserRole(string clientId)
        {
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;
        Initial Catalog=LibraryDB;
        Integrated Security=True;
        Encrypt=True;
        Trust Server Certificate=True;"))
            {
                con.Open();
                string query = "SELECT Role FROM AddStudentAcc WHERE ClientID = @ClientID"; // Adjust table/column name as needed
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ClientID", clientId);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "Student"; // Default to "Student" if not found
                }
            }
        }


        // ✅ Determine client type (Student / Faculty) automatically from database
        private string GetClientType(string clientID)
        {
            string clientType = "Student"; // default fallback

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
                    string query = "SELECT Role FROM AddStudentAcc WHERE ClientID = @ClientID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", clientID);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            clientType = result.ToString().Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking client type: " + ex.Message,
                                "Database Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

            return clientType;
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
            dgvBorrowList.ReadOnly = false;
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

            // ✅ Use SessionData when creating new Form1
            Form1 form1 = new Form1(SessionData.CurrentEmployeeID);
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


        // AFTER:
        public void UpdateTotalOverdueLabel()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query = @"
                SELECT COUNT(*) 
                FROM IssueBooks
                WHERE 
                    Status = 'Overdue'
                    AND (OverdueDays IS NOT NULL AND OverdueDays > 0)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        int totalOverdue = (int)cmd.ExecuteScalar();

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
            PANELdataList.Visible = true;
            PANELoverdue.Visible = true;

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
            PANELdataList.Visible = false;
            PANELoverdue.Visible = false;
        }



        private bool suppressTextChanged = false; // 🚫 prevent recursive calls

        private void ClientID_TextChanged(object sender, EventArgs e)
        {
            string clientID = ClientID.Text.Trim();

            // 🧩 1️⃣ Too short — clear and stop
            if (clientID.Length < 4)
            {
                ClientName.Text = "";
                IssueRole.Text = "";
                return;
            }

            // 🧩 2️⃣ Too long — invalid ID
            if (clientID.Length > 6)
            {
                MessageBox.Show("Student not found. Please check the Client ID.",
                                "Not Found",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                ClientName.Text = "";
                IssueRole.Text = "";
                return;
            }

            // 🧩 3️⃣ Retrieve from AddStudentAcc
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

            string query = "SELECT Name, Role FROM AddStudentAcc WHERE ClientID = @ClientID";




            // 🔍 Validate ClientID — numbers only
            if (!System.Text.RegularExpressions.Regex.IsMatch(clientID, @"^\d+$"))
            {
                MessageBox.Show("Client ID must contain numbers only.",
                                "Invalid Input",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                ClientID.Text = "";
                return; // ⛔ Stop execution — don’t query the database
            }


            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@ClientID", clientID);
                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // 🧍 Fill Name
                        ClientName.Text = reader["Name"].ToString();

                        // 🎓 Fill Role
                        IssueRole.Text = reader["Role"].ToString();

                        // 🧮 Auto-compute due date
                        DateTime issueDateValue = DateTime.Now;
                        DateTime dueDateValue = ComputeDueDate(IssueRole.Text, issueDateValue);
                        dueDate.Value = dueDateValue;

                        ISBN.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Student not found. Please check the Client ID.",
                                        "Not Found",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);

                        ClientName.Text = "";
                        IssueRole.Text = "";
                    }
                }
            }
        }









        private void ReturnBookID_TextChanged(object sender, EventArgs e)
        {
            string isbn = ReturnedBookID.Text.Trim();

            if (isbn.Length >= 4)
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT BookTitle FROM BooksAcq WHERE ISBN = @ISBN";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ISBN", isbn);
                        con.Open();

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            // ✅ (Optional) you can show the book title somewhere if needed
                            string bookTitle = result.ToString();
                            BookTitle.Text = bookTitle;
                        }
                        else
                        {
                            // No matching ISBN found
                            BookTitle.Text = string.Empty;
                        }
                    }
                }
            }
            else
            {
                // Clear if ISBN text is too short
                BookTitle.Text = string.Empty;
            }
        }





        private void dgvReturnList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void MoveReturnedBooks()
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

            int movedCount = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // ✅ 1️⃣ Get all IssueBooks where Status = 'Returned'
                    string selectQuery = @"
                SELECT IssueID, ClientID, StudentName, Source, Quantity,
                       BookTitle, ISBN, IssueDate, DueDate, ReturnDate, BookCondition
                FROM IssueBooks
                WHERE Status = 'Returned';";

                    DataTable returned = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(selectQuery, con))
                    {
                        da.Fill(returned);
                    }

                    if (returned.Rows.Count == 0)
                    {
                        // No returned records found — exit quietly
                        return;
                    }

                    // ✅ 2️⃣ Loop through returned records
                    foreach (DataRow row in returned.Rows)
                    {
                        using (SqlTransaction tx = con.BeginTransaction())
                        {
                            try
                            {
                                int issueID = Convert.ToInt32(row["IssueID"]);
                                string isbn = row["ISBN"].ToString();

                                // ✅ 3️⃣ Check if already exists in ReturnedBooks (avoid duplicate inserts)
                                string checkQuery = "SELECT COUNT(*) FROM ReturnedBooks WHERE IssueID = @IssueID;";
                                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con, tx))
                                {
                                    checkCmd.Parameters.AddWithValue("@IssueID", issueID);
                                    int exists = (int)checkCmd.ExecuteScalar();
                                    if (exists > 0)
                                    {
                                        tx.Commit(); // already moved, skip
                                        continue;
                                    }
                                }

                                // ✅ 4️⃣ Insert into ReturnedBooks
                                string insert = @"
INSERT INTO ReturnedBooks
    (IssueID, ClientID, ClientName, BookTitle, Quantity, Source,
     IssueDate, DueDate, ReturnDate, Status, BookCondition)
VALUES
    (@IssueID, @ClientID, @ClientName, @BookTitle, @Quantity, @Source,
     @IssueDate, @DueDate, @ReturnDate, 'Returned', @BookCondition);";

                                using (SqlCommand cmdIns = new SqlCommand(insert, con, tx))
                                {
                                    cmdIns.Parameters.AddWithValue("@IssueID", issueID);
                                    cmdIns.Parameters.AddWithValue("@ClientID", row["ClientID"]);
                                    cmdIns.Parameters.AddWithValue("@ClientName", row["StudentName"]);
                                    cmdIns.Parameters.AddWithValue("@BookTitle", row["BookTitle"]);
                                    cmdIns.Parameters.AddWithValue("@Quantity", row["Quantity"]);
                                    cmdIns.Parameters.AddWithValue("@Source", row["Source"]);
                                    cmdIns.Parameters.AddWithValue("@IssueDate", row["IssueDate"]);
                                    cmdIns.Parameters.AddWithValue("@DueDate", row["DueDate"]);
                                    cmdIns.Parameters.AddWithValue("@ReturnDate", row["ReturnDate"]);
                                    cmdIns.Parameters.AddWithValue("@BookCondition", row["BookCondition"]);
                                    cmdIns.ExecuteNonQuery();
                                }

                                // ✅ 5️⃣ Delete from IssueBooks
                                string delete = "DELETE FROM IssueBooks WHERE IssueID = @IssueID;";
                                using (SqlCommand cmdDel = new SqlCommand(delete, con, tx))
                                {
                                    cmdDel.Parameters.AddWithValue("@IssueID", issueID);
                                    cmdDel.ExecuteNonQuery();
                                }

                                // ✅ 6️⃣ Increase quantity back in BooksAcq
                                string updateQty = "UPDATE BooksAcq SET Quantity = Quantity + 1 WHERE ISBN = @ISBN;";
                                using (SqlCommand cmdQty = new SqlCommand(updateQty, con, tx))
                                {
                                    cmdQty.Parameters.AddWithValue("@ISBN", isbn);
                                    cmdQty.ExecuteNonQuery();
                                }

                                tx.Commit();
                                movedCount++;
                            }
                            catch (Exception ex)
                            {
                                tx.Rollback();
                                MessageBox.Show("Error moving record (IssueID: " + row["IssueID"] + "):\n" + ex.Message,
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }

                // ✅ 7️⃣ Only show success if at least 1 was moved
                if (movedCount > 0)
                {

                }

                // 🔄 Optional: refresh your grid after move
                LoadIssueBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error moving returned books:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            string selectedBook = ReturnedBookID.SelectedItem.ToString();
            string isbn = selectedBook.Split('-')[0].Trim();
            string clientID = ReturnClientID.Text.Trim();
            string clientName = ReturnClientName.Text;
            string clientType = ReturnRoleComboBox.Text;
            string bookTitle = selectedBook.Split('-').Length > 1 ? selectedBook.Split('-')[1].Trim() : "Unknown";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // 1️⃣ Get issue details
                    int issueID = 0;
                    DateTime issueDate = DateTime.Now;
                    DateTime dueDate = DateTime.Now;
                    string source = "";
                    int quantity = 1;
                    string issuedConditionFromDB = "";

                    string getIssueQuery = @"
SELECT TOP 1 IssueID, IssueDate, DueDate, Source, Quantity, BookCondition
FROM IssueBooks
WHERE ClientID = @ClientID AND ISBN = @ISBN
AND (Status = 'Issued' OR Status = 'Overdue')
ORDER BY IssueDate DESC";

                    using (SqlCommand cmdGet = new SqlCommand(getIssueQuery, con, transaction))
                    {
                        cmdGet.Parameters.AddWithValue("@ClientID", clientID);
                        cmdGet.Parameters.AddWithValue("@ISBN", isbn);

                        using (SqlDataReader reader = cmdGet.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                issueID = Convert.ToInt32(reader["IssueID"]);
                                issueDate = Convert.ToDateTime(reader["IssueDate"]);
                                dueDate = Convert.ToDateTime(reader["DueDate"]);
                                source = reader["Source"].ToString();
                                quantity = Convert.ToInt32(reader["Quantity"]);
                                issuedConditionFromDB = reader["BookCondition"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("No matching issued book found for return.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                transaction.Rollback();
                                return;
                            }
                        }
                    }

                    // 2️⃣ Show issued condition (before issuing) in read-only ComboBox
                    issuedCondition.Items.Clear();
                    issuedCondition.Items.Add(issuedConditionFromDB);
                    issuedCondition.SelectedIndex = 0;
                    issuedCondition.Enabled = false; // librarian cannot change it

                    // 3️⃣ Get the selected current condition (upon return)
                    string selectedReturnCondition = returnCondition.SelectedItem?.ToString();

                    if (string.IsNullOrEmpty(selectedReturnCondition))
                    {
                        MessageBox.Show("Please select the book condition upon return.",
                            "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        transaction.Rollback();
                        return;
                    }

                    // ✅ ENHANCED CONDITION VALIDATION
                    string issuedCond = issuedConditionFromDB.Trim().ToLower();
                    string returnedCond = selectedReturnCondition.Trim().ToLower();

                    // Define condition hierarchy (worst → best)
                    Dictionary<string, int> conditionRank = new Dictionary<string, int>
            {
                { "lost", 1 },
                { "major damage", 2 },
                { "damaged", 3 },
                { "minor damage", 4 },
                { "minor damaged", 4 }, // handle both variants
                { "good", 5 },
                { "good condition", 5 },
                { "new", 6 }
            };

                    // ✅ Check if both conditions are valid
                    if (!conditionRank.ContainsKey(issuedCond))
                    {
                        MessageBox.Show($"Invalid issued condition: {issuedConditionFromDB}\n\nPlease contact system administrator.",
                            "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        transaction.Rollback();
                        return;
                    }

                    if (!conditionRank.ContainsKey(returnedCond))
                    {
                        MessageBox.Show($"Invalid return condition: {selectedReturnCondition}\n\nPlease select a valid condition from the dropdown.",
                            "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        transaction.Rollback();
                        return;
                    }

                    int issuedRank = conditionRank[issuedCond];
                    int returnedRank = conditionRank[returnedCond];

                    // ✅ CHECK IF CONDITION IMPROVED
                    if (returnedRank > issuedRank)
                    {
                        DialogResult improvedResult = MessageBox.Show(
                            $"📈 Book Condition Improved!\n\n" +
                            $"Book was issued as: {issuedConditionFromDB}\n" +
                            $"Returned as: {selectedReturnCondition}\n\n" +
                            $"The book condition has IMPROVED.\n" +
                            $"Do you want to update the book's condition in the database?\n\n" +
                            $"Or are you just confused with the condition?\n\n" +
                            $"• Click YES to save and update the book condition\n" +
                            $"• Click NO to cancel the entire transaction",
                            "Condition Improved",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (improvedResult == DialogResult.Yes)
                        {
                            // ✅ Update BookCondition in BooksAcq table
                            string updateCondition = @"
UPDATE BooksAcq 
SET BookCondition = @NewCondition 
WHERE ISBN = @ISBN";

                            using (SqlCommand cmdUpdateCond = new SqlCommand(updateCondition, con, transaction))
                            {
                                cmdUpdateCond.Parameters.AddWithValue("@NewCondition", selectedReturnCondition);
                                cmdUpdateCond.Parameters.AddWithValue("@ISBN", isbn);
                                cmdUpdateCond.ExecuteNonQuery();
                            }

                            // ✅ ALSO UPDATE IssueBooks BookCondition (so it reflects the improved condition)
                            string updateIssueCondition = @"
UPDATE IssueBooks 
SET BookCondition = @NewCondition 
WHERE IssueID = @IssueID";

                            using (SqlCommand cmdUpdateIssueCond = new SqlCommand(updateIssueCondition, con, transaction))
                            {
                                cmdUpdateIssueCond.Parameters.AddWithValue("@NewCondition", selectedReturnCondition);
                                cmdUpdateIssueCond.Parameters.AddWithValue("@IssueID", issueID);
                                cmdUpdateIssueCond.ExecuteNonQuery();
                            }

                            // ✅ Log the condition improvement
                            ActivityLog.RecordActivity(
                                SessionData.CurrentUserName,
                                "Update Book Condition",
                                "Return Module",
                                $"Book condition improved — ISBN: {isbn}, Title: {bookTitle}, From: {issuedConditionFromDB} → To: {selectedReturnCondition}"
                            );
                        }
                        else // User clicked NO
                        {
                            // ✅ Cancel the entire transaction - book will NOT be marked as returned
                            transaction.Rollback();
                            MessageBox.Show(
                                "Transaction cancelled. The book has NOT been marked as returned.\n" +
                                "Please verify the condition and try again.",
                                "Transaction Cancelled",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            return; // ❌ Exit completely
                        }
                    }

                    // ✅ CHECK FOR WORSENED CONDITION
                    else if (returnedRank < issuedRank)
                    {
                        DialogResult result = MessageBox.Show(
                            $"⚠️ Book Condition Mismatch Detected!\n\n" +
                            $"Issued as: {issuedConditionFromDB}\n" +
                            $"Returned as: {selectedReturnCondition}\n\n" +
                            $"The book condition has worsened.\n" +
                            $"Do you want to create a damage report?",
                            "Condition Mismatch",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (result == DialogResult.Yes)
                        {
                            // ✅ ROLLBACK TRANSACTION - Don't process the return
                            transaction.Rollback();

                            // ✅ Open the damage report form with dynamic data
                            DamagedBookReport damageForm = new DamagedBookReport()
                            {
                                StartPosition = FormStartPosition.CenterScreen,
                                PreClientID = ReturnClientID.Text.Trim(),
                                PreISBN = isbn,
                                PreBookTitle = bookTitle
                            };
                            damageForm.Show();

                            // ✅ EXIT - Stop processing
                            return;
                        }
                        else if (result == DialogResult.No)
                        {
                            transaction.Rollback();
                            return;
                        }
                    }

                    // ✅ If conditions match exactly (issuedRank == returnedRank), continue normally

                    // 4️⃣ Update IssueBooks to mark as Returned
                    string updateIssue = @"
UPDATE IssueBooks
SET Status = 'Returned',
    ReturnDate = GETDATE()
WHERE IssueID = @IssueID;";
                    using (SqlCommand cmdUpdate = new SqlCommand(updateIssue, con, transaction))
                    {
                        cmdUpdate.Parameters.AddWithValue("@IssueID", issueID);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    // 5️⃣ Insert into ReturnedBooks (with condition)
                    string insertReturned = @"
INSERT INTO ReturnedBooks 
    (IssueID, ClientID, ClientName, ClientType, BookTitle, Quantity, Source, IssueDate, DueDate, ReturnDate, Status, BookCondition)
VALUES
    (@IssueID, @ClientID, @ClientName, @ClientType, @BookTitle, @Quantity, @Source, @IssueDate, @DueDate, GETDATE(), 'Returned', @BookCondition);";

                    using (SqlCommand cmdInsert = new SqlCommand(insertReturned, con, transaction))
                    {
                        cmdInsert.Parameters.AddWithValue("@IssueID", issueID);
                        cmdInsert.Parameters.AddWithValue("@ClientID", clientID);
                        cmdInsert.Parameters.AddWithValue("@ClientName", clientName);
                        cmdInsert.Parameters.AddWithValue("@ClientType", clientType);
                        cmdInsert.Parameters.AddWithValue("@BookTitle", bookTitle);
                        cmdInsert.Parameters.AddWithValue("@Quantity", quantity);
                        cmdInsert.Parameters.AddWithValue("@Source", source);
                        cmdInsert.Parameters.AddWithValue("@IssueDate", issueDate);
                        cmdInsert.Parameters.AddWithValue("@DueDate", dueDate);
                        cmdInsert.Parameters.AddWithValue("@BookCondition", selectedReturnCondition);
                        cmdInsert.ExecuteNonQuery();
                    }

                    // 6️⃣ Delete from IssueBooks
                    string deleteIssue = "DELETE FROM IssueBooks WHERE IssueID = @IssueID;";
                    using (SqlCommand cmdDelete = new SqlCommand(deleteIssue, con, transaction))
                    {
                        cmdDelete.Parameters.AddWithValue("@IssueID", issueID);
                        cmdDelete.ExecuteNonQuery();
                    }

                    // 7️⃣ Increase quantity back in BooksAcq
                    string updateQty = "UPDATE BooksAcq SET Quantity = Quantity + 1 WHERE ISBN = @ISBN;";
                    using (SqlCommand cmdQty = new SqlCommand(updateQty, con, transaction))
                    {
                        cmdQty.Parameters.AddWithValue("@ISBN", isbn);
                        cmdQty.ExecuteNonQuery();
                    }

                    // 8️⃣ Commit all
                    transaction.Commit();

                    // 9️⃣ Log activity
                    ActivityLog.RecordActivity(
                        SessionData.CurrentUserName,
                        "Return Book",
                        "Return Module",
                        $"Returned book — Title: {bookTitle}, Borrower: {clientName}, Condition: {selectedReturnCondition}"
                    );

                    MessageBox.Show("Book returned successfully! Status updated, moved to ReturnedBooks, and quantity adjusted.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    lblReturnDate.Text = $"Returned on: {DateTime.Now:MMMM dd, yyyy hh:mm tt}";

                    // 🔄 Clear UI
                    ReturnClientID.Clear();
                    ReturnClientName.Text = " ";
                    ReturnedBookID.Items.Clear();
                    ReturnBookStatus.Text = "";
                    ReturnPenalty.Clear();
                    returnCondition.SelectedIndex = -1; // Clear return condition selection
                    issuedCondition.Items.Clear(); // Clear issued condition

                    // 🔄 Refresh UI
                    LoadIssueBooks();
                    LoadReturnedBooks();
                    GlobalEvents.RaiseBorrowedDataChanged();
                    GlobalEvents.RaiseOverdueDataChanged();
                    GlobalEvents.RaisePenaltiesDataChanged();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error returning book:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void AddRemoveButtonToBorrowList()
        {
            // Check if the button already exists
            if (dgvBorrowList.Columns["Remove"] == null)
            {
                DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
                btn.HeaderText = "Action";
                btn.Name = "Remove";
                btn.Text = "Remove";
                btn.UseColumnTextForButtonValue = true;
                btn.FlatStyle = FlatStyle.Flat;
                btn.DefaultCellStyle.BackColor = Color.OrangeRed;
                btn.DefaultCellStyle.ForeColor = Color.White;
                btn.DefaultCellStyle.SelectionBackColor = Color.DarkRed;
                btn.Width = 90;

                dgvBorrowList.Columns.Add(btn);
            }
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

        private void ISBN_TextChanged(object sender, EventArgs e)
        {
            string isbn = ISBN.Text.Trim();



            if (isbn.Length >= 4)
            {
                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;";
                string query = "SELECT BookTitle, Source, BookCondition FROM BooksAcq WHERE ISBN = @ISBN";


                // 🔍 Validate ISBN — numbers only
                if (!System.Text.RegularExpressions.Regex.IsMatch(isbn, @"^\d+$"))
                {
                    MessageBox.Show("ISBN must contain numbers only.",
                                    "Invalid Input",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    ISBN.Text = "";
                    return; // ⛔ Stop execution — don’t query the database
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ISBN", isbn);
                        con.Open();

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            // ✅ Book Title
                            string title = reader["BookTitle"].ToString();
                            BookTitle.Items.Clear();
                            BookTitle.Items.Add(title);
                            BookTitle.SelectedIndex = 0;
                            BookTitle.Text = title;

                            // ✅ Source
                            Source.Text = reader["Source"].ToString();

                            // ✅ Book Condition
                            string condition = reader["BookCondition"].ToString();
                            issuedCondition.Items.Clear();
                            issuedCondition.Items.Add(condition);
                            issuedCondition.SelectedIndex = 0;
                            issuedCondition.Text = condition;

                            btnAddToList.Focus();
                        }
                        else
                        {
                            BookTitle.Items.Clear();
                            Source.Text = string.Empty;
                            issuedCondition.Items.Clear();
                        }
                    }
                }
            }
            else
            {
                BookTitle.Items.Clear();
                Source.Text = string.Empty;
                issuedCondition.Items.Clear();
            }
        }


        private void label4_Click(object sender, EventArgs e)
        {

        }
        // ✅ Compute due date based on client type
        // ✅ Compute due date based on client type and skip Sundays/holidays for students
        private DateTime ComputeDueDate(string clientType, DateTime issueDate)
        {
            HashSet<DateTime> philippineHolidays = new HashSet<DateTime>
    {
        new DateTime(issueDate.Year, 1, 1),
        new DateTime(issueDate.Year, 4, 9),
        new DateTime(issueDate.Year, 5, 1),
        new DateTime(issueDate.Year, 6, 12),
        new DateTime(issueDate.Year, 8, 21),
        new DateTime(issueDate.Year, 11, 1),
        new DateTime(issueDate.Year, 11, 30),
        new DateTime(issueDate.Year, 12, 25),
        new DateTime(issueDate.Year, 12, 30)
    };

            if (clientType.Equals("Faculty", StringComparison.OrdinalIgnoreCase))
            {
                return issueDate.AddMonths(3);
            }
            else
            {
                int validDays = 0;
                DateTime due = issueDate;
                while (validDays < 3)
                {
                    due = due.AddDays(1);
                    if (due.DayOfWeek != DayOfWeek.Sunday && !philippineHolidays.Contains(due.Date))
                    {
                        validDays++;
                    }
                }
                return due;
            }
        }

        private void ReturnClientID_TextChanged(object sender, EventArgs e)
        {
            string clientID = ReturnClientID.Text.Trim();

            // 🧩 1️⃣ Validate ClientID length first
            if (clientID.Length > 6)
            {
                MessageBox.Show("Student not found. Please check the Client ID.",
                                "Not Found",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                // Clear all fields
                ReturnClientName.Text = " ";
                ReturnPenalty.Clear();



                ReturnBookQty.Text = "";
                ReturnedBookID.Items.Clear();
                ReturnBookStatus.Text = "";
                CMBbookConditon.Items.Clear();
                return;
            }

            // 🧩 2️⃣ Proceed only if ClientID has 4 to 6 digits
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

                    // ✅ 1️⃣ Get Student Name and Role from AddStudentAcc
                    string queryClient = "SELECT Name, Role FROM AddStudentAcc WHERE ClientID = @ClientID";
                    using (SqlCommand cmdClient = new SqlCommand(queryClient, con))
                    {
                        cmdClient.Parameters.AddWithValue("@ClientID", ReturnClientID.Text.Trim());
                        using (SqlDataReader reader = cmdClient.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // 🧍 Fill client name TextBox
                                string name = reader["Name"].ToString();
                                ReturnClientName.Text = name; // ✅ Now using Text, not Items

                                // 🎓 Fill role (Student/Faculty)
                                string role = reader["Role"].ToString();
                                ReturnRoleComboBox.Text = role; // ✅ Works for both ComboBox or Guna2TextBox
                            }
                            else
                            {
                                MessageBox.Show("Student not found. Please check the Client ID.",
                                                "Not Found",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);

                                // 🧹 Clear all related fields
                                ReturnClientName.Clear();
                                ReturnRoleComboBox.Text = "";
                                ReturnBookQty.Clear(); // ✅ Changed from ComboBox to TextBox
                                ReturnedBookID.Items.Clear(); // Assuming this one is still a ComboBox
                                ReturnBookStatus.Text = "";
                                ReturnPenalty.Clear();
                                CMBbookConditon.Items.Clear();
                                return;
                            }

                            // 🎓 Fill role (Student/Faculty)
                            string userRole = reader["Role"]?.ToString() ?? "";

                            // Check if ReturnRoleComboBox is a real ComboBox (not Guna2TextBox)
                            if (ReturnRoleComboBox is System.Windows.Forms.ComboBox comboBox)
                            {
                                comboBox.Items.Clear();

                                if (!string.IsNullOrWhiteSpace(userRole))
                                {
                                    comboBox.Items.Add(userRole);
                                    comboBox.SelectedIndex = 0;
                                }
                                else
                                {
                                    comboBox.Text = "";
                                }
                            }
                            else
                            {
                                // If it's a TextBox (e.g., Guna2TextBox)
                                ReturnRoleComboBox.Text = userRole;
                            }








                        }

                    }

                    // ✅ 2️⃣ Get Borrowed Books Info
                    string queryBorrow = @"
            SELECT 
                ISBN,
                BookTitle,
                Status,
                Penalty,
                Quantity,
                IssueID,
                Source
            FROM IssueBooks
            WHERE ClientID = @ClientID
              AND (Status = 'Issued' OR Status = 'Overdue')";

                    using (SqlCommand cmdBorrow = new SqlCommand(queryBorrow, con))
                    {
                        cmdBorrow.Parameters.AddWithValue("@ClientID", clientID);

                        using (SqlDataReader reader = cmdBorrow.ExecuteReader())
                        {
                            int bookCount = 0;
                            double totalPenalty = 0;
                            List<string> isbnAndTitles = new List<string>();
                            List<string> statuses = new List<string>();

                            ReturnedBookID.Items.Clear();

                            while (reader.Read())
                            {
                                bookCount++;

                                string isbn = reader["ISBN"] != DBNull.Value ? reader["ISBN"].ToString() : "N/A";
                                string bookTitle = reader["BookTitle"] != DBNull.Value ? reader["BookTitle"].ToString() : "Unknown";

                                isbnAndTitles.Add($"{isbn} - {bookTitle}");
                                statuses.Add(reader["Status"].ToString());

                                if (double.TryParse(reader["Penalty"].ToString(), out double penalty))
                                    totalPenalty += penalty;
                            }

                            // ✅ Fill borrowed book data
                            ReturnBookQty.Text = bookCount.ToString(); // directly display count in textbox


                            foreach (var entry in isbnAndTitles)
                                ReturnedBookID.Items.Add(entry);
                            if (ReturnedBookID.Items.Count > 0)
                                ReturnedBookID.SelectedIndex = 0;

                            // Assuming 'statuses' is a collection (like List<string>)
                            if (statuses != null && statuses.Any())
                            {
                                ReturnBookStatus.Text = statuses.Distinct().First();
                            }
                            else
                            {
                                ReturnBookStatus.Clear();
                            }


                            ReturnPenalty.Text = totalPenalty.ToString("0.00");
                        }
                    }

                    // ✅ 3️⃣ If there’s at least one borrowed book, fetch its BookCondition
                    if (ReturnedBookID.Items.Count > 0)
                    {
                        string selectedISBN = ReturnedBookID.Items[0].ToString().Split('-')[0].Trim();

                        string conditionQuery = "SELECT BookCondition FROM BooksAcq WHERE ISBN = @ISBN";
                        using (SqlCommand cmdCond = new SqlCommand(conditionQuery, con))
                        {
                            cmdCond.Parameters.AddWithValue("@ISBN", selectedISBN);
                            object condResult = cmdCond.ExecuteScalar();

                            CMBbookConditon.Items.Clear();

                            if (condResult != null)
                            {
                                string condition = condResult.ToString();
                                CMBbookConditon.Items.Add(condition);
                                CMBbookConditon.SelectedIndex = 0;
                                CMBbookConditon.Text = condition;
                            }
                        }
                    }
                }
            }
            else
            {
                // ✅ Clear all if ClientID too short
                ReturnClientName.Clear();        // Changed from .Items.Clear()
                ReturnPenalty.Clear();

                ReturnRoleComboBox.Text = "";    // Works for both ComboBox or Guna2TextBox
                ReturnBookQty.Clear();           // Changed from .Items.Clear()

                ReturnedBookID.Items.Clear();    // Assuming this is still a ComboBox
                ReturnBookStatus.Text = "";
                CMBbookConditon.Items.Clear();   // Assuming this is still a ComboBox

            }



            // Check if the textbox has any text
            if (ReturnClientID.Text.Length == 4)
            {
                // Move focus to the button
                ReturnButton.Focus();
            }
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


        private void ReturnBookQty_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void RoleComboBox_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void CMBbookConditon_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is DamagedBookReport)
                {
                    openForm.Show();
                    this.Hide();
                    return;
                }
            }

            // ✅ Use SessionData when creating new Form1
            DamagedBookReport damage = new DamagedBookReport();
            damage.Show();
            this.Hide();
        }

        private void dgvBorrowList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // ✅ Ignore header clicks or invalid rows
            if (e.RowIndex < 0 || e.RowIndex >= dgvBorrowList.Rows.Count)
                return;

            // ✅ Only respond to clicks on the "Remove" column
            if (dgvBorrowList.Columns[e.ColumnIndex].Name == "Remove")
            {
                // ✅ Check if borrowList is in sync and index is valid
                if (borrowList == null || e.RowIndex >= borrowList.Count)
                {
                    MessageBox.Show("List is out of sync. Please refresh or try again.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ✅ Get the quantity value safely
                object cellValue = dgvBorrowList.Rows[e.RowIndex].Cells["Quantity"].Value;
                int quantity = 0;

                if (cellValue != null && int.TryParse(cellValue.ToString(), out int parsedQty))
                    quantity = parsedQty;

                // ✅ Prevent removing if quantity < 1
                if (quantity < 1)
                {
                    MessageBox.Show("Quantity cannot be less than 1.", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ✅ Confirm before removal
                var result = MessageBox.Show("Remove this book from the list?",
                                             "Confirm Remove",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // ✅ Double-check index range before removal
                    if (e.RowIndex < borrowList.Count)
                        borrowList.RemoveAt(e.RowIndex);

                    if (e.RowIndex < dgvBorrowList.Rows.Count)
                        dgvBorrowList.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private void ReturnClientID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // prevent "ding" sound
                ReturnButton.PerformClick(); // trigger button click
            }
        }

        private void kupal_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void ReturnedBookID_TextChanged(object sender, EventArgs e)
        {
            if (ReturnedBookID.SelectedItem == null)
                return;

            // Extract ISBN from selected item (format: "ISBN - BookTitle")
            string selectedItem = ReturnedBookID.SelectedItem.ToString();
            string selectedISBN = selectedItem.Split('-')[0].Trim();

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // ✅ Get the BookCondition from IssueBooks (condition when issued)
                string conditionQuery = @"
            SELECT BookCondition 
            FROM IssueBooks 
            WHERE ISBN = @ISBN 
              AND ClientID = @ClientID
              AND (Status = 'Issued' OR Status = 'Overdue')";

                using (SqlCommand cmd = new SqlCommand(conditionQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ISBN", selectedISBN);
                    cmd.Parameters.AddWithValue("@ClientID", ReturnClientID.Text.Trim());

                    object condResult = cmd.ExecuteScalar();

                    if (condResult != null)
                    {
                        string issuedCondition = condResult.ToString();

                        // ✅ Update CMBbookConditon (Book Condition before issued)
                        CMBbookConditon.Items.Clear();
                        CMBbookConditon.Items.Add(issuedCondition);
                        CMBbookConditon.SelectedIndex = 0;
                        CMBbookConditon.Text = issuedCondition;

                        // ✅ Store issued condition for validation later
                        CMBbookConditon.Tag = issuedCondition; // Store for comparison
                    }
                }
            }
        }

        private void ReturnedBookID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReturnedBookID.SelectedItem == null)
                return;

            // Extract ISBN from selected item (format: "ISBN - BookTitle")
            string selectedItem = ReturnedBookID.SelectedItem.ToString();
            string selectedISBN = selectedItem.Split('-')[0].Trim();

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // ✅ Get the BookCondition from IssueBooks (condition when issued)
                string conditionQuery = @"
            SELECT BookCondition 
            FROM IssueBooks 
            WHERE ISBN = @ISBN 
              AND ClientID = @ClientID
              AND (Status = 'Issued' OR Status = 'Overdue')";

                using (SqlCommand cmd = new SqlCommand(conditionQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ISBN", selectedISBN);
                    cmd.Parameters.AddWithValue("@ClientID", ReturnClientID.Text.Trim());

                    object condResult = cmd.ExecuteScalar();

                    if (condResult != null)
                    {
                        string issuedCondition = condResult.ToString();

                        // ✅ Update CMBbookConditon (Book Condition before issued)
                        CMBbookConditon.Items.Clear();
                        CMBbookConditon.Items.Add(issuedCondition);
                        CMBbookConditon.SelectedIndex = 0;
                        CMBbookConditon.Text = issuedCondition;
                    }
                }
            }
        }
    }
}
