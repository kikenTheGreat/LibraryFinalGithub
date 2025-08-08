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
    public partial class Return : Form
    {
        public Return()
        {
            InitializeComponent();
            LoadIssueIDs();
            LoadReturnedBooks();

        }

        private void kryptonTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void kryptonCheckButton1_Click(object sender, EventArgs e)
        {
            // Parse values
            int issueId;
            int quantity = 1;
            decimal penalty = 0;
            DateTime returnDate;

            if (!int.TryParse(IssueID.Text, out issueId))
            {
                MessageBox.Show("Invalid IssueID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!DateTime.TryParse(IssueDate.Text, out returnDate))
            {
                MessageBox.Show("Invalid Return Date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(Quantity.Text, out quantity))
            {
                quantity = 1; // default to 1 if invalid
            }

          

            string clientId = ClientID.Text.Trim();
            string clientName = ClientName.Text.Trim();
            string bookId = BookID.Text.Trim();
            string bookTitle = BookTitle.Text.Trim();
            string source = Source.Text.Trim();
    
            string status = Status.Text.Trim();

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

            string insertQuery = @"
        INSERT INTO ReturnedBooks
        (IssueID, ClientID, ClientName,  BookTitle, Source, ReturnDate,  Status, Quantity )
        VALUES
        (@IssueID, @ClientID, @ClientName,  @BookTitle, @Source, @ReturnDate, @Status, @Quantity )";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@IssueID", issueId);
                    cmd.Parameters.AddWithValue("@ClientID", clientId);
                    cmd.Parameters.AddWithValue("@ClientName", clientName);
                 
                    cmd.Parameters.AddWithValue("@BookTitle", bookTitle);
                    cmd.Parameters.AddWithValue("@Source", source);
                    cmd.Parameters.AddWithValue("@ReturnDate", returnDate);
                    
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                 
                    con.Open();
                    cmd.ExecuteNonQuery();
                    LoadReturnedBooks();
                    con.Close();


                    MessageBox.Show("Return record saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Optionally, clear your form or refresh your data grid here
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving return record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void kryptonLabel6_Click(object sender, EventArgs e)
        {

        }

        private void kryptonComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

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

        private void BookTitle_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void BookID_TextChanged(object sender, EventArgs e)
        {
            string bookID = BookID.Text.Trim();

            if (bookID.Length >= 4)
            {
                string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                string query = "SELECT BookTitle FROM BooksAcq WHERE BookID = @BookID";

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
                            BookTitle.Text = "";

                            BookTitle.Text = title;


                        }
                        else
                        {
                            BookTitle.Text = "";

                        }
                    }
                }
            }
            else
            {
                // If less than 4 characters, clear the ComboBox
                BookTitle.Text = "";

            }
        }

        private void Source_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void IssueID_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("IssueID SelectedIndexChanged fired");
            if (IssueID.SelectedItem == null)
                return;

            string issueID = IssueID.SelectedItem.ToString();
            Console.WriteLine($"Selected IssueID: {issueID}");

            FillIssueDetails(issueID);

        }

        private void FillIssueDetails(string issueID)
        {


            string query = @"
        SELECT 
            ib.IssueID,
            sa.ClientID,
            sa.Name AS ClientName,
            sa.Role,
            ib.BookTitle,
            ba.BookID,
            ba.Source,
            ib.IssueDate,
            ib.DueDate,
            ib.Status,
            ib.Quantity
        FROM IssueBooks ib
        LEFT JOIN AddStudentAcc sa ON ib.StudentName = sa.Name
        LEFT JOIN BooksAcq ba ON ib.BookTitle = ba.BookTitle
        WHERE ib.IssueID = @IssueID";

            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IssueID", issueID);
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        ClientID.Text = reader["ClientID"].ToString();
                        ClientName.Text = reader["ClientName"].ToString();
                        ClientType.Text = reader["Role"].ToString();

                        BookID.Text = reader["BookID"].ToString();

                        // This is where you set multi-line book titles
                        string rawTitles = reader["BookTitle"].ToString();
                        BookTitle.Text = string.Join(Environment.NewLine,
                            rawTitles.Split(',').Select(t => t.Trim()));





                        Source.Text = reader["Source"].ToString();         // new field you want to display


                        IssueDate.Text = Convert.ToDateTime(reader["IssueDate"]).ToString("yyyy-MM-dd");
                        DueDate.Text = Convert.ToDateTime(reader["DueDate"]).ToString("yyyy-MM-dd");

                        Status.Text = reader["Status"].ToString();

                        Quantity.Text = reader["Quantity"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("IssueID not found.");
                    }
                }
            }
        }



        private void LoadIssueIDs()
        {
            string query = "SELECT IssueID FROM IssueBooks ORDER BY IssueID";

            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    IssueID.Items.Clear();

                    while (reader.Read())
                    {
                        IssueID.Items.Add(reader["IssueID"].ToString());
                    }
                }
            }
        }



        private void LoadReturnedBooks() // LOAD THE DATAGRIDDDD
        {
            string query = @"
    SELECT 
        ReturnID,
        IssueID,
        ClientID,
        ClientName,
        BookTitle,
        Source,
        ReturnDate,
        Status,
        Quantity
    FROM ReturnedBooks
    ORDER BY ReturnID DESC"; // latest returns first

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                DataGridReturnBooks.DataSource = dt;

                DataGridReturnBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                DataGridReturnBooks.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                DataGridReturnBooks.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                DataGridReturnBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DataGridReturnBooks.MultiSelect = false;
                DataGridReturnBooks.ReadOnly = true;
                DataGridReturnBooks.RowHeadersVisible = false;
            }
        }




        private void BookTitle_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
