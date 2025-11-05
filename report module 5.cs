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
    public partial class report_module_5 : Form
    {
        SqlConnection con = new SqlConnection(@"  Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;
");

        private string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";




        public report_module_5()
        {
            InitializeComponent();
            LoadSummaryData();


            // Default
            comboReportType.Items.AddRange(new string[] { "Daily", "Weekly", "Monthly", "Custom" });
            comboReportType.SelectedIndex = 0;

        }




        private void LoadSummaryData()
        {
            DateTime startDate = dateFrom.Value.Date;
            DateTime endDate = dateTo.Value.Date.AddDays(1).AddSeconds(-1);

            // Get previous period (based on range length)
            TimeSpan rangeLength = endDate - startDate;
            DateTime prevStart = startDate - rangeLength - TimeSpan.FromSeconds(1);
            DateTime prevEnd = startDate - TimeSpan.FromSeconds(1);

            // Current period
            int borrowed = GetCount("IssueBooks", "IssueDate", startDate, endDate, "Status = 'Issued'");
            int returned = GetCount("ReturnedBooks", "ReturnDate", startDate, endDate, "Status = 'Returned'");
            int booksAdded = GetCount("BooksAcq", "DateAdded", startDate, endDate);
            int accounts = GetCount("AddStudentAcc", "DateCreated", startDate, endDate);


            // Previous period
            int borrowedPrev = GetCount("IssueBooks", "IssueDate", prevStart, prevEnd, "Status = 'Issued'");
            int returnedPrev = GetCount("ReturnedBooks", "ReturnDate", prevStart, prevEnd, "Status = 'Returned'");
            int booksAddedPrev = GetCount("BooksAcq", null, prevStart, prevEnd);
            int accountsPrev = GetCount("AddStudentAcc", null, prevStart, prevEnd);

            // Display totals
            lblBooksBorrowed.Text = borrowed.ToString();
            lblBooksReturned.Text = returned.ToString();
            lblBooksAdded.Text = booksAdded.ToString();
            lblAccountsCreated.Text = accounts.ToString();

            // Display growth
            ShowGrowth(lblBorrowedChange, borrowed, borrowedPrev);
            ShowGrowth(lblReturnedChange, returned, returnedPrev);
            ShowGrowth(lblAddedChange, booksAdded, booksAddedPrev);
            ShowGrowth(lblAccountsChange, accounts, accountsPrev);
        }

        private void ShowGrowth(Label lbl, int current, int previous)
        {
            double change = 0;
            if (previous > 0)
                change = ((double)(current - previous) / previous) * 100;
            else if (current > 0)
                change = 100;

            if (change > 0)
            {
                lbl.ForeColor = Color.Green;
                lbl.Text = $"↑ +{change:F1}%";
            }
            else if (change < 0)
            {
                lbl.ForeColor = Color.Red;
                lbl.Text = $"↓ {change:F1}%";
            }
            else
            {
                lbl.ForeColor = Color.Gray;
                lbl.Text = "— 0%";
            }
        }

        private int GetCount(string tableName, string dateColumn, DateTime startDate, DateTime endDate, string extraCondition = null)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = $"SELECT COUNT(*) FROM {tableName}";
                string whereClause = "";

                if (!string.IsNullOrEmpty(dateColumn))
                {
                    whereClause += $"{dateColumn} BETWEEN @startDate AND @endDate";
                }

                if (!string.IsNullOrEmpty(extraCondition))
                {
                    if (whereClause.Length > 0)
                        whereClause += " AND ";
                    whereClause += extraCondition;
                }

                if (whereClause.Length > 0)
                    query += " WHERE " + whereClause;

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(dateColumn))
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);
                    }

                    conn.Open();
                    count = (int)cmd.ExecuteScalar();
                }
            }
            return count;
        }



        private void guna2Button1_Click(object sender, EventArgs e)
        {
            LoadSummaryData();
        }

        private void comboReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string reportType = comboReportType.SelectedItem.ToString();
            DateTime now = DateTime.Now;

            switch (reportType)
            {
                case "Daily":
                    dateFrom.Value = now.Date;
                    dateTo.Value = now.Date;
                    break;

                case "Weekly":
                    int diff = now.DayOfWeek - DayOfWeek.Monday;
                    if (diff < 0) diff += 7;
                    DateTime startOfWeek = now.AddDays(-1 * diff).Date;
                    DateTime endOfWeek = startOfWeek.AddDays(6);
                    dateFrom.Value = startOfWeek;
                    dateTo.Value = endOfWeek;
                    break;

                case "Monthly":
                    DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
                    DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                    dateFrom.Value = startOfMonth;
                    dateTo.Value = endOfMonth;
                    break;

                case "Custom":
                    // user sets manually
                    break;
            }

            LoadSummaryData();

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            report.Show();
            this.Hide();
        }

        private void arthanButton2_Click(object sender, EventArgs e)
        {
            Report report = new Report();
            report.Show();
            this.Hide();
        }
    }
}
