using LibraryCGC;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Final
{
    public partial class NotificationDashboard : Form
    {
        string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;";

        public NotificationDashboard()
        {
            InitializeComponent();
            StyleDataGrid(dgvEmailLog);
            LoadEmailHistory();

            LoadStatistics();
        }
        private void StyleDataGrid(DataGridView dgv)
        {
            // 🧭 General layout
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.GridColor = Color.LightGray;

            // ✅ Enable wrapping + auto row height
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // 🧱 Header style
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 220, 130); // Light yellow header
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 42;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            // 📘 Default row style
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 220); // Light yellow rows
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240); // Light gray for selection
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.DefaultCellStyle.Padding = new Padding(5, 6, 5, 6);

            // 🪶 Alternating row style (light beige)
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 245, 180);
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.Black;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 240, 240); // Same gray
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.AlternatingRowsDefaultCellStyle.Padding = new Padding(5, 6, 5, 6);

            // 🔧 Row height
            dgv.RowTemplate.Height = 38;

            // 🧩 Apply theme if Guna2 grid
            if (dgv is Guna.UI2.WinForms.Guna2DataGridView gunaGrid)
            {
                gunaGrid.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(255, 245, 180);
                gunaGrid.ThemeStyle.RowsStyle.BackColor = Color.FromArgb(255, 255, 220);
                gunaGrid.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(250, 220, 130);
                gunaGrid.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                gunaGrid.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(240, 240, 240);
            }

            dgv.Refresh();
        }




        private void LoadEmailHistory()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            LogID AS 'ID',
                            ClientID AS 'Client ID',
                            RecipientName AS 'Name',
                            RecipientEmail AS 'Email',
                            NotificationType AS 'Type',
                            Subject,
                            Status,
                            SentDate AS 'Sent Date',
                            BookTitle AS 'Book'
                        FROM EmailNotificationLog
                        WHERE CAST(SentDate AS DATE) BETWEEN @From AND @To
                        ORDER BY SentDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@From", dtpFrom.Value.Date);
                        cmd.Parameters.AddWithValue("@To", dtpTo.Value.Date);


                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvEmailLog.DataSource = dt;

                        // Update label to show count for the filter period
                        int filteredCount = dt.Rows.Count;
                        lblFilteredCount.Text = $"Emails Sent from {dtpFrom.Value:MMMM dd, yyyy} to {dtpTo.Value:MMMM dd, yyyy}: = {filteredCount}";

                    }
                }

                // Color code status
                foreach (DataGridViewRow row in dgvEmailLog.Rows)
                {
                    if (row.Cells["Status"].Value?.ToString() == "Success")
                    {
                        row.Cells["Status"].Style.BackColor = Color.LightGreen;
                        row.Cells["Status"].Style.ForeColor = Color.DarkGreen;
                    }
                    else
                    {
                        row.Cells["Status"].Style.BackColor = Color.LightCoral;
                        row.Cells["Status"].Style.ForeColor = Color.DarkRed;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading email history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadStatistics()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Email count only
                    string emailQuery = "SELECT COUNT(*) FROM EmailNotificationLog WHERE Status = 'Success'";
                    using (SqlCommand cmd = new SqlCommand(emailQuery, con))
                    {
                        int emailCount = (int)cmd.ExecuteScalar();
                        lblEmailCount.Text = $"Total Emails Sent: {emailCount}";
                    }

                    // Last run time
                    string lastRunQuery = "SELECT TOP 1 SentDate FROM EmailNotificationLog ORDER BY SentDate DESC";
                    using (SqlCommand cmd = new SqlCommand(lastRunQuery, con))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            DateTime lastRun = Convert.ToDateTime(result);
                            lblLastRun.Text = $"Last Notification Run: {lastRun:MMMM dd, yyyy hh:mm tt}";
                        }
                        else
                        {
                            lblLastRun.Text = "Last Notification Run: N/A";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading statistics: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NotificationDashboard_Load(object sender, EventArgs e)
        {
            LoadEmailHistory();
            LoadStatistics();
           
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEmailHistory();
            LoadStatistics();
            MessageBox.Show("Dashboard refreshed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void btnSendNow_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
               "Send all pending email notifications now?\n\n" +
               "This will send:\n" +
               "• Due date reminders\n" +
               "• Overdue notices\n" +
               "• Weekly notifications\n\n" +
               "Continue?",
               "Confirm Send",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question
           );

            if (result == DialogResult.Yes)
            {
                btnSendNow.Enabled = false;
                btnSendNow.Text = "⏳ Sending...";
                Cursor = Cursors.WaitCursor;

                await Task.Run(() =>
                {
                    EmailNotificationService.CheckAndSendAllNotifications();
                });

                Cursor = Cursors.Default;
                btnSendNow.Enabled = true;
                btnSendNow.Text = "📤 Send Notifications Now";

                LoadEmailHistory();
                LoadStatistics();

                MessageBox.Show("Email notifications sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "CSV File|*.csv",
                    FileName = $"Email_Notification_Log_{DateTime.Now:yyyyMMdd}.csv"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(saveDialog.FileName))
                    {
                        // Headers
                        for (int i = 0; i < dgvEmailLog.Columns.Count; i++)
                        {
                            writer.Write(dgvEmailLog.Columns[i].HeaderText);
                            if (i < dgvEmailLog.Columns.Count - 1)
                                writer.Write(",");
                        }
                        writer.WriteLine();

                        // Rows
                        foreach (DataGridViewRow row in dgvEmailLog.Rows)
                        {
                            for (int i = 0; i < dgvEmailLog.Columns.Count; i++)
                            {
                                writer.Write(row.Cells[i].Value?.ToString() ?? "");
                                if (i < dgvEmailLog.Columns.Count - 1)
                                    writer.Write(",");
                            }
                            writer.WriteLine();
                        }
                    }

                    MessageBox.Show("Export completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvEmailLog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
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

            Form1 form1 = new Form1(SessionData.CurrentEmployeeID);
            form1.Show();
            this.Hide();
        }
    }
}
