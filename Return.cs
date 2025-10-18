using LibraryCGC;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.Design.AxImporter;

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
            if (cmbIssueSelector.SelectedItem == null)
            {
                MessageBox.Show("Please select a book to mark as returned.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Extract IssueID from ComboBox text (e.g., "1005 - Book Title (Issued)")
            string selectedItem = cmbIssueSelector.SelectedItem.ToString();
            string issueID = selectedItem.Split('-')[0].Trim();

            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;
        Initial Catalog=LibraryDB;
        Integrated Security=True;
        Encrypt=True;
        Trust Server Certificate=True;"))
            {
                con.Open();

                // ✅ Insert into ReturnedBooks (matches your DB)
                string insertQuery = @"
            INSERT INTO ReturnedBooks (
                IssueID, ClientID, ClientName, ClientType, BookTitle,
                Quantity, Source, IssueDate, DueDate, ReturnDate, Status
            )
            SELECT 
                ib.IssueID,
                ib.ClientID,
                sa.Name AS ClientName,
                sa.Role AS ClientType,
                ib.BookTitle,
                ib.Quantity,
                ib.Source,
                ib.IssueDate,
                ib.DueDate,
                GETDATE() AS ReturnDate,
                'Returned' AS Status
            FROM IssueBooks ib
            LEFT JOIN AddStudentAcc sa ON ib.ClientID = sa.ClientID
            WHERE ib.IssueID = @IssueID;
        ";

                // ✅ Delete from IssueBooks
                string deleteQuery = "DELETE FROM IssueBooks WHERE IssueID = @IssueID;";

                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, con, trans))
                        {
                            insertCmd.Parameters.AddWithValue("@IssueID", issueID);
                            insertCmd.ExecuteNonQuery();
                        }

                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, con, trans))
                        {
                            deleteCmd.Parameters.AddWithValue("@IssueID", issueID);
                            deleteCmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        MessageBox.Show("Book moved to ReturnedBooks successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // ✅ Log to activity panel
                        AddActivityLog($"📗 {ClientName.Text} returned \"{BookTitle.Text}\" on {DateTime.Now:MMM dd, yyyy hh:mm tt}");
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("Error during return process: " + ex.Message);
                    }
                }
            }

            // ✅ Refresh the book list for the selected client
            LoadIssuedBooksForClient(ClientID.Text.Trim());

            // ✅ Check if the client still has any remaining borrowed books
            bool hasRemainingBooks = false;
            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;
        Initial Catalog=LibraryDB;
        Integrated Security=True;
        Encrypt=True;
        Trust Server Certificate=True;"))
            {
                con.Open();
                string checkQuery = @"
            SELECT COUNT(*) 
            FROM IssueBooks
            WHERE ClientID = @ClientID 
              AND (Status = 'Issued' OR Status = 'Overdue')";
                using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ClientID", ClientID.Text.Trim());
                    int remaining = (int)cmd.ExecuteScalar();
                    hasRemainingBooks = remaining > 0;
                }
            }

            // ✅ Refresh dashboard (Form1)
            var form1 = Application.OpenForms["Form1"] as Form1;
            if (form1 != null)
            {
                form1.LoadPenaltyCards();
                form1.UpdateTotalOverdueLabel();
            }

            // ✅ Refresh ClientID dropdown
            LoadClientIDs();

            // ✅ If no books remain, clear all related fields
            if (!hasRemainingBooks)
            {
                ClientID.Text = "";
                ClientName.Text = "";
                Quantity.Text = "";
                ClientType.Text = "";
                Status.Text = "";
                IssueDate.Text = "";
                DueDate.Text = "";
                Source.Text = "";
                BookTitle.Text = "";
                cmbIssueSelector.Items.Clear();
            }

            GlobalEvents.RaiseBorrowedDataChanged();
        }

        // 🧾 Method to add entry in FlowLayoutPanel (activity trail)
        private void AddActivityLog(string message)
        {
            Label logEntry = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.White,
                Text = $"{DateTime.Now:HH:mm:ss} — {message}",
                Padding = new Padding(5)
            };

            // Suppose your FlowLayoutPanel name is "flowActivityPanel"
            flowLayoutPanel2.Controls.Add(logEntry);
            flowLayoutPanel2.ScrollControlIntoView(logEntry); // auto-scroll to bottom
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
                string query = @"
                SELECT DISTINCT ClientID
                FROM IssueBooks
                WHERE Status IN ('Issued', 'Overdue')
                ORDER BY ClientID"; ;

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
                SqlCommand cmd = new SqlCommand(@"
SELECT DISTINCT ClientID
FROM IssueBooks
WHERE Status IN ('Issued', 'Overdue')
ORDER BY ClientID", con);

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
            LoadIssuedBooksForClient(ClientID.Text.Trim());
        }

        private void LoadIssuedBooksForClient(string clientId)
        {
            cmbIssueSelector.Items.Clear(); // clear old items first

            if (string.IsNullOrEmpty(clientId))
                return;

            using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;
        Initial Catalog=LibraryDB;
        Integrated Security=True;
        Encrypt=True;
        Trust Server Certificate=True;"))
            {
                con.Open();

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

                        cmbIssueSelector.Items.Add($"{issueID} - {bookTitle} ({status})");
                    }

                    dr.Close();
                }
            }

            // show message only if user is manually selecting a client
            if (cmbIssueSelector.Items.Count == 0)
            {
                MessageBox.Show("This client has no issued or overdue books to return.",
                    "No Active Borrows", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                cmbIssueSelector.SelectedIndex = 0;
            }
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

        private void cmbIssueSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbIssueSelector.SelectedIndex == -1) return;

            string selectedText = cmbIssueSelector.SelectedItem.ToString();
            string selectedIssueID = selectedText.Split('-')[0].Trim();

            FillIssueDetails(selectedIssueID);

        }
    }
}
