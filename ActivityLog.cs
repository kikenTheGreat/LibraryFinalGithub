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
            StyleDataGrid(DataGridActivity);
        }
        private void StyleDataGrid(DataGridView dgv) // dgv usable method for styling any datagridviewvvvvvvvvvvvvvvvvvvvvvvvvv
        {
            // 🧭 General layout
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.GridColor = Color.LightGray;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            // 🧱 Header style
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 42;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // 📘 Row style — add padding and center vertically
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // ✨ Center vertically + top & bottom padding (8px total)
            dgv.DefaultCellStyle.Padding = new Padding(5, 6, 5, 6); // left, top, right, bottom
            dgv.RowTemplate.Height = 38; // Adjust height for padding

            // 🪶 Alternating row style
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgv.AlternatingRowsDefaultCellStyle.Padding = new Padding(5, 6, 5, 6);

            FormatDataGridView();

            if (dgv is Guna.UI2.WinForms.Guna2DataGridView gunaGrid)
            {
                gunaGrid.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(250, 250, 250);
                gunaGrid.ThemeStyle.RowsStyle.BackColor = Color.White;
                gunaGrid.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(240, 240, 240);
                gunaGrid.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                gunaGrid.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            }


        }

        private void FormatDataGridView()
        {
            // Enable text wrapping for all cells
            DataGridActivity.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Automatically adjust row heights based on content
            DataGridActivity.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Optionally adjust column sizing behavior
            DataGridActivity.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // You can also make only the Details column wrap
            DataGridActivity.Columns["Details"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
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
