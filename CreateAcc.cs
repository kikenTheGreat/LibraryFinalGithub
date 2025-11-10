using Library_Final;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic; // for Interaction.InputBox
using OnBarcode.Barcode;
using PdfSharp.Drawing;
using PdfSharp.Drawing;
using PdfSharp.Drawing;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Fonts;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf;
using PdfSharp.Pdf;
using PdfSharp.Pdf;
using QRCoder;
using QRCoder;
using QRCoder;
using System;
using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;          // Bitmap
using System.Drawing;
using System.Drawing;
using System.Drawing;
using System.Drawing.Imaging;  // ImageFormat
using System.Drawing.Imaging;
using System.Drawing.Imaging;
using System.Drawing.Imaging;
using System.Drawing.Imaging;
using System.IO;
using System.IO;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using System.Windows.Forms;
using System.Xml.Linq;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Rendering;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace LibraryCGC
{
    public partial class CreateAcc : Form
    {

        public CreateAcc()
        {
            InitializeComponent();
            LoadStudentAccounts();

            GlobalFontSettings.FontResolver = SimpleFontResolver.Instance;



        }

        private void CreateAcc_Load(object sender, EventArgs e)
        {
            Name.Focus();
            //output data grid
            SetupAccountGrid();
            LoadStudentAccounts();
            CheckSemesterStatus(); // ✅ check button states on lo
            MoveInactiveStudents();
        }

        public void LoadStudentAccounts()   //output data grid
        {
            using (SqlConnection con = new SqlConnection("  Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n"))
            {
                con.Open();

                string query = "SELECT * FROM AddStudentAcc";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    AddStudentAccDataGrid.DataSource = dt;


                    // ✅ Ensure each DataGridView column has a proper Name
                    foreach (DataGridViewColumn col in AddStudentAccDataGrid.Columns)
                    {
                        col.Name = col.DataPropertyName;
                    }

                }

          HighlightStudentStatusRows(); 




            }
        }

        private void HighlightStudentStatusRows()
        {
            foreach (DataGridViewRow row in AddStudentAccDataGrid.Rows)
            {
                if (row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();

                    if (status.Equals("With Pending Issues", StringComparison.OrdinalIgnoreCase))
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                        row.DefaultCellStyle.ForeColor = Color.White;
                    }

                }
            }
        }


        private void SetupAccountGrid()
        {
            AddStudentAccDataGrid.Columns.Clear();
            AddStudentAccDataGrid.AutoGenerateColumns = false;
            AddStudentAccDataGrid.ReadOnly = true;
            AddStudentAccDataGrid.RowHeadersVisible = false;
            AddStudentAccDataGrid.BorderStyle = BorderStyle.None;
            AddStudentAccDataGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            AddStudentAccDataGrid.EnableHeadersVisualStyles = false;

            // --- Client ID (hidden) ---
            var colClientID = new DataGridViewTextBoxColumn();
            colClientID.HeaderText = "Client ID";
            colClientID.DataPropertyName = "ClientID";
            colClientID.Name = "ClientID";
            colClientID.Visible = true;
            AddStudentAccDataGrid.Columns.Add(colClientID);

            // --- Name ---
            var colName = new DataGridViewTextBoxColumn();
            colName.HeaderText = "Name";
            colName.DataPropertyName = "Name";
            colName.Name = "Name";
            colName.Width = 180;
            AddStudentAccDataGrid.Columns.Add(colName);

            // --- Role ---
            var colRole = new DataGridViewTextBoxColumn();
            colRole.HeaderText = "Role";
            colRole.DataPropertyName = "Role";
            colRole.Name = "Role";
            colRole.Width = 100;
            AddStudentAccDataGrid.Columns.Add(colRole);

            // status
            var colStatus = new DataGridViewTextBoxColumn();
            colStatus.HeaderText = "Status";
            colStatus.DataPropertyName = "Status";
            AddStudentAccDataGrid.Columns.Add(colStatus);

            // --- Section / SY ---
            var colSection = new DataGridViewTextBoxColumn();
            colSection.HeaderText = "Section/SY";
            colSection.DataPropertyName = "SectionSY";
            colSection.Name = "SectionSY";
            colSection.Width = 120;
            AddStudentAccDataGrid.Columns.Add(colSection);

            // --- Email ---
            var colEmail = new DataGridViewTextBoxColumn();
            colEmail.HeaderText = "Email";
            colEmail.DataPropertyName = "Email";
            colEmail.Name = "Email";
            colEmail.Width = 180;
            AddStudentAccDataGrid.Columns.Add(colEmail);

            // --- Student Number ---
            var colStudentNumber = new DataGridViewTextBoxColumn();
            colStudentNumber.HeaderText = "Student Number";
            colStudentNumber.DataPropertyName = "StudentNumber";
            colStudentNumber.Name = "StudentNumber";
            colStudentNumber.Width = 140;
            AddStudentAccDataGrid.Columns.Add(colStudentNumber);

            // --- Department ---
            var colDepartment = new DataGridViewTextBoxColumn();
            colDepartment.HeaderText = "Department";
            colDepartment.DataPropertyName = "Department";
            colDepartment.Name = "Department";
            colDepartment.Width = 120;
            AddStudentAccDataGrid.Columns.Add(colDepartment);

            // --- Styling (same as others) ---
            AddStudentAccDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            AddStudentAccDataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            AddStudentAccDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(253, 242, 194);
            AddStudentAccDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            AddStudentAccDataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            AddStudentAccDataGrid.DefaultCellStyle.BackColor = Color.White;
            AddStudentAccDataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 250, 230);
            AddStudentAccDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }


        private void ClientID_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void arthanButton5_Load(object sender, EventArgs e)
        {


        }


        private void arthanButton5_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
            {
                string name = Name.Text.Trim();
                // --- NAME VALIDATION ---
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Name cannot be empty.",
                                    "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Name.Focus();
                    return;
                }


                if (Role.Text == "Student")
                {

                    // Check if email is empty
                    string email = Email.Text.Trim();
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        MessageBox.Show("Email cannot be empty.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Email.Focus();
                        return;
                    }


                    // Separate if for invalid email
                    if (!string.IsNullOrWhiteSpace(email) &&
                        !email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase) &&
                        !email.EndsWith(".citiglobalcollege.edu.ph", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Email must end with @gmail.com or .citiglobalcollege.edu.ph", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Check if STUDENT ID is empty
                    string studentID = StudentNumber.Text.Trim();
                    if (string.IsNullOrWhiteSpace(studentID))
                    {
                        MessageBox.Show("Student Number cannot be empty.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Email.Focus();
                        return;
                    }

                    // Check if STUDENT ID is empty
                    string SECTION = SectionSY.Text.Trim();
                    if (string.IsNullOrWhiteSpace(SECTION))
                    {
                        MessageBox.Show("Class Section cannot be empty.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Email.Focus();
                        return;
                    }



                    // ✅ Allow empty student number, but if not empty, it must be numbers only
                    string studentPattern = @"^\d+$"; // only digits allowed

                    if (!string.IsNullOrWhiteSpace(StudentNumber.Text.Trim()) &&
                        !Regex.IsMatch(StudentNumber.Text.Trim(), studentPattern))
                    {
                        MessageBox.Show("Student Number should be numbers only.",
                                        "Invalid Student Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        StudentNumber.Focus();
                        return;
                    }


                }
                else
                {
                    // Check if email is empty
                    string department = Department.Text.Trim();
                    if (string.IsNullOrWhiteSpace(department))
                    {
                        MessageBox.Show("Department cannot be empty.", "Invalid Email", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Email.Focus();
                        return;
                    }
                }









                con.Open();

                // ✅ Step 1: Check if there's an active semester
                SqlCommand semActiveCmd = new SqlCommand("SELECT COUNT(*) FROM SemesterDuration WHERE IsActive = 1", con);
                int semActive = (int)semActiveCmd.ExecuteScalar();

                if (semActive == 0)
                {
                    MessageBox.Show("Please start a semester first before creating accounts.", "No Active Semester");
                    return;
                }

                // ✅ Step 2: Auto-check if the semester duration has expired (6 months by default)
                SqlCommand semCheckCmd = new SqlCommand(@"
            SELECT TOP 1 StartDate, DurationMonths
            FROM SemesterDuration
            WHERE IsActive = 1
            ORDER BY ID DESC", con);

                using (SqlDataReader reader = semCheckCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        DateTime startDate = Convert.ToDateTime(reader["StartDate"]);
                        int duration = Convert.ToInt32(reader["DurationMonths"]);

                        if (DateTime.Now >= startDate.AddMonths(duration))
                        {
                            reader.Close(); // close before running new queries on same connection

                            DialogResult endSemAsk = MessageBox.Show(
                                "The semester duration has ended. Do you want to end the semester now?",
                                "Semester Expired",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question
                            );

                            if (endSemAsk == DialogResult.Yes)
                            {
                                SqlCommand endSem = new SqlCommand(@"
                            UPDATE AddStudentAcc SET Status = 'Inactive';
                            UPDATE SemesterDuration SET IsActive = 0 WHERE IsActive = 1;", con);
                                endSem.ExecuteNonQuery();

                                MessageBox.Show("Semester ended successfully. All accounts are now inactive.");
                                LoadStudentAccounts();
                                return;
                            }
                            else
                            {
                                // Ask librarian how many months to extend
                                string input = Interaction.InputBox(
                                    "Enter how many months to extend this semester:",
                                    "Extend Semester",
                                    "1" // default suggestion
                                );

                                if (int.TryParse(input, out int extendMonths) && extendMonths > 0)
                                {
                                    SqlCommand extendCmd = new SqlCommand(@"
                                UPDATE SemesterDuration
                                SET DurationMonths = DurationMonths + @extendMonths
                                WHERE IsActive = 1;", con);
                                    extendCmd.Parameters.AddWithValue("@extendMonths", extendMonths);
                                    extendCmd.ExecuteNonQuery();

                                    MessageBox.Show($"Semester extended by {extendMonths} month(s).");
                                }
                                else
                                {
                                    MessageBox.Show("Invalid input. No extension applied.");
                                }
                            }
                        }
                    }
                }

                // ✅ Step 3: Proceed with account creation
                SqlCommand cmd = new SqlCommand(@"
            INSERT INTO AddStudentAcc (Name, SectionSY, Email, StudentNumber, Department, Role, DateCreated, Status)
            OUTPUT INSERTED.ClientID
            VALUES (@Name, @SectionSY, @Email, @StudentNumber, @Department, @Role, GETDATE(), 'Active')", con);

                cmd.Parameters.AddWithValue("@Name", Name.Text);
                cmd.Parameters.AddWithValue("@SectionSY", SectionSY.Text);
                cmd.Parameters.AddWithValue("@Email", Email.Text);
                cmd.Parameters.AddWithValue("@StudentNumber", StudentNumber.Text);
                cmd.Parameters.AddWithValue("@Department", Department.Text);
                cmd.Parameters.AddWithValue("@Role", Role.Text);

                // ✅ Step 4: Prevent duplicate StudentNumber only if it's not blank
                if (!string.IsNullOrWhiteSpace(StudentNumber.Text))
                {
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM AddStudentAcc WHERE StudentNumber = @StudentNumber", con);
                    checkCmd.Parameters.AddWithValue("@StudentNumber", StudentNumber.Text.Trim());
                    object result = checkCmd.ExecuteScalar();

                    int exists = 0;
                    if (result != null && int.TryParse(result.ToString(), out int value))
                    {
                        exists = value;
                    }

                    if (exists > 0)
                    {
                        MessageBox.Show("This student number already exists!");

                        return;
                    }
                }



                // ✅ Step 5: Insert record
                int clientId = (int)cmd.ExecuteScalar();
                MessageBox.Show("Student record added successfully!");

                // ✅ Step 6: Log activity
                ActivityLog.RecordActivity(
                    SessionData.CurrentUserName,
                    "Create Account",
                    "Account Management",
                    $"Created new student account — Name: {Name.Text}, Student No: {StudentNumber.Text}, Department: {Department.Text}"
                );

                // ✅ Step 7: Generate barcode PDF (unchanged from your version)
                var writer = new BarcodeWriter<Bitmap>
                {
                    Format = BarcodeFormat.CODE_128,
                    Renderer = new SimpleBitmapRenderer(),
                    Options = new EncodingOptions
                    {
                        Width = 300,
                        Height = 80,
                        Margin = 2
                    }
                };

                Bitmap barcodeImage = writer.Write(clientId.ToString());

                PdfDocument pdf = new PdfDocument();
                PdfPage page = pdf.AddPage();
                page.Width = XUnit.FromMillimeter(58);
                page.Height = XUnit.FromMillimeter(40);
                XGraphics gfx = XGraphics.FromPdfPage(page);

                var fontHeader = new XFont("Arial", 10);
                var fontRegular = new XFont("Arial", 8);

                double margin = XUnit.FromMillimeter(3);
                double labelWidth = page.Width - margin * 2;

                gfx.DrawString("CGC Library System", fontHeader, XBrushes.Black,
                    new XRect(margin, margin, labelWidth, 10), XStringFormats.TopCenter);

                double barcodeWidth = XUnit.FromMillimeter(45);
                double barcodeHeight = XUnit.FromMillimeter(15);
                double barcodeX = (page.Width - barcodeWidth) / 2;
                double barcodeY = margin + 11;

                using (MemoryStream ms = new MemoryStream())
                {
                    barcodeImage.Save(ms, ImageFormat.Png);
                    XImage xImage = XImage.FromStream(ms);
                    gfx.DrawImage(xImage, barcodeX, barcodeY, barcodeWidth, barcodeHeight);
                }

                double textStart = barcodeY + barcodeHeight + XUnit.FromMillimeter(1.5);
                string shortName = Name.Text.Length > 18 ? Name.Text.Substring(0, 17) + "…" : Name.Text;

                gfx.DrawString($"ID: {clientId}", fontRegular, XBrushes.Black,
                    new XRect(margin, textStart, labelWidth, 10), XStringFormats.TopCenter);
                gfx.DrawString(shortName, fontRegular, XBrushes.Black,
                    new XRect(margin, textStart + 7, labelWidth, 10), XStringFormats.TopCenter);
                gfx.DrawString(Role.Text, fontRegular, XBrushes.Black,
                    new XRect(margin, textStart + 14, labelWidth, 10), XStringFormats.TopCenter);

                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Library_Barcodes");
                Directory.CreateDirectory(folderPath);
                string pdfPath = Path.Combine(folderPath, $"Client_{clientId}.pdf");
                pdf.Save(pdfPath);
                pdf.Close();
                barcodeImage.Dispose();

                MessageBox.Show($"Label PDF created:\n{pdfPath}");

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = pdfPath,
                    UseShellExecute = true
                });

                LoadStudentAccounts();
            }

            // --- Clear fields ---
            Name.Text = SectionSY.Text = Email.Text = StudentNumber.Text = Department.Text = "";
            Role.Text = "";
        }


        private void AddStudentAccDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



        public void RefreshSemesterButtons()
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = "SELECT COUNT(*) FROM SemesterDuration WHERE IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        int activeSemesters = Convert.ToInt32(cmd.ExecuteScalar());

                        btnStartSem.Enabled = activeSemesters == 0;
                        btnEndSem.Enabled = activeSemesters > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnStartSem.Enabled = false;
                btnEndSem.Enabled = false;
            }
        }

        //private void HighlightStudentStatusRows()
        //{
        //    foreach (DataGridViewRow row in AddStudentAccDataGrid.Rows)
        //    {
        //        if (row.Cells["Status"].Value != null)
        //        {
        //            string status = row.Cells["Status"].Value.ToString();

        //            if (status.Equals("With Pending Issues", StringComparison.OrdinalIgnoreCase))
        //            {
        //                row.DefaultCellStyle.BackColor = Color.LightCoral;
        //                row.DefaultCellStyle.ForeColor = Color.White;
        //            }
                  
        //        }
        //    }
        //}




        private void arthanPanel1_Paint(object sender, PaintEventArgs e)
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

        private void arthanButton5_Load_1(object sender, EventArgs e)
        {
            // Add options for Role combo box
            Role.Items.AddRange(new string[] { "Student", "Faculty" });
            Role.SelectedIndex = 0; // default selection (optional)
        }

        private void Role_TabIndexChanged(object sender, EventArgs e)
        {

        }

        private void arthanPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Role_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedRole = Role.SelectedItem.ToString();

            if (selectedRole == "Student")
            {
                // Disable Department
                Department.Enabled = false;
                Department.BackColor = Color.LightGray;

                // Enable the rest
                Email.Enabled = true;
                Email.BackColor = Color.White;

                SectionSY.Enabled = true;
                SectionSY.BackColor = Color.White;

                StudentNumber.Enabled = true;
                StudentNumber.BackColor = Color.White;
            }
            else if (selectedRole == "Faculty")
            {
                // Disable Student fields
                Email.Enabled = false;
                Email.BackColor = Color.LightGray;

                SectionSY.Enabled = false;
                SectionSY.BackColor = Color.LightGray;

                StudentNumber.Enabled = false;
                StudentNumber.BackColor = Color.LightGray;

                // Enable Department
                Department.Enabled = true;
                Department.BackColor = Color.White;
            }
        }

        private void btnStartSem_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
            {
                con.Open();

                // Check if a semester is already active
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM SemesterDuration WHERE IsActive = 1", con);
                int activeSemCount = (int)checkCmd.ExecuteScalar();

                if (activeSemCount > 0)
                {
                    DialogResult result = MessageBox.Show(
                        "⚠️ WARNING: A semester is already active!\n\n" +
                        "Starting a new semester will:\n" +
                        "• Deactivate the current semester\n" +
                        "• Begin a fresh semester period\n\n" +
                        "Do you want to continue?",
                        "Confirm Start New Semester",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.No)
                        return;

                    // ✅ FIXED: Only deactivate old semester (students already handled by End Semester)
                    SqlCommand deactivateOld = new SqlCommand(
                        "UPDATE SemesterDuration SET IsActive = 0 WHERE IsActive = 1", con);
                    deactivateOld.ExecuteNonQuery();
                }
                else
                {
                    // No active semester - show confirmation
                    DialogResult result = MessageBox.Show(
                        "📅 Start New Semester\n\n" +
                        "This will:\n" +
                        "• Begin a new academic semester\n" +
                        "• Allow creation of new student accounts\n" +
                        "• Set semester duration to 6 months (can be extended later)\n\n" +
                        "Do you want to start the semester?",
                        "Confirm Start Semester",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.No)
                        return;
                }

                // Start new semester
                SqlCommand insertCmd = new SqlCommand("INSERT INTO SemesterDuration (StartDate, IsActive) VALUES (GETDATE(), 1)", con);
                insertCmd.ExecuteNonQuery();

                MessageBox.Show("✅ Semester started successfully!\n\nYou can now create student accounts for this semester.",
                    "Semester Started",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Log activity
                ActivityLog.RecordActivity(
                    SessionData.CurrentUserName,
                    "Start Semester",
                    "Account Management",
                    "New semester started manually."
                );

                RefreshSemesterButtons();
                LoadStudentAccounts();
            }
        }


        // Replace the btnEndSem_Click method in CreateAcc.cs with this updated version:

        private void btnEndSem_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
            {
                con.Open();

                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM SemesterDuration WHERE IsActive = 1", con);
                int activeSemCount = (int)checkCmd.ExecuteScalar();

                if (activeSemCount == 0)
                {
                    MessageBox.Show("There's no active semester to end.", "No Active Semester", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // ✅ Get count of active students
                SqlCommand countCmd = new SqlCommand("SELECT COUNT(*) FROM AddStudentAcc WHERE Status = 'Active'", con);
                int activeStudentCount = (int)countCmd.ExecuteScalar();

                // ✅ Get count of students with penalties or issues (who will REMAIN ACTIVE)
                SqlCommand penaltyCmd = new SqlCommand(@"
            SELECT COUNT(DISTINCT ClientID) 
            FROM IssueBooks 
            WHERE (Penalty > 0 OR Status = 'Overdue' OR Status = 'Report filed by librarian') 
            AND (Status != 'Returned')", con);
                int studentsWithIssues = (int)penaltyCmd.ExecuteScalar();

                // ✅ Get total unreturned books
                SqlCommand unreturnedCmd = new SqlCommand(@"
            SELECT COUNT(*) 
            FROM IssueBooks 
            WHERE Status IN ('Issued', 'Overdue', 'Report filed by librarian')", con);
                int unreturnedBooks = (int)unreturnedCmd.ExecuteScalar();

                // ✅ Calculate students who will be deactivated (active students WITHOUT issues)
                int studentsToDeactivate = activeStudentCount - studentsWithIssues;

                // ✅ Show detailed warning with transaction summary
                // Around line 460 - Update the warning message
                string warningMessage = $"⚠️ WARNING: End Current Semester\n\n" +
                    $"SEMESTER SUMMARY:\n" +
                    $"• Total active students: {activeStudentCount}\n" +
                    $"• Students with penalties/issues (will be marked 'With Pending Issues'): {studentsWithIssues}\n" +
                    $"• Students to be deactivated (clean records): {studentsToDeactivate}\n" +
                    $"• Unreturned books: {unreturnedBooks}\n\n" +
                    $"WHAT WILL HAPPEN:\n" +
                    $"• {studentsToDeactivate} student accounts with clean records will be set to INACTIVE\n" +
                    $"• {studentsWithIssues} students with penalties/issues will be marked as 'WITH PENDING ISSUES'\n" +
                    $"• Penalties will be recorded in PendingPenalties table\n" +
                    $"• Unreturned books ({unreturnedBooks}) will remain tracked and block future borrowing\n" +
                    $"• All completed transactions will be archived\n" +
                    $"• The semester will be closed\n\n" +
                    $"⚠️ This action cannot be easily undone!\n\n" +
                    $"Do you want to proceed with ending the semester?";

                DialogResult result = MessageBox.Show(
                    warningMessage,
                    "Confirm End Semester",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No)
                    return;

                // Around line 490 - Update the final confirmation message
                DialogResult finalConfirm = MessageBox.Show(
                    "Are you absolutely sure?\n\n" +
                    $"This will:\n" +
                    $"• Deactivate {studentsToDeactivate} accounts with clean records\n" +
                    $"• Mark {studentsWithIssues} accounts as 'WITH PENDING ISSUES'\n" +
                    $"• Track {unreturnedBooks} unreturned books\n\n" +
                    "Click YES to confirm.",
                    "Final Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation
                );

                if (finalConfirm == DialogResult.No)
                    return;

                // ✅ Start transaction to handle all semester-end operations
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // ✅ 1. Create PendingPenalties table if it doesn't exist
                    string createPenaltiesTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PendingPenalties' AND xtype='U')
                CREATE TABLE PendingPenalties (
                    PenaltyID INT IDENTITY(1,1) PRIMARY KEY,
                    ClientID INT NOT NULL,
                    ClientName NVARCHAR(255),
                    BookTitle NVARCHAR(255),
                    ISBN NVARCHAR(50),
                    IssueDate DATETIME,
                    DueDate DATETIME,
                    PenaltyAmount DECIMAL(10,2),
                    Status NVARCHAR(50),
                    Reason NVARCHAR(MAX),
                    SemesterEnded DATETIME DEFAULT GETDATE(),
                    IsPaid BIT DEFAULT 0,
                    DatePaid DATETIME NULL
                )";
                    new SqlCommand(createPenaltiesTable, con, transaction).ExecuteNonQuery();

                    // ✅ 2. Move students with penalties to PendingPenalties table (for historical record)
                    string movePenalties = @"
                INSERT INTO PendingPenalties 
                    (ClientID, ClientName, BookTitle, ISBN, IssueDate, DueDate, PenaltyAmount, Status, Reason)
                SELECT 
                    i.ClientID,
                    i.StudentName,
                    i.BookTitle,
                    i.ISBN,
                    i.IssueDate,
                    i.DueDate,
                    i.Penalty,
                    i.Status,
                    CASE 
                        WHEN i.Status = 'Overdue' THEN 'Book overdue - ₱' + CAST(i.Penalty AS NVARCHAR) + ' penalty'
                        WHEN i.Status = 'Report filed by librarian' THEN 'Damage/Loss reported by librarian'
                        ELSE 'Unreturned book'
                    END
                FROM IssueBooks i
                WHERE (i.Penalty > 0 OR i.Status = 'Overdue' OR i.Status = 'Report filed by librarian')
                AND i.Status != 'Returned'";
                    new SqlCommand(movePenalties, con, transaction).ExecuteNonQuery();

                    // ✅ 3. Archive completed transactions to SemesterArchive
                    string createArchiveTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SemesterArchive' AND xtype='U')
                CREATE TABLE SemesterArchive (
                    ArchiveID INT IDENTITY(1,1) PRIMARY KEY,
                    OriginalIssueID INT,
                    ClientID INT,
                    ClientName NVARCHAR(255),
                    BookTitle NVARCHAR(255),
                    ISBN NVARCHAR(50),
                    IssueDate DATETIME,
                    DueDate DATETIME,
                    ReturnDate DATETIME,
                    Status NVARCHAR(50),
                    Penalty DECIMAL(10,2),
                    SemesterEnded DATETIME DEFAULT GETDATE()
                )";
                    new SqlCommand(createArchiveTable, con, transaction).ExecuteNonQuery();

                    string archiveCompleted = @"
                INSERT INTO SemesterArchive 
                    (OriginalIssueID, ClientID, ClientName, BookTitle, ISBN, IssueDate, DueDate, ReturnDate, Status, Penalty)
                SELECT 
                    IssueID, ClientID, StudentName, BookTitle, ISBN, IssueDate, DueDate, ReturnDate, Status, Penalty
                FROM IssueBooks
                WHERE Status = 'Returned'";
                    new SqlCommand(archiveCompleted, con, transaction).ExecuteNonQuery();

                    // ✅ 4. Remove completed transactions (returned books)
                    string deleteReturned = "DELETE FROM IssueBooks WHERE Status = 'Returned'";
                    new SqlCommand(deleteReturned, con, transaction).ExecuteNonQuery();

                    // ✅ 5. CRITICAL: Update ONLY students WITHOUT penalties/issues to Inactive
                    // Students with penalties or "Report filed by librarian" stay ACTIVE
                    // ✅ 5. Update student statuses based on their records
                    // Students with penalties/issues → "With Pending Issues"
                    // Clean students → "Inactive"

                    // First, mark students with penalties/issues
                    string markPendingIssues = @"
    UPDATE AddStudentAcc 
    SET Status = 'With Pending Issues'
    WHERE ClientID IN (
        SELECT DISTINCT ClientID 
        FROM IssueBooks 
        WHERE (Penalty > 0 OR Status = 'Overdue' OR Status = 'Report filed by librarian' OR Status = 'Issued')
        AND Status != 'Returned'
    )";
                    new SqlCommand(markPendingIssues, con, transaction).ExecuteNonQuery();

                    // Then, deactivate students with clean records
                    string deactivateCleanStudents = @"
    UPDATE AddStudentAcc 
    SET Status = 'Inactive'
    WHERE Status = 'Active'";  // Only affects students still marked as Active
                    new SqlCommand(deactivateCleanStudents, con, transaction).ExecuteNonQuery();

                    // ✅ 6. End the semester
                    string endSemester = "UPDATE SemesterDuration SET IsActive = 0 WHERE IsActive = 1";
                    new SqlCommand(endSemester, con, transaction).ExecuteNonQuery();

                    // ✅ Commit all changes
                    transaction.Commit();

                    // Around line 560 - Update the success message
                    MessageBox.Show(
                        $"✅ Semester ended successfully!\n\n" +
                        $"SUMMARY:\n" +
                        $"• {studentsToDeactivate} accounts with clean records moved to inactive status\n" +
                        $"• {studentsWithIssues} students marked as 'WITH PENDING ISSUES'\n" +
                        $"• {unreturnedBooks} unreturned books being tracked\n" +
                        $"• All completed transactions archived\n\n" +
                        $"IMPORTANT NOTES:\n" +
                        $"• Students with pending issues cannot borrow until resolved\n" +
                        $"• Check 'Pending Penalties' report for outstanding issues\n" +
                        $"• Clean accounts can be reactivated when new semester starts",
                        "Semester Ended Successfully",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Around line 575 - Update the activity log
                    ActivityLog.RecordActivity(
                        SessionData.CurrentUserName,
                        "End Semester",
                        "Account Management",
                        $"Semester ended — {studentsToDeactivate} clean accounts deactivated, {studentsWithIssues} accounts marked 'With Pending Issues', {unreturnedBooks} books tracked"
                    );

                    RefreshSemesterButtons();
                    MoveInactiveStudents();
                    LoadStudentAccounts();

                    // ✅ Update Issue form if it's open
                    var issueForm = Application.OpenForms.OfType<Issue>().FirstOrDefault();
                    if (issueForm != null)
                    {
                        issueForm.LoadIssueBooks();
                        issueForm.LoadReturnedBooks();
                        issueForm.UpdateTotalOverdueLabel();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show(
                        $"Error ending semester:\n\n{ex.Message}\n\nAll changes have been rolled back.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void CheckSemesterStatus()
        {
            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SemesterDuration WHERE IsActive = 1", con);
                int activeSemCount = (int)cmd.ExecuteScalar();

                btnStartSem.Enabled = activeSemCount == 0;
                btnEndSem.Enabled = activeSemCount > 0;
            }
        }

        private void Email_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnActivateStudent_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        // Replace your btnActivateAccount_Click method with this:
        private void btnActivateAccount_Click(object sender, EventArgs e)
        {
            string clientID = ActivateClientID.Text.Trim();

            if (string.IsNullOrWhiteSpace(clientID))
            {
                MessageBox.Show("Please enter a valid Client ID.", "Missing ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
            {
                con.Open();

                // ✅ 1. Check if the semester has started
                string semCheckQuery = "SELECT TOP 1 StartDate, DurationMonths FROM SemesterDuration WHERE IsActive = 1";
                using (SqlCommand semCheckCmd = new SqlCommand(semCheckQuery, con))
                {
                    using (SqlDataReader reader = semCheckCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DateTime startDate = Convert.ToDateTime(reader["StartDate"]);
                            int durationMonths = Convert.ToInt32(reader["DurationMonths"]);

                            if (DateTime.Now < startDate)
                            {
                                MessageBox.Show(
                                    $"The semester has not started yet.\n\n" +
                                    $"Start Date: {startDate:MMMM dd, yyyy}\n" +
                                    $"You can only activate students once the semester begins.",
                                    "Semester Not Started",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                                return; // stop activation
                            }
                        }
                        else
                        {
                            MessageBox.Show(
                                "No active semester found.\nPlease create or start a new semester before activating students.",
                                "No Active Semester",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                            return; // stop activation
                        }
                    }
                }

                // ✅ 2. Check if the client exists in InactiveStudents
                string checkQuery = "SELECT COUNT(*) FROM InactiveStudents WHERE ClientID = @ClientID";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@ClientID", clientID);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists == 0)
                    {
                        MessageBox.Show("No inactive student found with this Client ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // ✅ 3. Retrieve the student's info from InactiveStudents
                string selectQuery = "SELECT Name, Role FROM InactiveStudents WHERE ClientID = @ClientID";
                string studentName = "";
                string studentRole = "";

                using (SqlCommand cmd = new SqlCommand(selectQuery, con))
                {
                    cmd.Parameters.AddWithValue("@ClientID", clientID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            studentName = reader["Name"].ToString();
                            studentRole = reader["Role"].ToString();
                        }
                    }
                }

                // ✅ 4. Move the record from InactiveStudents → AddStudentAcc
                try
                {
                    string moveQuery = @"
                SET IDENTITY_INSERT AddStudentAcc ON;

                INSERT INTO AddStudentAcc (ClientID, Name, YearLevel, SectionSY, Email, StudentNumber, Department, Semester, Role, DateCreated, Status)
                SELECT ClientID, Name, YearLevel, SectionSY, Email, StudentNumber, Department, Semester, Role, DateCreated, 'Active'
                FROM InactiveStudents
                WHERE ClientID = @ClientID;

                SET IDENTITY_INSERT AddStudentAcc OFF;

                DELETE FROM InactiveStudents WHERE ClientID = @ClientID;";

                    using (SqlCommand moveCmd = new SqlCommand(moveQuery, con))
                    {
                        moveCmd.Parameters.AddWithValue("@ClientID", clientID);
                        moveCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("✅ Student account successfully activated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear input fields
                    ActivateClientID.Text = "";
                    ActivateName.Text = "";
                    ActivateRole.Text = "";

                    // ✅ Log the activation
                    ActivityLog.RecordActivity(
                        SessionData.CurrentUserName,
                        "Activate Student",
                        "Account Management",
                        $"Activated student account — ClientID: {clientID}, Name: {studentName}"
                    );

                    LoadStudentAccounts(); // refresh table
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error activating account: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        // Update your ActivateClientID_TextChanged to query InactiveStudents:
        private void ActivateClientID_TextChanged(object sender, EventArgs e)
        {
            string clientID = ActivateClientID.Text.Trim();

            if (clientID.Length == 4)
            {
                using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
                {
                    try
                    {
                        con.Open();
                        string query = "SELECT Name, Role FROM InactiveStudents WHERE ClientID = @ClientID";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@ClientID", clientID);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    ActivateName.Text = reader["Name"].ToString();
                                    ActivateRole.Text = reader["Role"].ToString();
                                    btnActivateAccount.Focus();
                                }
                                else
                                {
                                    ActivateName.Text = "";
                                    ActivateRole.Text = "";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (clientID.Length < 4)
            {
                ActivateName.Text = "";
                ActivateRole.Text = "";
            }
        }

        // Replace your MoveInactiveStudents() method with this:
        public void MoveInactiveStudents()
        {
            string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // ✅ Ensure InactiveStudents table exists
                string ensureTables = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='InactiveStudents' AND xtype='U')
        CREATE TABLE InactiveStudents (
            ClientID INT PRIMARY KEY,
            Name NVARCHAR(255),
            YearLevel NVARCHAR(50),
            SectionSY NVARCHAR(100),
            Email NVARCHAR(255),
            StudentNumber NVARCHAR(100),
            Department NVARCHAR(100),
            Semester NVARCHAR(50),
            Role NVARCHAR(50),
            DateCreated DATETIME,
            Status NVARCHAR(50)
        );";
                new SqlCommand(ensureTables, con).ExecuteNonQuery();

                // ✅ Move Inactive records from AddStudentAcc → InactiveStudents
                string moveToInactive = @"
        -- Insert new inactive records
        INSERT INTO InactiveStudents (ClientID, Name, YearLevel, SectionSY, Email, StudentNumber, Department, Semester, Role, DateCreated, Status)
        SELECT ClientID, Name, YearLevel, SectionSY, Email, StudentNumber, Department, Semester, Role, DateCreated, Status
        FROM AddStudentAcc
        WHERE Status = 'Inactive'
          AND ClientID NOT IN (SELECT ClientID FROM InactiveStudents);

        -- Delete moved records from AddStudentAcc
        DELETE FROM AddStudentAcc WHERE Status = 'Inactive';";

                new SqlCommand(moveToInactive, con).ExecuteNonQuery();
            }
        }





        private void ActivateClientID_KeyDown_1(object sender, KeyEventArgs e)
        {
            // 🔑 Only trigger when user presses Enter
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // prevents the 'ding' sound
                string clientID = ActivateClientID.Text.Trim();

                if (string.IsNullOrWhiteSpace(clientID))
                {
                    MessageBox.Show("Please enter a Client ID first.", "Missing Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
                {
                    try
                    {
                        con.Open();
                        string query = "SELECT Name, Role FROM InactiveStudents WHERE ClientID = @ClientID";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@ClientID", clientID);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // ✅ Auto-fill
                                    ActivateName.Text = reader["Name"].ToString();
                                    ActivateRole.Text = reader["Role"].ToString();

                                    // ✅ Move focus to Activate button
                                    btnActivateAccount.Focus();
                                }
                                else
                                {
                                    // ❌ Not found message
                                    ActivateName.Text = "";
                                    ActivateRole.Text = "";
                                    MessageBox.Show("Student record not found.", "No Record", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    ActivateClientID.Text = "";
                                    ActivateName.Text = "";
                                    ActivateRole.Text = "";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void txtPrintClientID_TextChanged(object sender, EventArgs e)
        {
            string clientID = txtPrintClientID.Text.Trim();

            if (clientID.Length >= 4) // Check when at least 4 digits entered
            {
                using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
                {
                    try
                    {
                        con.Open();
                        // Check in both active and inactive students
                        string query = @"
                    SELECT Name, Role FROM AddStudentAcc WHERE ClientID = @ClientID
                    UNION
                    SELECT Name, Role FROM InactiveStudents WHERE ClientID = @ClientID";

                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@ClientID", clientID);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    txtPrintName.Text = reader["Name"].ToString();
                                    txtPrintRole.Text = reader["Role"].ToString();
                                }
                                else
                                {
                                    txtPrintName.Text = "";
                                    txtPrintRole.Text = "";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                txtPrintName.Text = "";
                txtPrintRole.Text = "";
            }
        }

        private void btnPrintBarcode_Click(object sender, EventArgs e)
        {
            string clientID = txtPrintClientID.Text.Trim();
            string name = txtPrintName.Text.Trim();
            string role = txtPrintRole.Text.Trim();

            // Validation
            if (string.IsNullOrWhiteSpace(clientID))
            {
                MessageBox.Show("Please enter a Client ID.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrintClientID.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Student record not found. Please check the Client ID.", "Invalid ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Generate barcode using ZXing
                var writer = new BarcodeWriter<Bitmap>
                {
                    Format = BarcodeFormat.CODE_128,
                    Renderer = new SimpleBitmapRenderer(),
                    Options = new EncodingOptions
                    {
                        Width = 300,
                        Height = 80,
                        Margin = 2
                    }
                };

                Bitmap barcodeImage = writer.Write(clientID);

                // Create PDF document
                PdfDocument pdf = new PdfDocument();
                PdfPage page = pdf.AddPage();
                page.Width = XUnit.FromMillimeter(58);
                page.Height = XUnit.FromMillimeter(40);
                XGraphics gfx = XGraphics.FromPdfPage(page);

                var fontHeader = new XFont("Arial", 10);
                var fontRegular = new XFont("Arial", 8);

                double margin = XUnit.FromMillimeter(3);
                double labelWidth = page.Width - margin * 2;

                // Draw header
                gfx.DrawString("CGC Library System", fontHeader, XBrushes.Black,
                    new XRect(margin, margin, labelWidth, 10), XStringFormats.TopCenter);

                // Draw barcode
                double barcodeWidth = XUnit.FromMillimeter(45);
                double barcodeHeight = XUnit.FromMillimeter(15);
                double barcodeX = (page.Width - barcodeWidth) / 2;
                double barcodeY = margin + 11;

                using (MemoryStream ms = new MemoryStream())
                {
                    barcodeImage.Save(ms, ImageFormat.Png);
                    XImage xImage = XImage.FromStream(ms);
                    gfx.DrawImage(xImage, barcodeX, barcodeY, barcodeWidth, barcodeHeight);
                }

                // Draw text information
                double textStart = barcodeY + barcodeHeight + XUnit.FromMillimeter(1.5);
                string shortName = name.Length > 18 ? name.Substring(0, 17) + "…" : name;

                gfx.DrawString($"ID: {clientID}", fontRegular, XBrushes.Black,
                    new XRect(margin, textStart, labelWidth, 10), XStringFormats.TopCenter);
                gfx.DrawString(shortName, fontRegular, XBrushes.Black,
                    new XRect(margin, textStart + 7, labelWidth, 10), XStringFormats.TopCenter);
                gfx.DrawString(role, fontRegular, XBrushes.Black,
                    new XRect(margin, textStart + 14, labelWidth, 10), XStringFormats.TopCenter);

                // Save PDF
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Library_Barcodes");
                Directory.CreateDirectory(folderPath);
                string pdfPath = Path.Combine(folderPath, $"Client_{clientID}_Reprint.pdf");
                pdf.Save(pdfPath);
                pdf.Close();
                barcodeImage.Dispose();

                MessageBox.Show($"Barcode reprinted successfully!\n{pdfPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Log the reprint activity
                ActivityLog.RecordActivity(
                    SessionData.CurrentUserName,
                    "Reprint Barcode",
                    "Account Management",
                    $"Reprinted barcode for ClientID: {clientID}, Name: {name}, Role: {role}"
                );

                // Open the PDF
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = pdfPath,
                    UseShellExecute = true
                });

                // Clear fields after successful print
                txtPrintClientID.Text = "";
                txtPrintName.Text = "";
                txtPrintRole.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating barcode: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPrintClientID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (!string.IsNullOrWhiteSpace(txtPrintClientID.Text) &&
                    !string.IsNullOrWhiteSpace(txtPrintName.Text))
                {
                    btnPrintBarcode.Focus();
                    btnPrintBarcode.PerformClick();
                }
                else
                {
                    MessageBox.Show("Please enter a valid Client ID.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void AddStudentAccDataGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

          






        }
    }
}