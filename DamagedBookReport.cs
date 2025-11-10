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
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Library_Final
{
    public partial class DamagedBookReport : Form
    {
        private int currentEmployeeID; // store logged-in user's ID
        public string PreClientID { get; set; }
        public string PreISBN { get; set; }
        public string PreBookTitle { get; set; }



        // ✅ ADD THESE FLAGS
        private bool isUpdatingFields = false;
        // ✅ OPTION 2: Keep constructor parameter but validate with SessionData
        public DamagedBookReport()
        {
            InitializeComponent();




        }





        private void LoadEmployeeFullName()
        {
            // ✅ Use SessionData instead of the potentially incorrect currentEmployeeID
            int employeeID = SessionData.CurrentEmployeeID;



            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;";
            string query = "SELECT FirstName, LastName FROM Employees WHERE EmployeeID = @EmployeeID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string fullName = $"{reader["FirstName"]} {reader["LastName"]}";
                        guna2ComboBox1.Items.Clear();
                        guna2ComboBox1.Items.Add(fullName);
                        guna2ComboBox1.SelectedIndex = 0;

                        MessageBox.Show($"Successfully loaded: {fullName}", "Success");
                    }
                    else
                    {

                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employee: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


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
                string beforeCondition = cmbBookCondition.Text.Trim();
                string returnedCondition = cmbReturnBookCondition.Text.Trim();

                if (string.IsNullOrEmpty(beforeCondition))
                    beforeCondition = "Unknown";

                if (string.IsNullOrEmpty(returnedCondition))
                {
                    MessageBox.Show("Please select the book condition upon return.",
                        "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string combinedCondition = $"{beforeCondition} → {returnedCondition}";
                int damageReportID = 0;

                using (SqlConnection con = new SqlConnection(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;"))
                {
                    con.Open();

                    // Insert into DamagedBooks and get the ID
                    string insertQuery = @"
INSERT INTO DamagedBooks 
    (ISBN, Title, BookCondition, DamageDescription, ReportDate, FineAmount, ClientID, ReportedByEmployeeID, ReportedByName)
OUTPUT INSERTED.DamageID
VALUES 
    (@ISBN, @Title, @BookCondition, @DamageDescription, @ReportDate, @FineAmount, @ClientID, @ReportedByEmployeeID, @ReportedByName)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@ISBN", isbn);
                        cmd.Parameters.AddWithValue("@Title", txtBookTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@BookCondition", combinedCondition);
                        cmd.Parameters.AddWithValue("@DamageDescription", txtDamageDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@ReportDate", dtpReportDate.Value);
                        cmd.Parameters.AddWithValue("@ClientID", clientID);
                        cmd.Parameters.AddWithValue("@ReportedByEmployeeID", currentEmployeeID);
                        cmd.Parameters.AddWithValue("@ReportedByName", guna2ComboBox1.Text);

                        decimal fine = 0;
                        decimal.TryParse(txtFineAmount.Text, out fine);
                        cmd.Parameters.AddWithValue("@FineAmount", fine);

                        damageReportID = (int)cmd.ExecuteScalar();
                    }

                    // Update IssueBooks for consistency
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

                    txtStatus.Text = "Report filed by librarian";

                    MessageBox.Show("Damage report filed successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Generate PDF Report
                    GenerateDamageReportPDF(damageReportID);

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

        // Add this helper method to wrap text into multiple lines
        private List<string> WrapText(string text, XFont font, double maxWidth, XGraphics gfx)
        {
            List<string> lines = new List<string>();
            string[] words = text.Split(' ');
            string currentLine = "";

            foreach (string word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                XSize size = gfx.MeasureString(testLine, font);

                if (size.Width > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);

            return lines;
        }

        // Add this new method to generate the PDF:
        private void GenerateDamageReportPDF(int reportID)
        {
            try
            {
                // Create PDF Document
                PdfDocument pdf = new PdfDocument();
                PdfPage page = pdf.AddPage();
                page.Width = XUnit.FromMillimeter(210);  // A4 width
                page.Height = XUnit.FromMillimeter(297); // A4 height
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Fonts
                var fontTitle = new XFont("Arial", 16, XFontStyle.Bold);
                var fontHeader = new XFont("Arial", 12, XFontStyle.Bold);
                var fontRegular = new XFont("Arial", 10);
                var fontSmall = new XFont("Arial", 9);

                // Margins
                double margin = XUnit.FromMillimeter(20);
                double pageWidth = page.Width - (margin * 2);
                double currentY = margin;
                double lineHeight = 14;

                // --- Header Section ---
                gfx.DrawString("CGC LIBRARY SYSTEM", fontTitle, XBrushes.Black,
                    new XRect(margin, currentY, pageWidth, 20), XStringFormats.TopCenter);
                currentY += 25;

                gfx.DrawString("Damaged Book Report", fontHeader, XBrushes.Black,
                    new XRect(margin, currentY, pageWidth, 15), XStringFormats.TopCenter);
                currentY += 25;

                // Draw separator line
                gfx.DrawLine(new XPen(XColors.Gray, 1), margin, currentY, page.Width - margin, currentY);
                currentY += 15;

                // --- Report Information ---
                gfx.DrawString($"Report ID: {reportID}", fontRegular, XBrushes.Black,
                    new XRect(margin, currentY, pageWidth, 12), XStringFormats.TopLeft);
                currentY += 15;

                gfx.DrawString($"Date Filed: {dtpReportDate.Value.ToString("MMMM dd, yyyy")}", fontRegular, XBrushes.Black,
                    new XRect(margin, currentY, pageWidth, 12), XStringFormats.TopLeft);
                currentY += 15;

                gfx.DrawString($"Reported By: {guna2ComboBox1.Text}", fontRegular, XBrushes.Black,
                    new XRect(margin, currentY, pageWidth, 12), XStringFormats.TopLeft);
                currentY += 20;

                // Draw separator line
                gfx.DrawLine(new XPen(XColors.Gray, 1), margin, currentY, page.Width - margin, currentY);
                currentY += 15;

                // --- Book Information Section ---
                gfx.DrawString("Book Information", fontHeader, XBrushes.Black,
                    new XRect(margin, currentY, pageWidth, 15), XStringFormats.TopLeft);
                currentY += 20;

                // Create paragraph text
                string bookInfo = $"The book titled \"{txtBookTitle.Text.Trim()}\" with ISBN {txtISBN.Text.Trim()} " +
                                 $"has been reported as damaged. This book was originally issued in {cmbBookCondition.Text.Trim()} condition " +
                                 $"and was returned in {cmbReturnBookCondition.Text.Trim()} condition.";

                List<string> bookInfoLines = WrapText(bookInfo, fontRegular, pageWidth, gfx);
                foreach (string line in bookInfoLines)
                {
                    gfx.DrawString(line, fontRegular, XBrushes.Black,
                        new XRect(margin, currentY, pageWidth, lineHeight), XStringFormats.TopLeft);
                    currentY += lineHeight;
                }
                currentY += 10;

                // --- Borrower Information Section ---
                gfx.DrawString("Borrower Information", fontHeader, XBrushes.Black,
                    new XRect(margin, currentY, pageWidth, 15), XStringFormats.TopLeft);
                currentY += 20;

                string borrowerInfo = $"The book was borrowed by {txtReportedBy.Text.Trim()} (Client ID: {txtClientID.Text.Trim()}), " +
                                     $"who is registered as a {txtClientType.Text.Trim()} in the library system.";

                List<string> borrowerInfoLines = WrapText(borrowerInfo, fontRegular, pageWidth, gfx);
                foreach (string line in borrowerInfoLines)
                {
                    gfx.DrawString(line, fontRegular, XBrushes.Black,
                        new XRect(margin, currentY, pageWidth, lineHeight), XStringFormats.TopLeft);
                    currentY += lineHeight;
                }
                currentY += 10;

                // --- Damage Description Section ---
                gfx.DrawString("Damage Description", fontHeader, XBrushes.Black,
                    new XRect(margin, currentY, pageWidth, 15), XStringFormats.TopLeft);
                currentY += 20;

                string damageDesc = txtDamageDescription.Text.Trim();
                if (string.IsNullOrEmpty(damageDesc))
                    damageDesc = "No detailed description provided.";

                List<string> damageDescLines = WrapText(damageDesc, fontRegular, pageWidth, gfx);
                foreach (string line in damageDescLines)
                {
                    gfx.DrawString(line, fontRegular, XBrushes.Black,
                        new XRect(margin, currentY, pageWidth, lineHeight), XStringFormats.TopLeft);
                    currentY += lineHeight;
                }
                currentY += 10;

                // --- Fine Information Section ---
                gfx.DrawString("Financial Assessment", fontHeader, XBrushes.Black,
                    new XRect(margin, currentY, pageWidth, 15), XStringFormats.TopLeft);
                currentY += 20;

                decimal fineAmount = 0;
                decimal.TryParse(txtFineAmount.Text, out fineAmount);

                string fineInfo = $"Based on the extent of damage reported, a fine of P{fineAmount:N2} has been assessed " +
                                 $"and will be charged to the borrower's account. This amount must be settled before the borrower " +
                                 $"can borrow additional materials from the library.";

                List<string> fineInfoLines = WrapText(fineInfo, fontRegular, pageWidth, gfx);
                foreach (string line in fineInfoLines)
                {
                    gfx.DrawString(line, fontRegular, XBrushes.Black,
                        new XRect(margin, currentY, pageWidth, lineHeight), XStringFormats.TopLeft);
                    currentY += lineHeight;
                }
                currentY += 20;

                // Draw separator line
                gfx.DrawLine(new XPen(XColors.Gray, 1), margin, currentY, page.Width - margin, currentY);
                currentY += 15;

                // --- Footer Section ---
                string footer = "This is an official library document. Please keep this report for your records. " +
                               "For questions or concerns regarding this damage report, please contact the library administration.";

                List<string> footerLines = WrapText(footer, fontSmall, pageWidth, gfx);
                foreach (string line in footerLines)
                {
                    gfx.DrawString(line, fontSmall, XBrushes.Gray,
                        new XRect(margin, currentY, pageWidth, 12), XStringFormats.TopCenter);
                    currentY += 12;
                }

                // Save and open PDF
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Library_DamageReports");
                Directory.CreateDirectory(folderPath);
                string pdfPath = Path.Combine(folderPath, $"DamageReport_{reportID}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                pdf.Save(pdfPath);
                pdf.Close();

                MessageBox.Show($"Damage report PDF created:\n{pdfPath}", "PDF Generated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Automatically open for printing
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = pdfPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating PDF: {ex.Message}", "PDF Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                    string query = @"
                SELECT 
                    d.DamageID,
                    d.ISBN,
                    d.Title AS BookTitle,
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

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvDamageReports.DataSource = null;
                        dgvDamageReports.Columns.Clear();
                        dgvDamageReports.AutoGenerateColumns = true;
                        dgvDamageReports.DataSource = dt;

                        // Apply styling
                        StyleLogDataGrid(dgvDamageReports);

                        // Rename columns
                        if (dgvDamageReports.Columns.Contains("DamageDescription"))
                            dgvDamageReports.Columns["DamageDescription"].HeaderText = "Damage Description";
                        if (dgvDamageReports.Columns.Contains("BookCondition"))
                            dgvDamageReports.Columns["BookCondition"].HeaderText = "Book Condition";
                        if (dgvDamageReports.Columns.Contains("FineAmount"))
                            dgvDamageReports.Columns["FineAmount"].HeaderText = "Fine";
                        if (dgvDamageReports.Columns.Contains("BorrowerName"))
                            dgvDamageReports.Columns["BorrowerName"].HeaderText = "Borrower";
                        if (dgvDamageReports.Columns.Contains("ReportDate"))
                            dgvDamageReports.Columns["ReportDate"].HeaderText = "Report Date";
                        if (dgvDamageReports.Columns.Contains("BookTitle"))
                            dgvDamageReports.Columns["BookTitle"].HeaderText = "Book Title";

                        // Adjust columns
                        AdjustDamageColumns(dgvDamageReports);
                    }
                }
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
            // ✅ Check if Form1 is already open
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is Form1)
                {
                    openForm.Show();
                    openForm.BringToFront();
                    this.Close(); // Use Close instead of Hide
                    return;
                }
            }

            // ✅ If not open, create new instance with SessionData
            Form1 form1 = new Form1(SessionData.CurrentEmployeeID);
            form1.Show();
            this.Close(); // Use Close instead of Hide
        }

        private async void DamagedBookReport_Load(object sender, EventArgs e)

        {
            await Task.Delay(100);
            await LoadEmployeeFullNameAsync();
            await LoadDamageReportsAsync(); // This already handles all styling

            // Pre-populate fields if passed from another form
            if (!string.IsNullOrEmpty(PreClientID))
                txtClientID.Text = PreClientID;

            if (!string.IsNullOrEmpty(PreISBN))
                txtISBN.Text = PreISBN;

            if (!string.IsNullOrEmpty(PreBookTitle))
                txtBookTitle.Text = PreBookTitle;

            // Populate return condition dropdown
            cmbReturnBookCondition.Items.Clear();
            cmbReturnBookCondition.Items.AddRange(new string[]
            {
        "Minor Damaged",
        "Damaged",
        "Lost"
            });
            cmbReturnBookCondition.SelectedIndex = 0;
        }






        private async Task LoadDamageReportsAsync()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;"))
                {
                    await con.OpenAsync();

                    string query = @"
                SELECT 
                    d.ISBN,
                    d.Title AS BookTitle,
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

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        await Task.Run(() => da.Fill(dt));

                        dgvDamageReports.Invoke(new Action(() =>
                        {
                            // Clear and set up the DataGridView
                            dgvDamageReports.DataSource = null;
                            dgvDamageReports.Columns.Clear();
                            dgvDamageReports.AutoGenerateColumns = true;
                            dgvDamageReports.DataSource = dt;

                            // Apply styling FIRST
                            StyleLogDataGrid(dgvDamageReports);

                            // THEN customize column headers
                            if (dgvDamageReports.Columns.Contains("DamageDescription"))
                                dgvDamageReports.Columns["DamageDescription"].HeaderText = "Damage Description";
                            if (dgvDamageReports.Columns.Contains("BookCondition"))
                                dgvDamageReports.Columns["BookCondition"].HeaderText = "Book Condition";
                            if (dgvDamageReports.Columns.Contains("FineAmount"))
                                dgvDamageReports.Columns["FineAmount"].HeaderText = "Fine";
                            if (dgvDamageReports.Columns.Contains("BorrowerName"))
                                dgvDamageReports.Columns["BorrowerName"].HeaderText = "Borrower";
                            if (dgvDamageReports.Columns.Contains("ReportDate"))
                                dgvDamageReports.Columns["ReportDate"].HeaderText = "Report Date";
                            if (dgvDamageReports.Columns.Contains("BookTitle"))
                                dgvDamageReports.Columns["BookTitle"].HeaderText = "Book Title";

                            // FINALLY adjust column widths
                            AdjustDamageColumns(dgvDamageReports);
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading damage reports: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Replace your StyleLogDataGrid method with this:
        private void StyleLogDataGrid(DataGridView dgv)
        {
            // General layout
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Alternating row colors
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgv.RowsDefaultCellStyle.BackColor = Color.White;

            // Row style
            dgv.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgv.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgv.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 245);
            dgv.RowsDefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.RowsDefaultCellStyle.Padding = new Padding(5);
            dgv.RowTemplate.Height = 45;

            // Header style
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 102, 204);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Grid lines
            dgv.GridColor = Color.LightGray;

            // Auto-size
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }


        // REMOVE THE OLD AdjustDamageColumns method and replace with this ONE:
        private void AdjustDamageColumns(DataGridView dgv)
        {
            if (dgv.Columns.Count == 0) return;

            // Set fill mode
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Adjust column weights
            if (dgv.Columns.Contains("DamageID"))
            {
                dgv.Columns["DamageID"].FillWeight = 8;
                dgv.Columns["DamageID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgv.Columns.Contains("ISBN"))
            {
                dgv.Columns["ISBN"].FillWeight = 12;
                dgv.Columns["ISBN"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgv.Columns.Contains("BookTitle"))
            {
                dgv.Columns["BookTitle"].FillWeight = 20;
                dgv.Columns["BookTitle"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            if (dgv.Columns.Contains("ClientID"))
            {
                dgv.Columns["ClientID"].FillWeight = 10;
                dgv.Columns["ClientID"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgv.Columns.Contains("BorrowerName"))
            {
                dgv.Columns["BorrowerName"].FillWeight = 15;
                dgv.Columns["BorrowerName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            if (dgv.Columns.Contains("BookCondition"))
            {
                dgv.Columns["BookCondition"].FillWeight = 15;
                dgv.Columns["BookCondition"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgv.Columns.Contains("DamageDescription"))
            {
                dgv.Columns["DamageDescription"].FillWeight = 35;
                dgv.Columns["DamageDescription"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                dgv.Columns["DamageDescription"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }

            if (dgv.Columns.Contains("ReportDate"))
            {
                dgv.Columns["ReportDate"].FillWeight = 15;
                dgv.Columns["ReportDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns["ReportDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
            }

            if (dgv.Columns.Contains("FineAmount"))
            {
                dgv.Columns["FineAmount"].FillWeight = 10;
                dgv.Columns["FineAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns["FineAmount"].DefaultCellStyle.Format = "C2"; // Currency format
            }

            if (dgv.Columns.Contains("Status"))
            {
                dgv.Columns["Status"].FillWeight = 15;
                dgv.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();

        }

        private async Task LoadEmployeeFullNameAsync()
        {
            int employeeID = SessionData.CurrentEmployeeID;

            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;";
            string query = "SELECT FirstName, LastName FROM Employees WHERE EmployeeID = @EmployeeID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    await conn.OpenAsync();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        string fullName = $"{reader["FirstName"]} {reader["LastName"]}";
                        guna2ComboBox1.Items.Clear();
                        guna2ComboBox1.Items.Add(fullName);
                        guna2ComboBox1.SelectedIndex = 0;
                    }

                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employee: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
                {
                    con.Open();

                    // ✅ 1. Check if record exists with 'Report filed by librarian' status
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

                    // ✅ 2. Update IssueBooks to mark as Returned
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
                            // ✅ 3. Automatically mark any pending penalties as paid
                            string clearPenaltyQuery = @"
                        UPDATE PendingPenalties
                        SET IsPaid = 1
                        WHERE ClientID = @ClientID AND IsPaid = 0";
                            using (SqlCommand clearPenaltyCmd = new SqlCommand(clearPenaltyQuery, con))
                            {
                                clearPenaltyCmd.Parameters.AddWithValue("@ClientID", clientID);
                                clearPenaltyCmd.ExecuteNonQuery();
                            }

                            // ✅ 4. Check if student has ANY remaining issues
                            string checkRemainingIssuesQuery = @"
                        SELECT COUNT(*) 
                        FROM IssueBooks 
                        WHERE ClientID = @ClientID 
                        AND (
                            Penalty > 0 
                            OR Status = 'Overdue' 
                            OR Status = 'Report filed by librarian'
                            OR Status = 'Issued'
                        )
                        AND Status != 'Returned'";

                            int remainingIssues = 0;
                            using (SqlCommand checkRemainingCmd = new SqlCommand(checkRemainingIssuesQuery, con))
                            {
                                checkRemainingCmd.Parameters.AddWithValue("@ClientID", clientID);
                                remainingIssues = (int)checkRemainingCmd.ExecuteScalar();
                            }

                            // ✅ 5. Check again for unpaid penalties (after clearing)
                            string checkPendingPenaltiesQuery = @"
                        SELECT COUNT(*) 
                        FROM PendingPenalties 
                        WHERE ClientID = @ClientID AND IsPaid = 0";
                            int pendingPenalties = 0;
                            using (SqlCommand checkPendingCmd = new SqlCommand(checkPendingPenaltiesQuery, con))
                            {
                                checkPendingCmd.Parameters.AddWithValue("@ClientID", clientID);
                                pendingPenalties = (int)checkPendingCmd.ExecuteScalar();
                            }

                            // ✅ 6. Update AddStudentAcc status
                            if (remainingIssues == 0 && pendingPenalties == 0)
                            {
                                string updateStatusQuery = @"
                            UPDATE AddStudentAcc 
                            SET Status = 'Inactive' 
                            WHERE ClientID = @ClientID";

                                using (SqlCommand updateStatusCmd = new SqlCommand(updateStatusQuery, con))
                                {
                                    updateStatusCmd.Parameters.AddWithValue("@ClientID", clientID);
                                    int statusUpdated = updateStatusCmd.ExecuteNonQuery();

                                    MessageBox.Show(
                                        $"✅ Book successfully returned!\n\n" +
                                        $"Student status updated to 'Returned' (Rows affected: {statusUpdated}).\n" +
                                        $"The student can now borrow books again.",
                                        "Success",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information
                                    );
                                }
                            }
                            else
                            {
                                MessageBox.Show(
                                    $"✅ Book returned successfully!\n\n" +
                                    $"⚠️ However, this student still has:\n" +
                                    $"• {remainingIssues} unreturned/overdue book(s)\n" +
                                    $"• {pendingPenalties} unpaid penalty/penalties\n\n" +
                                    $"Status remains as 'With Pending Issues'.\n" +
                                    $"Student cannot borrow until all issues are resolved.",
                                    "Partial Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                            }

                            // ✅ 7. Log the activity
                            ActivityLog.RecordActivity(
                                SessionData.CurrentUserName,
                                "Mark Returned (Damage Report)",
                                "Damage Report Module",
                                $"Marked damage report as returned for ClientID: {clientID}"
                            );

                            LoadDamageReports(); // Refresh the grid
                            txtClientIDStatus.Clear();

                            // ✅ 8. Refresh CreateAcc form if open
                            foreach (Form openForm in Application.OpenForms)
                            {
                                if (openForm is CreateAcc createAccForm)
                                {
                                    createAccForm.LoadStudentAccounts();
                                    break;
                                }
                            }

                            // ✅ 9. Refresh Issue form if open
                            foreach (Form openForm in Application.OpenForms)
                            {
                                if (openForm is Issue issueForm)
                                {
                                    issueForm.LoadIssueBooks();
                                    issueForm.LoadReturnedBooks();
                                    issueForm.UpdateTotalOverdueLabel();
                                    break;
                                }
                            }
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


        private void dgvDamageReports_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}