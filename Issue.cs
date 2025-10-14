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
        }
        private List<(string BookID, string BookTitle)> borrowList = new List<(string, string)>();

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
            Quantity
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
                            // ✅ Retrieve BookTitle
                            string title = reader["BookTitle"].ToString();
                            BookTitle.Items.Clear();
                            BookTitle.Items.Add(title);
                            BookTitle.SelectedIndex = 0;
                            BookTitle.SelectedItem = title;
                            BookTitle.Text = title;  // ensures it’s not blank

                            // ✅ Retrieve Source
                            string src = reader["Source"].ToString();
                            Source.Items.Clear();
                            Source.Items.Add(src);
                            Source.SelectedIndex = 0;
                            Source.SelectedItem = src;
                            Source.Text = src;  // ensures it’s not blank
                        }
                        else
                        {
                            // Clear controls if no match found
                            BookTitle.Items.Clear();
                            Source.Items.Clear();
                        }
                    }
                }
            }
            else
            {
                // Clear ComboBoxes if less than 4 characters
                BookTitle.Items.Clear();
                Source.Items.Clear();
            }
        }


        private void btnAddToList_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BookID.Text) || string.IsNullOrWhiteSpace(BookTitle.Text))
            {
                MessageBox.Show("Please select a valid book.");
                return;
            }

            borrowList.Add((BookID.Text, kupal.Text));

            dgvBorrowList.Rows.Add(BookID.Text, BookTitle.Text);

            // Clear for next entry
            BookID.Clear();
            kupal.Text = "";
        }

        private void btnConfirmBorrow_Click_1(object sender, EventArgs e)
        {
          


            if (borrowList.Count == 0)
            {
                MessageBox.Show("No books selected to borrow.");
                return;
            }

            DateTime issueDate = IssueDate.Value;
            DateTime dueDate = DueDate.Value;

            List<string> bookTitles = new List<string>();
            foreach (DataGridViewRow row in dgvBorrowList.Rows)
            {
                if (row.Cells["BookTitle"].Value != null)
                    bookTitles.Add(row.Cells["BookTitle"].Value.ToString());
            }

            string combinedBookTitles = string.Join(", ", bookTitles);
            int quantity = bookTitles.Count;

            string query = @"INSERT INTO IssueBooks (Status, StudentName, BookTitle, Source, IssueDate, DueDate, Quantity)
                     VALUES (@Status, @StudentName, @BookTitle, @Source, @IssueDate, @DueDate, @Quantity)";

            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Status", Status.Text);
                    cmd.Parameters.AddWithValue("@StudentName", ClientName.Text);
                    cmd.Parameters.AddWithValue("@BookTitle", combinedBookTitles);
                    cmd.Parameters.AddWithValue("@Source", Source.Text);
                    cmd.Parameters.AddWithValue("@IssueDate", issueDate);
                    cmd.Parameters.AddWithValue("@DueDate", dueDate);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            MessageBox.Show("Book(s) issued successfully!");
            borrowList.Clear();
            dgvBorrowList.Rows.Clear();
            LoadIssueBooks();
        }
    }
}
