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

            LoadClientIDs();

        }

        private void kryptonTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        // ✅ Automatically mark books as "Overdue" if their due date has passed
        private void UpdateOverdueBooks()
        {
            using (SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;

"))
            {
                con.Open();

                string query = @"UPDATE IssueBooks
                         SET Status = 'Overdue'
                         WHERE Status = 'Issued' AND DueDate < GETDATE()";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    int affected = cmd.ExecuteNonQuery();

                    if (affected > 0)
                    {
                        Console.WriteLine($"{affected} book(s) marked as Overdue.");
                    }
                }
            }
        }


        private void kryptonCheckButton1_Click(object sender, EventArgs e)
        {
            if (cmbIssueSelector.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a book to return first.");
                return;
            }

            // Extract IssueID from ComboBox text
            string selectedText = cmbIssueSelector.SelectedItem.ToString();
            string selectedIssueID = selectedText.Split('-')[0].Trim();

            using (SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;

"))
            {
                con.Open();
                SqlTransaction tx = con.BeginTransaction();

                try
                {
                    // 1️⃣ Insert into ReturnedBooks
                    string insertQuery = @"INSERT INTO ReturnedBooks (ClientID, ClientName, BookTitle, Source, ReturnDate, Status, Quantity)
                                   VALUES (@ClientID, @ClientName, @BookTitle, @Source, @ReturnDate, @Status, @Quantity)";
                    using (SqlCommand cmdInsert = new SqlCommand(insertQuery, con, tx))
                    {
                        cmdInsert.Parameters.AddWithValue("@ClientID", Convert.ToInt32(ClientID.Text));
                        cmdInsert.Parameters.AddWithValue("@ClientName", ClientName.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@BookTitle", BookTitle.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@Source", Source.Text.Trim());
                        cmdInsert.Parameters.AddWithValue("@ReturnDate", DateTime.Now);
                        cmdInsert.Parameters.AddWithValue("@Status", "Returned");
                        cmdInsert.Parameters.AddWithValue("@Quantity", Convert.ToInt32(Quantity.Text));
                        cmdInsert.ExecuteNonQuery();
                    }

                    // 2️⃣ Update IssueBooks to Returned
                    string updateQuery = @"UPDATE IssueBooks
                                   SET Status = 'Returned'
                                   WHERE IssueID = @IssueID
                                   AND (Status = 'Issued' OR Status = 'Overdue')";
                    using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, con, tx))
                    {
                        cmdUpdate.Parameters.AddWithValue("@IssueID", Convert.ToInt32(selectedIssueID));
                        int rowsAffected = cmdUpdate.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("No issued record found or already returned.");
                        }
                    }

                    tx.Commit();
                    MessageBox.Show("Book successfully returned and status updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    MessageBox.Show("Error processing return: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";
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
            string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";
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

            string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";

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







       
      



        private void BookTitle_TextChanged(object sender, EventArgs e)
        {

        }

        private void Return_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;
"))
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

            UpdateOverdueBooks();

        }
        // ✅ This version loads all books that the selected client still has borrowed
        private void ClientID_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbIssueSelector.Items.Clear(); // clear old items first

            string clientId = ClientID.Text.Trim();
            if (string.IsNullOrEmpty(clientId))
                return;

            using (SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;
"))
            {
                con.Open();

                // Get all books with "Issued" or "Overdue" status for this client
                string query = @"
            SELECT IssueID, BookTitle, Status
            FROM IssueBooks
            WHERE ClientID = @ClientID
            AND (Status = 'Issued' OR Status = 'Overdue')
            ORDER BY IssueDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ClientID", clientId);
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        string issueID = dr["IssueID"].ToString();
                        string bookTitle = dr["BookTitle"].ToString();
                        string status = dr["Status"].ToString();

                        // Show IssueID and BookTitle (plus status for clarity)
                        cmbIssueSelector.Items.Add($"{issueID} - {bookTitle} ({status})");
                    }

                    if (cmbIssueSelector.Items.Count == 0)
                    {
                        MessageBox.Show("This client has no issued or overdue books to return.",
                            "No Active Borrows", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        cmbIssueSelector.SelectedIndex = 0; // auto-select the first item
                    }
                }
            }
        }


        private void arthanButton1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            this.Close();
        }

        private void cmbIssueSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbIssueSelector.SelectedIndex == -1) return;

            string selectedText = cmbIssueSelector.SelectedItem.ToString();
            string selectedIssueID = selectedText.Split('-')[0].Trim();

            FillIssueDetails(selectedIssueID);

        }
    }
}
