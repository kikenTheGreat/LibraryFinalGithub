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
    public partial class Report : Form
    {
        SqlConnection con = new SqlConnection(@"  Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;
");

        private string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";

        public Report()
        {
            InitializeComponent();
        }

        private void BorrowedBUTTON_Click(object sender, EventArgs e)
        {
            BorrowedPANEL.Visible = true;
            ReturnedPANEL.Visible = false;
        }

        private void Report_Load(object sender, EventArgs e)//most importantttttttttttttttttttttttttttttttttttttttttt
        {
            //live data
            lblTotalReturned.Text = GlobalEvents.GetTotalReturnedBooks().ToString();
            lblWithPenalties.Text = GlobalEvents.GetStudentsWithPenalties().ToString();
            lblTotalPenalties.Text = "₱" + GlobalEvents.GetTotalPenalties().ToString("N2");



            dtpStart.Value = DateTime.Now;
            dtpEnd.Value = DateTime.Now;

            BorrowedPANEL.Visible = true;
            ReturnedPANEL.Visible = false;
            LoadAllBorrowedBooks();
            LoadAllReturnedBooks();
            StyleDataGrid(dgvBorrowedBooks);
            StyleDataGrid(dgvReturnedBooks);
            //StyleDataGrid(dgvOverdueBooks);
            // StyleDataGrid(dgvFacultyBorrow);
            //StyleDataGrid(dgvStudentBorrow);

            StyleDataGrid(dgvBorrowedBooks);
        }

        private void LoadAllBorrowedBooks()
        {
            try
            {
                con.Open();
                string query = "SELECT * FROM IssueBooks";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvBorrowedBooks.DataSource = dt;
            }
            finally
            {
                con.Close();
            }
        }

        private void LoadAllReturnedBooks()
        {
            try
            {
                con.Open();
                string query = "SELECT * FROM ReturnedBooks";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvReturnedBooks.DataSource = dt;
            }
            finally
            {
                con.Close();
            }
        }



        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            if (rbAllBorrowed.Checked)
            {
                LoadAllBorrowedBooks();
            }
            else if (rbOverdue.Checked)
            {
                LoadOverdueBooks();
            }
            else if (rbDateRange.Checked)
            {
                LoadBorrowedWithinDateRange();
            }
        }

        private void LoadOverdueBooks()
        {
            try
            {
                con.Open();
                string query = "SELECT * FROM IssueBooks WHERE DueDate < GETDATE()";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvBorrowedBooks.DataSource = dt;
            }
            finally
            {
                con.Close();
            }
        }

        private void LoadBorrowedWithinDateRange()
        {
            try
            {
                con.Open();
                string query = "SELECT * FROM IssueBooks WHERE IssueDate BETWEEN @start AND @end";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@start", dtpStart.Value.Date);
                cmd.Parameters.AddWithValue("@end", dtpEnd.Value.Date);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvBorrowedBooks.DataSource = dt;
            }
            finally
            {
                con.Close();
            }
        }



        private void Decoy_Click(object sender, EventArgs e)
        {
            ReturnedPANEL.Visible = true;
            BorrowedPANEL.Visible = false;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
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
        private void guna2Button1_Click(object sender, EventArgs e) // button for applying filters on returned books
        {
            try
            {
                con.Open();

                // Base query - filter by date range
                string query = "SELECT * FROM ReturnedBooks WHERE ReturnDate BETWEEN @start AND @end";

                // Filter by ClientName if text is entered
                if (!string.IsNullOrEmpty(txtStudentName.Text))
                {
                    query += " AND ClientName LIKE @clientName";
                }

                // Filter by Status if combobox has a value
               

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@start", dtpReturnStart.Value.Date);
                cmd.Parameters.AddWithValue("@end", dtpReturnEnd.Value.Date);

                // Add client name parameter if used
                if (!string.IsNullOrEmpty(txtStudentName.Text))
                {
                    cmd.Parameters.AddWithValue("@clientName", "%" + txtStudentName.Text + "%");
                }

            

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvReturnedBooks.DataSource = dt;
            }
            finally
            {
                con.Close();
            }
        }




        private void btnReset_Click(object sender, EventArgs e)
        {
            txtStudentName.Clear();
            
            dtpReturnStart.Value = DateTime.Now;
            dtpReturnEnd.Value = DateTime.Now;
            LoadAllReturnedBooks();
        }

        private void btnSearchName_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                // If the textbox is empty, show all data
                string query;

                if (string.IsNullOrEmpty(txtStudentName.Text))
                {
                    query = "SELECT * FROM ReturnedBooks";
                }
                else
                {
                    query = "SELECT * FROM ReturnedBooks WHERE ClientName LIKE @clientName";
                }

                SqlCommand cmd = new SqlCommand(query, con);

                if (!string.IsNullOrEmpty(txtStudentName.Text))
                {
                    cmd.Parameters.AddWithValue("@clientName", "%" + txtStudentName.Text + "%");
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvReturnedBooks.DataSource = dt;

                // Optional: show a message if no results
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No records found for that name.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                con.Close();
            }
        }

        private void txtStudentName_TextChanged(object sender, EventArgs e)
        {
            btnSearchName_Click(sender, e);
        }


        public void UpdateTotalReturnsLabel()
        {

            SqlConnection con = new SqlConnection(@"  Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;

");
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ReturnedBooks", con);
                int totalarchived = (int)cmd.ExecuteScalar();
                lblTotalReturned.Text = totalarchived.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
















    }
}
