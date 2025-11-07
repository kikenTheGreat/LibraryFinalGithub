using Library_Final;
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

namespace LibraryCGC
{
    public partial class report_module_4 : Form
    {
        SqlConnection con = new SqlConnection(@"  Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;
");

        private string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";
        public report_module_4()
        {
            InitializeComponent();
        }

        private void report_module_4_Load(object sender, EventArgs e)// most importanttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt
        {


            LoadSummary();

            cmbActivityLevel.Items.AddRange(new string[] { "All", "Low Activity", "Medium Activity", "High Activity" });
            cmbHasOverdue.Items.AddRange(new string[] { "All", "Yes", "No" });
            LoadReturnedBooks();


            StyleDataGrid(dataGridReturnBooks);


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

            if (dgv is Guna.UI2.WinForms.Guna2DataGridView gunaGrid)
            {
                gunaGrid.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(250, 250, 250);
                gunaGrid.ThemeStyle.RowsStyle.BackColor = Color.White;
                gunaGrid.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(240, 240, 240);
                gunaGrid.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                gunaGrid.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            }


        }

        private void LoadReturnedBooks(string studentName = "", string activityLevel = "", string hasOverdue = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Base query
                string query = @"
            SELECT ClientName,
                   COUNT(ReturnID) AS TotalReturns,
                   SUM(CASE WHEN DATEDIFF(DAY, DueDate, ReturnDate) > 0 THEN 1 ELSE 0 END) AS OverdueCount
            FROM ReturnedBooks
            WHERE 1=1";

                // Add filters dynamically
                if (!string.IsNullOrWhiteSpace(studentName))
                    query += " AND ClientName LIKE @ClientName";

                if (hasOverdue == "Yes")
                    query += " AND DATEDIFF(DAY, DueDate, ReturnDate) > 0";
                else if (hasOverdue == "No")
                    query += " AND DATEDIFF(DAY, DueDate, ReturnDate) <= 0";

                query += " GROUP BY ClientName";

                // Execute query
                SqlCommand cmd = new SqlCommand(query, con);
                if (!string.IsNullOrWhiteSpace(studentName))
                    cmd.Parameters.AddWithValue("@ClientName", "%" + studentName + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Add computed "Activity Level" column
                if (!dt.Columns.Contains("ActivityLevel"))
                    dt.Columns.Add("ActivityLevel", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    int total = Convert.ToInt32(row["TotalReturns"]);
                    string level = total >= 5 ? "High Activity"
                                 : total >= 3 ? "Medium Activity"
                                 : "Low Activity";
                    row["ActivityLevel"] = level;
                }

                // Apply activity level filter
                if (!string.IsNullOrEmpty(activityLevel) && activityLevel != "All")
                {
                    var filtered = dt.AsEnumerable()
                        .Where(r => r.Field<string>("ActivityLevel") == activityLevel);
                    if (filtered.Any())
                        dt = filtered.CopyToDataTable();
                    else
                        dt.Rows.Clear();
                }

                dataGridReturnBooks.DataSource = dt;
            }
        }









        private void LoadSummary()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Total Students (this month)
                string totalReturnedQuery = "SELECT COUNT(*) FROM AddStudentAcc";
                SqlCommand cmdReturned = new SqlCommand(totalReturnedQuery, conn);
                lblTotalStudents.Text = cmdReturned.ExecuteScalar().ToString();



                // With Overdue
                string a = "SELECT COUNT(DISTINCT ClientID) FROM IssueBooks  WHERE Penalty > 0";
                SqlCommand b = new SqlCommand(a, conn);
                lblWithOverdue.Text = b.ExecuteScalar().ToString();

                // Active Borrowers
                string ac = "SELECT COUNT(DISTINCT ClientID) FROM IssueBooks WHERE Status = 'Issued'";
                SqlCommand bd = new SqlCommand(ac, conn);
                lblHighActivity.Text = bd.ExecuteScalar().ToString();



            }
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            string name = txtStudentName.Text.Trim();
            string activity = cmbActivityLevel.SelectedItem?.ToString() ?? "";
            string overdue = cmbHasOverdue.SelectedItem?.ToString() ?? "";

            LoadReturnedBooks(name, activity, overdue);
        }



        private void btnReset_Click(object sender, EventArgs e)
        {
            txtStudentName.Clear();
            cmbActivityLevel.SelectedIndex = -1;
            cmbHasOverdue.SelectedIndex = -1;
            LoadReturnedBooks(); // Load all
        }

        private void guna2CustomGradientPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblWithOverdue_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is Report)
                {
                    openForm.Show();
                    this.Hide();
                    return;
                }
            }
 


            Report report = new Report();
            report.ShowDialog();
            this.Hide();
        }
    }
}
