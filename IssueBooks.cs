using Microsoft.Data.SqlClient;
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
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Library_Final
{
    public partial class BorrowBooks : Form
    {

        public BorrowBooks()
        {
            InitializeComponent();

            LoadIssueBooks(); // Refresh DataGridView
        }




        private void kryptonTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void kryptonCheckButton1_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.ShowDialog();
            this.Close();
        }



        private void BorrowBooks_Load(object sender, EventArgs e)
        {












            //add value in combobox STATUS
            Status.Items.Add("Issued");
            Status.SelectedIndex = 0;











        }




        private void BookID_SelectedIndexChanged(object sender, EventArgs e)
        {



        }

        private void ClientID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Status_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void kryptonButton9_Click(object sender, EventArgs e)
        {







        }

        private void IssueBooksDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {





        }

        private void kryptonButton6_Click(object sender, EventArgs e)
        {
            DateTime issueDate = IssueDate.SelectionStart;
            DateTime dueDate = DueDate.SelectionStart;
            DateTime today = DateTime.Now;

            int overdueDays = 0;
            decimal penalty = 0;

            if (today > dueDate)
            {
                overdueDays = (today - dueDate).Days;
                penalty = overdueDays * 5; // ₱5 per day
            }

            string query = @"INSERT INTO IssueBooks (Status, StudentName, BookTitle, IssueDate, DueDate)
                     VALUES (@Status, @StudentName, @BookTitle, @IssueDate, @DueDate)";

            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Status", Status.Text);
                    cmd.Parameters.AddWithValue("@StudentName", ClientName.Text);
                    cmd.Parameters.AddWithValue("@BookTitle", BookTitle.Text);
                    cmd.Parameters.AddWithValue("@IssueDate", issueDate);
                    cmd.Parameters.AddWithValue("@DueDate", dueDate);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            MessageBox.Show("Issue Book inserted successfully.");

            LoadIssueBooks(); // Refresh DataGridView
        }


        private void LoadIssueBooks()
        {
            string query = @"SELECT IssueID, StudentName, BookTitle, IssueDate, DueDate,OverdueDays,Penalty, Status FROM IssueBooks";
            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                IssueBooksDataGrid.DataSource = dt;
            }
        }



        private void DueDate_DateChanged(object sender, DateRangeEventArgs e)
        {

        }

        private void kryptonLabel3_Click(object sender, EventArgs e)
        {

        }

        private void kryptonLabel4_Click(object sender, EventArgs e)
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
                            BookTitle.Items.Clear();
                            BookTitle.Items.Add(title);
                            BookTitle.SelectedIndex = 0;
                        }
                        else
                        {
                            BookTitle.Items.Clear();
                        }
                    }
                }
            }
            else
            {
                // If less than 4 characters, clear the ComboBox
                BookTitle.Items.Clear();
            }
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



    }
}
