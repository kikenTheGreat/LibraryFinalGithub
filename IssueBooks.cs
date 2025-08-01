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
            //retrieving the BookID in DATABASE ---OPENING----
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
            string query1 = "SELECT BookTitle FROM BooksAcq";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query1, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        BookID.Items.Add(reader["BookTitle"].ToString());
                    }
                }
            }
            //retrieving the BookID in DATABASE ---CLOSING----



            //retrieving the ClientID in DATABASE ---OPENING----
            string query2 = "SELECT Name FROM AddStudentAcc";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query2, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ClientID.Items.Add(reader["Name"].ToString());
                    }
                }
            }
            //retrieving the ClientID in DATABASE ---CLOSING----








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
                    cmd.Parameters.AddWithValue("@StudentName", ClientID.Text);
                    cmd.Parameters.AddWithValue("@BookTitle", BookID.Text);
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
    }
}
