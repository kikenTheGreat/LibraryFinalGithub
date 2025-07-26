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
        }

        private void BookID_SelectedIndexChanged(object sender, EventArgs e)
        {

          

        }
    }
}
