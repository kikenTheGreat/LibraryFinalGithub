using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Library_Final
{
    public partial class AddStudentAcc : Form
    {
        public AddStudentAcc()
        {
            InitializeComponent();
        }

        private void kryptonLabel1_Click(object sender, EventArgs e)
        {

        }

        private void AddStudentAcc_Load(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            con.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM AddStudentAcc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            AddStudentAccDataGrid.DataSource = dt;
        }

        private void kryptonButton5_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = new Dashboard();
            dashboard.ShowDialog();
            this.Close();
        }

        private void kryptonLabel3_Click(object sender, EventArgs e)
        {

        }

        private void kryptonLabel4_Click(object sender, EventArgs e)
        {

        }

        private void kryptonLabel6_Click(object sender, EventArgs e)
        {

        }

        private void kryptonLabel9_Click(object sender, EventArgs e)
        {

        }

        private void AddAccButton_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            con.Open();

            SqlCommand cmd = new SqlCommand(@"INSERT INTO AddStudentAcc 
(Name, YearLevel, ClientID, SectionSY, Email, StudentNumber, Department, Semester, Role)
VALUES 
(@Name, @YearLevel, @ClientID, @SectionSY, @Email, @StudentNumber, @Department, @Semester, @Role)", con);

            // Assign parameters from textboxes
            cmd.Parameters.AddWithValue("@Name", Name.Text);
            cmd.Parameters.AddWithValue("@YearLevel", YearLevel.Text);
            cmd.Parameters.AddWithValue("@ClientID", ClientID.Text);
            cmd.Parameters.AddWithValue("@SectionSY", SectionSY.Text);
            cmd.Parameters.AddWithValue("@Email", Email.Text);
            cmd.Parameters.AddWithValue("@StudentNumber", StudentNumber.Text);
            cmd.Parameters.AddWithValue("@Department", Department.Text);
            cmd.Parameters.AddWithValue("@Semester", Semester.Text);
            cmd.Parameters.AddWithValue("@Role", Role.Text);

            cmd.ExecuteNonQuery();
            MessageBox.Show("Student record added successfully!");
            con.Close();

            // Clear the fields after insert
            Name.Text = "";
            YearLevel.Text = "";
            ClientID.Text = "";
            SectionSY.Text = "";
            Email.Text = "";
            StudentNumber.Text = "";
            Department.Text = "";
            Semester.Text = "";
            Role.Text = "";



        }

        private void kryptonDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void kryptonLabel11_Click(object sender, EventArgs e)
        {

        }
    }
}
