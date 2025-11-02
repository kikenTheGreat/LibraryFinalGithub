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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Library_Final
{
    public partial class DamagedBookReport : Form
    {
        public string PreClientID { get; set; }
        public string PreISBN { get; set; }
        public string PreBookTitle { get; set; }

        // ✅ ADD THESE FLAGS
        private bool isUpdatingFields = false;

        public DamagedBookReport()
        {
            InitializeComponent();

 


        }
        // ✅ Retrieve book info only when ISBN or ClientID is provided
        private void AutoRetrieveFromIssueBooks()
        {
            if (string.IsNullOrWhiteSpace(txtISBN.Text) && string.IsNullOrWhiteSpace(txtClientID.Text))
                return;

            try
            {
                using (SqlConnection con = new SqlConnection(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;"))
                {
                    string query = @"
                        SELECT TOP 1 
                            ISBN,
                            BookTitle,
                            ClientID,
                            BookCondition,
                            Penalty AS FineAmount,
                            GETDATE() AS ReportDate
                        FROM IssueBooks
                        WHERE 
                            (ISBN = @ISBN OR ClientID = @ClientID)
                            AND (Status = 'Issued' OR Status = 'Overdue')
                        ORDER BY IssueDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ISBN", txtISBN.Text.Trim());
                        cmd.Parameters.AddWithValue("@ClientID", txtClientID.Text.Trim());

                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            txtISBN.Text = reader["ISBN"].ToString();
                            txtBookTitle.Text = reader["BookTitle"].ToString();
                            txtClientID.Text = reader["ClientID"].ToString();
                            cmbBookCondition.Text = reader["BookCondition"].ToString();

                            // Fine amount
                            decimal fine = 0;
                            decimal.TryParse(reader["FineAmount"].ToString(), out fine);
                            txtFineAmount.Text = fine.ToString("0.00");

                            // Report date
                            dtpReportDate.Value = Convert.ToDateTime(reader["ReportDate"]);

                            // Default description if empty
                            if (string.IsNullOrWhiteSpace(txtDamageDescription.Text))
                                txtDamageDescription.Text = "Reported as damaged during return.";
                        }
                        else
                        {
                            MessageBox.Show("No issued record found for this Client ID or ISBN.",
                                "No Record Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving issued book details: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ Auto-fill fields when typing ClientID
        // ✅ Auto-fill fields when typing ClientID
        // ✅ Auto-fill all fields including Borrower Name when typing ClientID
        // ✅ Auto-fill all fields when typing ClientID
        private void RetrieveBookInfoByClientID()
        {
            string clientIdInput = txtClientID.Text.Trim();
            if (string.IsNullOrWhiteSpace(clientIdInput) || isUpdatingFields)
                return;

            try
            {
                isUpdatingFields = true; // ✅ SET FLAG BEFORE UPDATING

                using (SqlConnection con = new SqlConnection(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;"))
                {
                    con.Open();

                    string query = @"
                SELECT TOP 1 
                    i.ISBN, i.BookTitle, i.Penalty AS FineAmount,
                    i.StudentName AS BorrowerName, s.Role AS ClientType,
                    i.BookCondition, i.Status
                FROM IssueBooks i
                LEFT JOIN AddStudentAcc s ON i.ClientID = s.ClientID
                WHERE i.ClientID = @ClientID
                ORDER BY i.IssueDate DESC;";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", clientIdInput);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                // ✅ DO NOT modify txtClientID - keep what user typed
                                txtISBN.Text = dr["ISBN"].ToString();
                                txtBookTitle.Text = dr["BookTitle"].ToString();
                                txtFineAmount.Text = dr["FineAmount"].ToString();
                                txtReportedBy.Text = dr["BorrowerName"].ToString();
                                txtClientType.Text = dr["ClientType"].ToString();
                                txtStatus.Text = dr["Status"]?.ToString();

                                string issuedCondition = dr["BookCondition"]?.ToString()?.Trim();

                                cmbBookCondition.Items.Clear();
                                cmbBookCondition.Items.AddRange(new string[]
                                    { "Good", "Minor Damaged", "Damaged", "Lost" });

                                if (!string.IsNullOrEmpty(issuedCondition))
                                {
                                    if (cmbBookCondition.Items.Contains(issuedCondition))
                                        cmbBookCondition.SelectedItem = issuedCondition;
                                    else
                                        cmbBookCondition.Text = issuedCondition;
                                }
                                else
                                {
                                    cmbBookCondition.SelectedItem = "Good";
                                }
                            }
                            else
                            {
                                // Clear other fields but NOT ClientID
                                txtISBN.Clear();
                                txtBookTitle.Clear();
                                txtFineAmount.Clear();
                                txtReportedBy.Clear();
                                txtClientType.Clear();
                                txtStatus.Clear();
                                cmbBookCondition.SelectedIndex = -1;
                                cmbReturnBookCondition.SelectedIndex = -1;
                                txtDamageDescription.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving book info: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isUpdatingFields = false; // ✅ RESET FLAG AFTER UPDATING
            }
        }











        // ✅ Load all books for a given ClientID from IssueBooks



        private void txtISBN_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingFields) return; // ✅ Don't trigger during updates

            if (txtISBN.Text.Trim().Length >= 3)
                RetrieveBookInfoByISBN();
        }

        private void RetrieveBookInfoByISBN()
        {
            if (string.IsNullOrWhiteSpace(txtISBN.Text) || isUpdatingFields)
                return;

            try
            {
                isUpdatingFields = true; // ✅ SET FLAG

                using (SqlConnection con = new SqlConnection(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;"))
                {
                    string query = @"
                SELECT TOP 1 
                    i.ClientID,
                    i.ISBN,
                    i.BookTitle,
                    i.Penalty AS FineAmount,
                    i.StudentName AS BorrowerName,
                    s.Role AS ClientType,
                    i.BookCondition,
                    i.Status
                FROM IssueBooks i
                LEFT JOIN AddStudentAcc s ON i.ClientID = s.ClientID
                WHERE i.ISBN = @ISBN
                ORDER BY i.IssueDate DESC;";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ISBN", txtISBN.Text.Trim());
                        con.Open();

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            // ✅ NOW it's OK to update ClientID since user searched by ISBN
                            txtClientID.Text = reader["ClientID"].ToString();
                            txtBookTitle.Text = reader["BookTitle"].ToString();
                            txtFineAmount.Text = reader["FineAmount"].ToString();
                            txtStatus.Text = reader["Status"]?.ToString();
                            txtReportedBy.Text = reader["BorrowerName"]?.ToString();
                            txtClientType.Text = reader["ClientType"]?.ToString();

                            string bookCondition = reader["BookCondition"]?.ToString();
                            cmbBookCondition.Items.Clear();
                            cmbBookCondition.Items.Add("Good");
                            cmbBookCondition.Items.Add("Minor Damaged");
                            cmbBookCondition.Items.Add("Damaged");
                            cmbBookCondition.Items.Add("Lost");

                            if (!string.IsNullOrWhiteSpace(bookCondition) &&
                                cmbBookCondition.Items.Contains(bookCondition))
                            {
                                cmbBookCondition.SelectedItem = bookCondition;
                            }
                            else
                            {
                                cmbBookCondition.SelectedItem = "Good";
                            }

                            dtpReportDate.Value = DateTime.Now;
                            txtDamageDescription.Text = "Reported as damaged during use or return.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving data by ISBN: " + ex.Message);
            }
            finally
            {
                isUpdatingFields = false; // ✅ RESET FLAG
            }
        }

        // ✅ Load all reports into DataGridView


        // ✅ Save Damage Report
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string isbn = txtISBN.Text.Trim();
                string clientID = txtClientID.Text.Trim();
                string beforeCondition = cmbBookCondition.Text.Trim();           // before issue
                string returnedCondition = cmbReturnBookCondition.Text.Trim();   // returning condition

                if (string.IsNullOrEmpty(beforeCondition))
                    beforeCondition = "Unknown";

                if (string.IsNullOrEmpty(returnedCondition))
                {
                    MessageBox.Show("Please select the book condition upon return.",
                        "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string combinedCondition = $"{beforeCondition} → {returnedCondition}";

                using (SqlConnection con = new SqlConnection(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;"))
                {
                    con.Open();

                    // ✅ Insert into DamagedBooks
                    string insertQuery = @"
                INSERT INTO DamagedBooks 
                    (ISBN, Title, BookCondition, DamageDescription, ReportDate, FineAmount, ClientID)
                VALUES 
                    (@ISBN, @Title, @BookCondition, @DamageDescription, @ReportDate, @FineAmount, @ClientID)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@ISBN", isbn);
                        cmd.Parameters.AddWithValue("@Title", txtBookTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@BookCondition", combinedCondition);
                        cmd.Parameters.AddWithValue("@DamageDescription", txtDamageDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@ReportDate", dtpReportDate.Value);
                        cmd.Parameters.AddWithValue("@ClientID", clientID);

                        decimal fine = 0;
                        decimal.TryParse(txtFineAmount.Text, out fine);
                        cmd.Parameters.AddWithValue("@FineAmount", fine);

                        cmd.ExecuteNonQuery();
                    }

                    // 🧩 Step 4: Update IssueBooks for consistency
                    string updateQuery = @"
    UPDATE IssueBooks
    SET 
        Status = 'Report filed by librarian',
        BookCondition = @BookCondition,
        Penalty = @FineAmount
    WHERE ISBN = @ISBN AND ClientID = @ClientID";

                    using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, con))
                    {
                        cmdUpdate.Parameters.AddWithValue("@BookCondition", combinedCondition);
                        cmdUpdate.Parameters.AddWithValue("@FineAmount", string.IsNullOrWhiteSpace(txtFineAmount.Text) ? 0 : Convert.ToDecimal(txtFineAmount.Text));
                        cmdUpdate.Parameters.AddWithValue("@ISBN", isbn);
                        cmdUpdate.Parameters.AddWithValue("@ClientID", clientID);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    // ✅ Show the updated status in the textbox for user feedback
                    txtStatus.Text = "Report filed by librarian";

                    MessageBox.Show("Damage report filed successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);


                    LoadDamageReports();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving damage report: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        // ✅ Load all damage reports into DataGridView
        // ✅ Load all damage reports into DataGridView
        private void LoadDamageReports()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;"))
                {
                    con.Open();

                    string query;

                    // ✅ If txtClientID has value, filter by that ClientID
                    if (!string.IsNullOrWhiteSpace(txtClientID.Text))
                    {
                        query = @"
                    SELECT 
                        d.DamageID,
                        d.ISBN,
                        d.Title,
                        d.BookCondition,
                        d.DamageDescription,
                        d.ReportDate,
                        d.FineAmount,
                        d.ClientID,
                        i.StudentName AS BorrowerName,
                        i.Status
                    FROM DamagedBooks d
                    LEFT JOIN IssueBooks i ON d.ClientID = i.ClientID AND d.ISBN = i.ISBN
                    WHERE d.ClientID = @ClientID
                    ORDER BY d.ReportDate DESC";
                    }
                    else
                    {
                        // ✅ Otherwise, load all damage reports
                        query = @"
                    SELECT 
                        d.DamageID,
                        d.ISBN,
                        d.Title,
                        d.BookCondition,
                        d.DamageDescription,
                        d.ReportDate,
                        d.FineAmount,
                        d.ClientID,
                        i.StudentName AS BorrowerName,
                        i.Status
                    FROM DamagedBooks d
                    LEFT JOIN IssueBooks i ON d.ClientID = i.ClientID AND d.ISBN = i.ISBN
                    ORDER BY d.ReportDate DESC";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (!string.IsNullOrWhiteSpace(txtClientID.Text))
                            cmd.Parameters.AddWithValue("@ClientID", txtClientID.Text.Trim());

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dgvDamageReports.DataSource = dt;
                        }
                    }
                }

                // ✅ Beautify DataGridView
                dgvDamageReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvDamageReports.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvDamageReports.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvDamageReports.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgvDamageReports.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Regular);
                dgvDamageReports.RowTemplate.Height = 30;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading damage reports: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ClearFields()
        {
            txtISBN.Clear();
            txtBookTitle.Clear();
            txtClientID.Clear();
            cmbBookCondition.SelectedIndex = -1;
            cmbReturnBookCondition.SelectedIndex = -1;
            txtDamageDescription.Clear();
            txtReportedBy.Clear();
            txtFineAmount.Clear();
            txtStatus.Clear(); // ✅ added
        }


        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void DamagedBookReport_Load(object sender, EventArgs e)
        {

            // ✅ ADD THIS DEBUGGING
            MessageBox.Show($"PreClientID value: '{PreClientID}'\ntxtClientID.Text value: '{txtClientID.Text}'",
                            "DEBUG: Load Event");

            if (string.IsNullOrEmpty(txtClientID.Text))
            {
                if (!string.IsNullOrEmpty(PreClientID))
                {
                    MessageBox.Show($"Setting txtClientID to PreClientID: {PreClientID}", "DEBUG");
                    txtClientID.Text = PreClientID;
                }
            }


            //end of deletion

            if (string.IsNullOrEmpty(txtClientID.Text))
            {
                if (!string.IsNullOrEmpty(PreClientID))
                    txtClientID.Text = PreClientID;
            }

            if (string.IsNullOrEmpty(txtISBN.Text))
            {
                if (!string.IsNullOrEmpty(PreISBN))
                    txtISBN.Text = PreISBN;
            }

            if (!string.IsNullOrEmpty(PreBookTitle))
                txtBookTitle.Text = PreBookTitle;



            LoadDamageReports();
            cmbReturnBookCondition.Items.AddRange(new string[]
           {
    
    "Minor Damaged",
    "Damaged",
    "Lost"
           });

            cmbReturnBookCondition.SelectedIndex = 0;
        }



        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();

        }

        private void txtClientID_TextChanged(object sender, EventArgs e)
        {
            if (isUpdatingFields) return; // ✅ Don't trigger during updates

            if (txtClientID.Text.Trim().Length >= 4)
                RetrieveBookInfoByClientID();
        }

        private void dgvDamageReports_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDamageReports.Rows[e.RowIndex];

                txtISBN.Text = row.Cells["ISBN"].Value?.ToString();
                txtBookTitle.Text = row.Cells["BookTitle"].Value?.ToString();
                cmbBookCondition.Text = row.Cells["BookCondition"].Value?.ToString();
                txtFineAmount.Text = row.Cells["FineAmount"].Value?.ToString();

                dtpReportDate.Value = DateTime.Now;
            }
        }

        private void btnMarkReturned_Click(object sender, EventArgs e)
        {
            string clientID = txtClientIDStatus.Text.Trim();

            if (string.IsNullOrEmpty(clientID))
            {
                MessageBox.Show("Please enter a Client ID first.", "Missing Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;"))
                {
                    con.Open();

                    // ✅ Check if client exists with that status
                    string checkQuery = @"
                SELECT COUNT(*) FROM IssueBooks
                WHERE ClientID = @ClientID AND Status = 'Report filed by librarian'";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@ClientID", clientID);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count == 0)
                        {
                            MessageBox.Show("No record found with status 'Report filed by librarian' for this Client ID.",
                                "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    // ✅ Update status to "Returned"
                    string updateQuery = @"
                UPDATE IssueBooks
                SET Status = 'Returned', ReturnDate = GETDATE()
                WHERE ClientID = @ClientID AND Status = 'Report filed by librarian'";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                    {
                        updateCmd.Parameters.AddWithValue("@ClientID", clientID);
                        int rows = updateCmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Client's status updated to 'Returned' successfully!",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadDamageReports(); // refresh the grid
                            txtClientIDStatus.Clear();
                        }
                        else
                        {
                            MessageBox.Show("No rows were updated.", "Info",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating status: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
