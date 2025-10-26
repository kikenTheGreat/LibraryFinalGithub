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
    public partial class ActivityLog : Form
    {

        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        public ActivityLog()
        {
            InitializeComponent();
            LoadLogs();
        }

        private void ActivityLog_Load(object sender, EventArgs e)
        {
            LoadLogs();

        }


        private void LoadLogs()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT TOP 100 LogID, Timestamp, UserName, Action, Module, Details FROM ActivityLog ORDER BY LogID DESC";
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        DataGridActivity.Invoke((MethodInvoker)delegate
                        {
                            DataGridActivity.DataSource = dt;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading logs: " + ex.Message);
            }
        }

        private void ActivityLog_FormClosing(object sender, FormClosingEventArgs e)
        {

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

        private void DataGridActivity_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
