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

        private string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";

        public ActivityLog()
        {
            InitializeComponent();
            LoadLogs();
        }

        private void ActivityLog_Load(object sender, EventArgs e)
        {
            LoadLogs();

        }




        // ✅ Display logs in DataGridView
        // ✅ Loads and displays all recent activity logs
        private void LoadLogs()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT TOP 100 LogID, Timestamp, UserName, Action, Module, Details 
                        FROM ActivityLog 
                        ORDER BY LogID DESC";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        DataGridActivity.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading logs: " + ex.Message,
                                "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // ✅ NEW: Method to record activity (use this anywhere)
        // ✅ Static method to record any activity from anywhere in your system
        // ✅ Static method — can be called anywhere in the system
        public static void RecordActivity(string userName, string action, string module, string details)
        {
            string connectionString =
                @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=LibraryDB;
                  Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        INSERT INTO ActivityLog (Timestamp, UserName, Action, Module, Details) 
                        VALUES (@Timestamp, @UserName, @Action, @Module, @Details)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserName", userName ?? "Unknown");
                        cmd.Parameters.AddWithValue("@Action", action ?? "No Action");
                        cmd.Parameters.AddWithValue("@Module", module ?? "Unknown Module");
                        cmd.Parameters.AddWithValue("@Details", details ?? "No Details");

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Don’t break user flow — just notify silently
                MessageBox.Show("Error recording activity: " + ex.Message,
                                "Logging Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
