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
    public partial class Return : Form
    {
        public Return()
        {
            InitializeComponent();

            LoadClientIDs();      // replaced IssueID with ClientID
            LoadReturnedBooks();

        }

        private void kryptonTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void kryptonCheckButton1_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
            if (string.IsNullOrWhiteSpace(ClientID.Text))
            {
                MessageBox.Show("Please select a Client ID.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime returnDate = DateTime.Now; // auto return date
            int quantity = int.TryParse(Quantity.Text, out var q) ? q : 1;

            string insertQuery = @"
                INSERT INTO ReturnedBooks
                ( ClientID, ClientName, BookTitle, Source, ReturnDate, Status, Quantity)
                VALUES
                ( @ClientID, @ClientName, @BookTitle, @Source, @ReturnDate, @Status, @Quantity)";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(insertQuery, con);


                    cmd.Parameters.AddWithValue("@ClientID", ClientID.Text);
                    cmd.Parameters.AddWithValue("@ClientName", ClientName.Text);
                    cmd.Parameters.AddWithValue("@BookTitle", BookTitle.Text);
                    cmd.Parameters.AddWithValue("@Source", Source.Text);
                    cmd.Parameters.AddWithValue("@ReturnDate", returnDate);
                    cmd.Parameters.AddWithValue("@Status", Status.Text);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Return record saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadReturnedBooks();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving return record:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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



        private void Source_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        // ✅ Load all ClientIDs from IssueBooks
        private void LoadClientIDs()
        {
            ClientID.Items.Clear();
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT DISTINCT ClientID FROM IssueBooks ORDER BY ClientID";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ClientID.Items.Add(dr["ClientID"].ToString());
                }
            }
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







        private void LoadReturnedBooks() // LOAD THE DATAGRIDDDD
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
            string query = @"
                SELECT 
                    ReturnID,
                  
                    ClientID,
                    ClientName,
                    BookTitle,
                    Source,
                    ReturnDate,
                    Status,
                    Quantity
                FROM ReturnedBooks
                ORDER BY ReturnID DESC";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataGridReturnBooks.DataSource = dt;

                // clean UI
                DataGridReturnBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                DataGridReturnBooks.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                DataGridReturnBooks.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                DataGridReturnBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                DataGridReturnBooks.ReadOnly = true;
                DataGridReturnBooks.MultiSelect = false;
                DataGridReturnBooks.RowHeadersVisible = false;
            }
        }
        private void ClearFields()
        {

            ClientID.Text = "";
            ClientName.Text = "";
            BookTitle.Text = "";
            Source.Text = "";
            IssueDate.Text = "";
            DueDate.Text = "";
            Status.Text = "";
            Quantity.Text = "";

        }



        private void BookTitle_TextChanged(object sender, EventArgs e)
        {

        }

        private void Return_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT ClientID FROM IssueBooks", con);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ClientID.Items.Add(dr["ClientID"].ToString());
                }
                dr.Close();
            }
        }

        private void ClientID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
            string clientId = ClientID.Text.Trim();

            if (string.IsNullOrEmpty(clientId))
                return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
                    SELECT TOP 1
                        ib.IssueID,
                        ib.ClientID,
                        ib.StudentName AS ClientName,
                        ib.BookTitle,
                        ib.Source,
                        ib.IssueDate,
                        ib.DueDate,
                        ib.Status,
                        ib.Quantity,
                        ib.Penalty,
                        ib.OverdueDays
                    FROM IssueBooks ib
                    WHERE ib.ClientID = @ClientID
                    ORDER BY ib.IssueDate DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClientID", clientId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {

                    ClientName.Text = reader["ClientName"].ToString();
                    BookTitle.Text = reader["BookTitle"].ToString();
                    Source.Text = reader["Source"].ToString();
                    IssueDate.Text = Convert.ToDateTime(reader["IssueDate"]).ToString("yyyy-MM-dd");
                    DueDate.Text = Convert.ToDateTime(reader["DueDate"]).ToString("yyyy-MM-dd");
                    Status.Text = reader["Status"].ToString();
                    Quantity.Text = reader["Quantity"].ToString();

                }
                else
                {
                    MessageBox.Show("No issue record found for this Client ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void arthanButton1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            this.Close();
        }
    }
}
