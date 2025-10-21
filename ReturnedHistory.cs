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
    public partial class ReturnedHistory : Form
    {
        public ReturnedHistory()
        {
            InitializeComponent();
            LoadBooksGrid();
        }




        private void LoadBooksGrid()          //output the datagrid 
        {
            using (SqlConnection con = new SqlConnection(" Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n"))
            {
                string query = "SELECT * FROM ReturnedBooks";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataGridReturnBooks.DataSource = dt;

                // Scroll to top
                if (DataGridReturnBooks.Rows.Count > 0)
                {
                    DataGridReturnBooks.FirstDisplayedScrollingRowIndex = 0;
                    DataGridReturnBooks.ClearSelection(); // Optional
                }



                DataGridReturnBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
        }

        private void DataGridReturnBooks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void arthanButton2_Click(object sender, EventArgs e)
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

        private void arthanButton2_Load(object sender, EventArgs e)
        {

        }

        private void ReturnedHistory_Load(object sender, EventArgs e)
        {
            SetupReturnBooksGrid();
            LoadBooksGrid();
        }

        private void SetupReturnBooksGrid()
        {
            DataGridReturnBooks.Columns.Clear();
            DataGridReturnBooks.AutoGenerateColumns = false;
            DataGridReturnBooks.ReadOnly = true;
            DataGridReturnBooks.RowHeadersVisible = false;
            DataGridReturnBooks.BorderStyle = BorderStyle.None;
            DataGridReturnBooks.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            DataGridReturnBooks.EnableHeadersVisualStyles = false;

            // --- Return ID (hidden) ---
            var colReturnID = new DataGridViewTextBoxColumn();
            colReturnID.HeaderText = "Return ID";
            colReturnID.DataPropertyName = "ReturnID";
            colReturnID.Name = "ReturnID";
            colReturnID.Visible = false;
            DataGridReturnBooks.Columns.Add(colReturnID);

            // --- Issue ID ---
            var colIssueID = new DataGridViewTextBoxColumn();
            colIssueID.HeaderText = "Issue ID";
            colIssueID.DataPropertyName = "IssueID";
            colIssueID.Name = "IssueID";
            colIssueID.Width = 100;
            DataGridReturnBooks.Columns.Add(colIssueID);

            // --- Client ID ---
            var colClientID = new DataGridViewTextBoxColumn();
            colClientID.HeaderText = "Client ID";
            colClientID.DataPropertyName = "ClientID";
            colClientID.Name = "ClientID";
            colClientID.Width = 100;
            DataGridReturnBooks.Columns.Add(colClientID);

            // --- Client Name ---
            var colClientName = new DataGridViewTextBoxColumn();
            colClientName.HeaderText = "Client Name";
            colClientName.DataPropertyName = "ClientName";
            colClientName.Name = "ClientName";
            colClientName.Width = 180;
            DataGridReturnBooks.Columns.Add(colClientName);

            // --- Client Type ---
            var colClientType = new DataGridViewTextBoxColumn();
            colClientType.HeaderText = "Client Type";
            colClientType.DataPropertyName = "ClientType";
            colClientType.Name = "ClientType";
            colClientType.Width = 120;
            DataGridReturnBooks.Columns.Add(colClientType);

            // --- Book Title ---
            var colBookTitle = new DataGridViewTextBoxColumn();
            colBookTitle.HeaderText = "Book Title";
            colBookTitle.DataPropertyName = "BookTitle";
            colBookTitle.Name = "BookTitle";
            colBookTitle.Width = 200;
            DataGridReturnBooks.Columns.Add(colBookTitle);

            // --- Quantity ---
            var colQuantity = new DataGridViewTextBoxColumn();
            colQuantity.HeaderText = "Quantity";
            colQuantity.DataPropertyName = "Quantity";
            colQuantity.Name = "Quantity";
            colQuantity.Width = 80;
            DataGridReturnBooks.Columns.Add(colQuantity);

            // --- Source ---
            var colSource = new DataGridViewTextBoxColumn();
            colSource.HeaderText = "Source";
            colSource.DataPropertyName = "Source";
            colSource.Name = "Source";
            colSource.Width = 100;
            DataGridReturnBooks.Columns.Add(colSource);

            // --- Issue Date ---
            var colIssueDate = new DataGridViewTextBoxColumn();
            colIssueDate.HeaderText = "Issue Date";
            colIssueDate.DataPropertyName = "IssueDate";
            colIssueDate.Name = "IssueDate";
            colIssueDate.Width = 120;
            DataGridReturnBooks.Columns.Add(colIssueDate);

            // --- Due Date ---
            var colDueDate = new DataGridViewTextBoxColumn();
            colDueDate.HeaderText = "Due Date";
            colDueDate.DataPropertyName = "DueDate";
            colDueDate.Name = "DueDate";
            colDueDate.Width = 120;
            DataGridReturnBooks.Columns.Add(colDueDate);

            // --- Return Date ---
            var colReturnDate = new DataGridViewTextBoxColumn();
            colReturnDate.HeaderText = "Return Date";
            colReturnDate.DataPropertyName = "ReturnDate";
            colReturnDate.Name = "ReturnDate";
            colReturnDate.Width = 120;
            DataGridReturnBooks.Columns.Add(colReturnDate);

            // --- Status ---
            var colStatus = new DataGridViewTextBoxColumn();
            colStatus.HeaderText = "Status";
            colStatus.DataPropertyName = "Status";
            colStatus.Name = "Status";
            colStatus.Width = 100;
            DataGridReturnBooks.Columns.Add(colStatus);

            // --- Styling (yellow theme) ---
            DataGridReturnBooks.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridReturnBooks.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridReturnBooks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(253, 242, 194);
            DataGridReturnBooks.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            DataGridReturnBooks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            DataGridReturnBooks.DefaultCellStyle.BackColor = Color.White;
            DataGridReturnBooks.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 250, 230);
        }



    }
}
