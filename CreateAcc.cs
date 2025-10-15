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
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace LibraryCGC
{
    public partial class CreateAcc : Form
    {
        public CreateAcc()
        {
            InitializeComponent();
            LoadStudentAccounts();
        }

        private void CreateAcc_Load(object sender, EventArgs e)
        {
            //output data grid
            LoadStudentAccounts();
        }

        private void LoadStudentAccounts()   //output data grid
        {
            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"))
            {
                con.Open();

                string query = "SELECT * FROM AddStudentAcc";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    AddStudentAccDataGrid.DataSource = dt;
                }
            }
        }

        private void ClientID_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void arthanButton5_Load(object sender, EventArgs e)
        {


        }

        private void arthanButton5_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            con.Open();

            SqlCommand cmd = new SqlCommand(@"INSERT INTO AddStudentAcc 
(Name,  SectionSY, Email, StudentNumber, Department,Role)
VALUES 
(@Name, @SectionSY, @Email, @StudentNumber, @Department, @Role)", con);

            // Assign parameters from textboxes
            cmd.Parameters.AddWithValue("@Name", Name.Text);
            cmd.Parameters.AddWithValue("@SectionSY", SectionSY.Text);
            cmd.Parameters.AddWithValue("@Email", Email.Text);
            cmd.Parameters.AddWithValue("@StudentNumber", StudentNumber.Text);
            cmd.Parameters.AddWithValue("@Department", Department.Text);
            cmd.Parameters.AddWithValue("@Role", Role.Text);

            cmd.ExecuteNonQuery();
            MessageBox.Show("Student record added successfully!");
            LoadStudentAccounts();       //output data grid
            con.Close();

            // Clear the fields after insert
            Name.Text = "";

            ClientID.Text = "";
            SectionSY.Text = "";
            Email.Text = "";
            StudentNumber.Text = "";
            Department.Text = "";

            Role.Text = "";

        }

        private void AddStudentAccDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void arthanPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
