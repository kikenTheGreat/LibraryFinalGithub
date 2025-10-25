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

            panelReturnBooks.Visible = false;
            ReturnPANEL.Visible = false;






            // Add multiple items at once
            BookCondition.Items.AddRange(new string[] { "Good", "Damaged", "Minor Damaged", "Lost" });
            BookCondition.SelectedIndex = 0;

            LoadReturnedBooks();

            issueDate.Value = DateTime.Now;
            issueDate.Format = DateTimePickerFormat.Custom;
            issueDate.CustomFormat = "dddd, MMMM dd, yyyy"; // Example: Friday, October 25, 2025

            StartDateTimeUpdater(); // ✅ Live update the Issue Date picker


        }



        private void StartDateTimeUpdater()
        {
            // ✅ Use Windows Forms Timer
            System.Windows.Forms.Timer dateTimer = new System.Windows.Forms.Timer();
            dateTimer.Interval = 1000; // every 1 second
            dateTimer.Tick += (s, e) =>
            {
                // ⏰ Update Issue Date to current date and time
                issueDate.Value = DateTime.Now;

                // 📅 Automatically set Due Date to 7 days after Issue Date
                dueDate.Value = DateTime.Now.AddDays(3);
            };
            dateTimer.Start();

            // ✅ Format display for both date pickers
            issueDate.Format = DateTimePickerFormat.Custom;
            issueDate.CustomFormat = "dddd, MMMM dd, yyyy hh:mm tt"; // Example: Friday, October 25, 2025 06:45 PM

            dueDate.Format = DateTimePickerFormat.Custom;
            dueDate.CustomFormat = "dddd, MMMM dd, yyyy hh:mm tt";   // Example: Friday, November 1, 2025 06:45 PM
        }










        private void BookID_TextChanged(object sender, EventArgs e)
        {
            string bookID = ISBN.Text.Trim();

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
            DateTime dueDateValue = dueDate.Value;

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
                    }

                    MessageBox.Show("Book(s) issued successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    ISBN,
    BookTitle,
    Status,
    Penalty,
    Quantity,
    IssueID,
    Source,
    ClientID
FROM IssueBooks
WHERE ClientID = @ClientID
  AND (Status = 'Issued' OR Status = 'Overdue')
";

                    using (SqlCommand cmdBorrow = new SqlCommand(queryBorrow, con))
                    {
                        cmdBorrow.Parameters.AddWithValue("@ClientID", clientID);

                        using (SqlDataReader reader = cmdBorrow.ExecuteReader())
                        {
                            // Prepare holders
                            int bookCount = 0;
                            double totalPenalty = 0;
                            List<string> isbnAndTitles = new List<string>();
                            List<string> statuses = new List<string>();

                            // ✅ Clear combo boxes before filling
                            ReturnedBookID.Items.Clear();

                            while (reader.Read())
                            {
                                bookCount++;

                                // ✅ Safe read (avoid null values)
                                string isbn = reader["ISBN"] != DBNull.Value ? reader["ISBN"].ToString() : "N/A";
                                string bookTitle = reader["BookTitle"] != DBNull.Value ? reader["BookTitle"].ToString() : "Unknown";

                                // Combine ISBN and Title for display
                                isbnAndTitles.Add($"{isbn} - {bookTitle}");
                                statuses.Add(reader["Status"].ToString());

                                if (double.TryParse(reader["Penalty"].ToString(), out double penalty))
                                    totalPenalty += penalty;
                            }

                            // ✅ Fill ComboBoxes and TextBox
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
            string isbn = selectedBook.Split('-')[0].Trim(); // ✅ Use ISBN now
            string clientID = ReturnClientID.Text.Trim();
            string clientName = ReturnClientName.Text;
            string clientType = "Student"; // or another dropdown if available
            string bookTitle = selectedBook.Split('-').Length > 1 ? selectedBook.Split('-')[1].Trim() : "Unknown";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // ✅ 1️⃣ Get Issue details
                    int issueID = 0;
                    DateTime issueDate = DateTime.Now;
                    DateTime dueDate = DateTime.Now;
                    string source = "";

                    string getIssueQuery = @"
                SELECT TOP 1 IssueID, IssueDate, DueDate, Source
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
                            }
                            else
                            {
                                MessageBox.Show("No matching issued book found for return.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }

                    // ✅ 2️⃣ Move from IssueBooks → ReturnedBooks, then delete from IssueBooks
                    string moveQuery = @"
                INSERT INTO ReturnedBooks 
                    (IssueID, ClientID, ClientName, ClientType, BookTitle, Quantity, Source, IssueDate, DueDate, ReturnDate, Status)
                SELECT 
                    IssueID, 
                    ClientID, 
                    StudentName AS ClientName, 
                    @ClientType AS ClientType, 
                    BookTitle, 
                    Quantity, 
                    Source, 
                    IssueDate, 
                    DueDate, 
                    GETDATE() AS ReturnDate, 
                    'Returned' AS Status
                FROM IssueBooks
                WHERE IssueID = @IssueID;

                DELETE FROM IssueBooks
                WHERE IssueID = @IssueID;";

                    using (SqlCommand cmdMove = new SqlCommand(moveQuery, con, transaction))
                    {
                        cmdMove.Parameters.AddWithValue("@IssueID", issueID);
                        cmdMove.Parameters.AddWithValue("@ClientType", clientType);
                        cmdMove.ExecuteNonQuery();
                    }

                    // ✅ 3️⃣ Increase quantity in BooksAcq
                    string updateQtyQuery = "UPDATE BooksAcq SET Quantity = Quantity + 1 WHERE ISBN = @ISBN";
                    using (SqlCommand updateCmd = new SqlCommand(updateQtyQuery, con, transaction))
                    {
                        updateCmd.Parameters.AddWithValue("@ISBN", isbn);
                        updateCmd.ExecuteNonQuery();
                    }

                    // ✅ 4️⃣ Commit changes
                    transaction.Commit();

                    MessageBox.Show("Book returned successfully! Quantity updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblReturnDate.Text = $"Returned on: {DateTime.Now:MMMM dd, yyyy hh:mm tt}";

                    // Clear input fields
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

            // ✅ 5️⃣ Refresh grids and events
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

        private void ISBN_TextChanged(object sender, EventArgs e)
        {
            string isbn = ISBN.Text.Trim();

            if (isbn.Length >= 4)
            {
                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;";
                string query = "SELECT BookTitle, Source FROM BooksAcq WHERE ISBN = @ISBN";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ISBN", isbn);

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string title = reader["BookTitle"].ToString();
                            BookTitle.Items.Clear();
                            BookTitle.Items.Add(title);
                            BookTitle.SelectedIndex = 0;
                            BookTitle.Text = title;

                            string src = reader["Source"].ToString();
                            Source.Text = src;
                        }
                        else
                        {
                            BookTitle.Items.Clear();
                            Source.Text = string.Empty;
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


    }
}
