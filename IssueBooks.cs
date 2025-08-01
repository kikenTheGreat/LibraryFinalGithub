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
            string query1 = "SELECT BookID FROM BooksAcq";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query1, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        BookID.Items.Add(reader["BookID"].ToString());
                    }
                }
            }
            //retrieving the BookID in DATABASE ---CLOSING----



            //retrieving the ClientID in DATABASE ---OPENING----
            string query2 = "SELECT ClientID FROM AddStudentAcc";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query2, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ClientID.Items.Add(reader["ClientID"].ToString());
                    }
                }
            }
            //retrieving the ClientID in DATABASE ---CLOSING----




            //retrieving the IssueID in DATABASE ---OPENING----
            string query3 = "SELECT IssueID FROM IssueBooks";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query3, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ClientID.Items.Add(reader["IssueID"].ToString());
                    }
                }
            }
            //retrieving the IssueID in DATABASE ---CLOSING----




            //add value in combobox STATUS
            Status.Items.Add("Availabe");
            Status.Items.Add("Issued");










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
            string query = @"INSERT INTO IssueBooks (Status, ClientID, BookID, DueDate)
                 VALUES (@Status, @ClientID, @BookID, @DueDate)";

            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Status", Status.Text);

                    // ClientID - Check if selected, otherwise insert NULL
                    if (!string.IsNullOrEmpty(ClientID.Text))
                        cmd.Parameters.AddWithValue("@ClientID", ClientID.Text);
                    else
                        cmd.Parameters.AddWithValue("@ClientID", DBNull.Value);

                    // BookID - Check if selected, otherwise insert NULL
                    if (!string.IsNullOrEmpty(BookID.Text))
                        cmd.Parameters.AddWithValue("@BookID", BookID.Text);
                    else
                        cmd.Parameters.AddWithValue("@BookID", DBNull.Value);

                    // DueDate - Get exact date from DateTimePicker
                   cmd.Parameters.AddWithValue("@DueDate", DueDate.SelectionStart);


                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Issue Book inserted successfully.");
                }
            }

            LoadIssueBooks(); // Refresh DataGridView


        }

        private void LoadIssueBooks()
        {
            string query = @"
        SELECT 
            ib.IssueID,
            sa.ClientID,
            ba.BookID,
            ib.Status,
            ib.DueDate
        FROM IssueBooks ib
        LEFT JOIN AddStudentAcc sa ON ib.ClientID = sa.ClientID
        LEFT JOIN BooksAcq ba ON ib.BookID = ba.BookID";

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
    }
}
