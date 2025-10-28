using Library_Final;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
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
        private List<(string ISBN, string BookTitle, string Source)> borrowList = new List<(string, string, string)>();




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
                HighlightOverdueRows();

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

            // ✅ Highlight overdue rows in red
            foreach (DataGridViewRow row in IssueBooksDataGrid.Rows)
            {
                if (row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();
                    if (status.Equals("Overdue", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral; // red shade
                        row.DefaultCellStyle.ForeColor = Color.White;
                    }
                }
            }

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
            dgvBorrowList.Columns.Add("ISBN", "ISBN");

            dgvBorrowList.Columns.Add("BookTitle", "Book Title");
            dgvBorrowList.Columns.Add("Source", "Source");

            overdueTimer = new System.Windows.Forms.Timer();
            overdueTimer.Interval = 1000; // every 1 sec (adjust as you like)
            overdueTimer.Tick += overdueTimer_Tick;
            overdueTimer.Start();
            HighlightOverdueRows();

            overdueTimer = new System.Windows.Forms.Timer();
            overdueTimer.Interval = 1000; // every 1 sec (adjust as you like)
            overdueTimer.Tick += overdueTimer_Tick;
            overdueTimer.Start();


            UpdateTotalOverdueLabel();

            //Issue and Return Panel Visibility
            panelIssueBooks.Visible = true;
            panel1IssueDataGrid.Visible = true;
            PANELdataList.Visible = true;

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
                            BookConditioncmb.Items.Clear();
                            BookConditioncmb.Items.Add(condition);
                            BookConditioncmb.SelectedIndex = 0;
                            BookConditioncmb.Text = condition;
                        }
                        else
                        {
                            // Clear fields if no record found
                            BookTitle.Items.Clear();
                            Source.Text = string.Empty;
                            BookConditioncmb.Items.Clear();
                        }
                    }
                }
            }
            else
            {
                // Clear fields if input too short
                BookTitle.Items.Clear();
                Source.Text = string.Empty;
                BookConditioncmb.Items.Clear();
            }
        }





        private void btnAddToList_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ISBN.Text) || string.IsNullOrWhiteSpace(BookTitle.Text))
            {
                MessageBox.Show("Please select a valid book.");
                return;
            }

            // Add to list
            borrowList.Add((ISBN.Text, BookTitle.Text, Source.Text));

            if (dgvBorrowList.Columns.Count < 3)
            {
                dgvBorrowList.Columns.Clear();
                dgvBorrowList.Columns.Add("ISBN", "ISBN");
                dgvBorrowList.Columns.Add("BookTitle", "Book Title");
                dgvBorrowList.Columns.Add("Source", "Source");
            }


            //remove later yah
            MessageBox.Show($"Borrow list contains {borrowList.Count} books.");


            dgvBorrowList.Rows.Add(ISBN.Text, BookTitle.Text, Source.Text);

            // Clear for next entry
            ISBN.Clear();
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

            DateTime issueDateValue = issueDate.Value;

            // 🔍 Automatically get client type from database
            string clientType = GetClientType(ClientID.Text.Trim());

            // 🧮 Compute due date based on client type
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

                    // ✅ STEP 1: Check borrow limit (max 3 books)
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

                    // ✅ STEP 2: Insert query for IssueBooks
                    string insertQuery = @"INSERT INTO IssueBooks 
               (ISBN, Status, StudentName, BookTitle, Source, IssueDate, DueDate, Quantity, ClientID)
               VALUES (@ISBN, @Status, @StudentName, @BookTitle, @Source, @IssueDate, @DueDate, @Quantity, @ClientID)";
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


                    GlobalEvents.RaiseBorrowedDataChanged();
                    GlobalEvents.RaiseOverdueDataChanged();
                    GlobalEvents.RaisePenaltiesDataChanged();
                }

                // ✅ STEP 6: Refresh UI and clear borrow list
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
            PANELdataList.Visible = true;

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
        }



        private void ClientID_TextChanged(object sender, EventArgs e)
        {
            string clientID = ClientID.Text.Trim();

            // 🧩 Validate ClientID length first
            if (clientID.Length >= 6)
            {
                MessageBox.Show("Student not found. Please check the Client ID.",
                                "Not Found",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                // Clear fields
                if (ClientName is ComboBox nameCombo)
                    nameCombo.Items.Clear();
                else
                    ClientName.Text = "";

                if (IssueRole is ComboBox roleCombo)
                    roleCombo.Items.Clear();
                else
                    IssueRole.Text = "";

                return; // stop further execution
            }

            if (clientID.Length >= 4)
            {
                string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

                // ✅ Get both Name and Role from AddStudentAcc
                string query = "SELECT Name, Role FROM AddStudentAcc WHERE ClientID = @ClientID";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", clientID);

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            // 🧍 Fill Client Name
                            string name = reader["Name"].ToString();
                            if (ClientName is ComboBox nameCombo)
                            {
                                nameCombo.Items.Clear();
                                nameCombo.Items.Add(name);
                                nameCombo.SelectedIndex = 0;
                            }
                            else
                            {
                                ClientName.Text = name;
                            }

                            // 🎓 Fill Role
                            string role = reader["Role"].ToString();
                            if (IssueRole is ComboBox roleCombo)
                            {
                                roleCombo.Items.Clear();
                                roleCombo.Items.Add(role);
                                roleCombo.SelectedIndex = 0;
                            }
                            else
                            {
                                IssueRole.Text = role;
                            }

                            // 🧮 Auto-update due date
                            DateTime issueDateValue = DateTime.Now;
                            DateTime dueDateValue = ComputeDueDate(role, issueDateValue);
                            dueDate.Value = dueDateValue;
                        }
                        else
                        {
                            // ❌ No match found for this ClientID
                            MessageBox.Show("Student not found. Please check the Client ID.",
                                            "Not Found",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning);

                            if (ClientName is ComboBox nameCombo)
                                nameCombo.Items.Clear();
                            else
                                ClientName.Text = "";

                            if (IssueRole is ComboBox roleCombo)
                                roleCombo.Items.Clear();
                            else
                                IssueRole.Text = "";
                        }
                    }
                }
            }
            else
            {
                // Clear if ClientID too short
                if (ClientName is ComboBox nameCombo)
                    nameCombo.Items.Clear();
                else
                    ClientName.Text = "";

                if (IssueRole is ComboBox roleCombo)
                    roleCombo.Items.Clear();
                else
                    IssueRole.Text = "";
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

            string selectedBook = ReturnedBookID.SelectedItem.ToString();
            string isbn = selectedBook.Split('-')[0].Trim();
            string clientID = ReturnClientID.Text.Trim();
            string clientName = ReturnClientName.Text;
            string clientType = ReturnRoleComboBox.Text; // get actual role if available
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

                    string getIssueQuery = @"
                SELECT TOP 1 IssueID, IssueDate, DueDate, Source, Quantity
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
                            }
                            else
                            {
                                MessageBox.Show("No matching issued book found for return.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }

                    // 2️⃣ Update IssueBooks to mark as Returned (before deletion)
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

                    // 3️⃣ Insert into ReturnedBooks
                    string insertReturned = @"
                INSERT INTO ReturnedBooks 
                    (IssueID, ClientID, ClientName, ClientType, BookTitle, Quantity, Source, IssueDate, DueDate, ReturnDate, Status)
                VALUES
                    (@IssueID, @ClientID, @ClientName, @ClientType, @BookTitle, @Quantity, @Source, @IssueDate, @DueDate, GETDATE(), 'Returned');";
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
                        cmdInsert.ExecuteNonQuery();
                    }

                    // 4️⃣ Delete the record from IssueBooks
                    string deleteIssue = "DELETE FROM IssueBooks WHERE IssueID = @IssueID;";
                    using (SqlCommand cmdDelete = new SqlCommand(deleteIssue, con, transaction))
                    {
                        cmdDelete.Parameters.AddWithValue("@IssueID", issueID);
                        cmdDelete.ExecuteNonQuery();
                    }

                    // 5️⃣ Increase quantity back in BooksAcq
                    string updateQty = "UPDATE BooksAcq SET Quantity = Quantity + 1 WHERE ISBN = @ISBN;";
                    using (SqlCommand cmdQty = new SqlCommand(updateQty, con, transaction))
                    {
                        cmdQty.Parameters.AddWithValue("@ISBN", isbn);
                        cmdQty.ExecuteNonQuery();
                    }

                    // 6️⃣ Commit all changes
                    transaction.Commit();

                    // 7️⃣ Log the action
                    ActivityLog.RecordActivity(
                        SessionData.CurrentUserName,
                        "Return Book",
                        "Return Module",
                        $"Returned book — Title: {bookTitle}, Borrower: {clientName}"
                    );

                    MessageBox.Show("Book returned successfully! Status updated, moved to ReturnedBooks, and quantity adjusted.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    lblReturnDate.Text = $"Returned on: {DateTime.Now:MMMM dd, yyyy hh:mm tt}";

                    // ✅ Clear fields
                    ReturnClientID.Clear();
                    ReturnClientName.Items.Clear();
                    ReturnedBookID.Items.Clear();
                    ReturnBookStatus.Items.Clear();
                    ReturnPenalty.Clear();

                    // ✅ Refresh UI
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
                            BookConditioncmb.Items.Clear();
                            BookConditioncmb.Items.Add(condition);
                            BookConditioncmb.SelectedIndex = 0;
                            BookConditioncmb.Text = condition;
                        }
                        else
                        {
                            BookTitle.Items.Clear();
                            Source.Text = string.Empty;
                            BookConditioncmb.Items.Clear();
                        }
                    }
                }
            }
            else
            {
                BookTitle.Items.Clear();
                Source.Text = string.Empty;
                BookConditioncmb.Items.Clear();
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
                ReturnClientName.Items.Clear();
                ReturnPenalty.Clear();

                if (ReturnRoleComboBox is ComboBox roleCombo)
                    roleCombo.Items.Clear();
                else
                    ReturnRoleComboBox.Text = "";

                ReturnBookQty.Items.Clear();
                ReturnedBookID.Items.Clear();
                ReturnBookStatus.Items.Clear();
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
                        cmdClient.Parameters.AddWithValue("@ClientID", clientID);
                        using (SqlDataReader reader = cmdClient.ExecuteReader())
                        {
                            ReturnClientName.Items.Clear();

                            if (reader.Read())
                            {
                                // 🧍 Fill client name ComboBox
                                string name = reader["Name"].ToString();
                                ReturnClientName.Items.Add(name);
                                ReturnClientName.SelectedIndex = 0;

                                // 🎓 Fill role (Student/Faculty)
                                string role = reader["Role"].ToString();
                                if (ReturnRoleComboBox is ComboBox combo)
                                {
                                    combo.Items.Clear();
                                    combo.Items.Add(role);
                                    combo.SelectedIndex = 0;
                                }
                                else
                                {
                                    ReturnRoleComboBox.Text = role;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Student not found. Please check the Client ID.",
                                                "Not Found",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Warning);

                                ReturnClientName.Items.Clear();
                                if (ReturnRoleComboBox is ComboBox combo)
                                    combo.Items.Clear();
                                else
                                    ReturnRoleComboBox.Text = "";

                                ReturnBookQty.Items.Clear();
                                ReturnedBookID.Items.Clear();
                                ReturnBookStatus.Items.Clear();
                                ReturnPenalty.Clear();
                                CMBbookConditon.Items.Clear();
                                return;
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
                            ReturnBookQty.Items.Clear();
                            ReturnBookQty.Items.Add(bookCount.ToString());
                            ReturnBookQty.SelectedIndex = 0;

                            foreach (var entry in isbnAndTitles)
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
                // Clear all if ClientID too short
                ReturnClientName.Items.Clear();
                ReturnPenalty.Clear();
                if (ReturnRoleComboBox is ComboBox combo)
                    combo.Items.Clear();
                else
                    ReturnRoleComboBox.Text = "";

                ReturnBookQty.Items.Clear();
                ReturnedBookID.Items.Clear();
                ReturnBookStatus.Items.Clear();
                CMBbookConditon.Items.Clear();
            }
        }



        private void ReturnBookQty_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void RoleComboBox_Click(object sender, EventArgs e)
        {

        }
    }
}
