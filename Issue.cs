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

            using (SqlConnection con = new SqlConnection(
                "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
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
                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
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

           


        }

        private void BookID_TextChanged(object sender, EventArgs e)
        {
            string bookID = BookID.Text.Trim();

            if (bookID.Length >= 4)
            {
                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
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

            // Fix for CS1061 and CS0165:
            // Assuming 'issueDate' and 'dueDat
            // e' are DateTimePicker controls.
            DateTime issueDateValue = issueDate.Value;
            DateTime dueDateValue = dueDate.Value;

            // Gather book titles and sources from the list
            List<string> bookTitles = new List<string>();
            List<string> bookSources = new List<string>();

            foreach (var item in borrowList)
            {
                bookTitles.Add(item.BookTitle);
                bookSources.Add(item.Source);
            }

            string combinedBookTitles = string.Join(", ", bookTitles);
            string combinedSources = string.Join(", ", bookSources);
            int quantity = bookTitles.Count;

            string query = @"INSERT INTO IssueBooks (Status, StudentName, BookTitle, Source, IssueDate, DueDate, Quantity,ClientID)
                     VALUES (@Status, @StudentName, @BookTitle, @Source, @IssueDate, @DueDate, @Quantity,@ClientID)";

            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Status", Status.Text);
                cmd.Parameters.AddWithValue("@StudentName", ClientName.Text);
                cmd.Parameters.AddWithValue("@BookTitle", combinedBookTitles);
                cmd.Parameters.AddWithValue("@Source", combinedSources); // ✅ Fixed
                cmd.Parameters.AddWithValue("@IssueDate", issueDateValue);
                cmd.Parameters.AddWithValue("@DueDate", dueDateValue);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@ClientID", ClientID.Text);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Book(s) issued successfully!");
            borrowList.Clear();
            dgvBorrowList.Rows.Clear();
            LoadIssueBooks();
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
            Form1 f = new Form1();
            f.Show();
            this.Hide();



        }

        private void arthanButton1_Load(object sender, EventArgs e)
        {

        }
    }
}
