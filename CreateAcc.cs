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
            //output data grid
            SetupAccountGrid();
            LoadStudentAccounts();
            CheckSemesterStatus(); // ✅ check button states on lo
        }

        private void LoadStudentAccounts()   //output data grid
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
            colClientID.Visible = false;
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


                if (Role.Text == "Student"){

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



        private void RefreshSemesterButtons()
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

                // Check if a semester is already active (optional warning only)
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM SemesterDuration WHERE IsActive = 1", con);
                int activeSemCount = (int)checkCmd.ExecuteScalar();

                if (activeSemCount > 0)
                {
                    DialogResult result = MessageBox.Show(
                        "A semester is already active. Do you want to start a new one and deactivate the old one?",
                        "Semester Already Active",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.No)
                        return;

                    // Deactivate old semester first
                    SqlCommand deactivateOld = new SqlCommand("UPDATE SemesterDuration SET IsActive = 0 WHERE IsActive = 1", con);
                    deactivateOld.ExecuteNonQuery();
                }

                // Start new semester
                SqlCommand insertCmd = new SqlCommand("INSERT INTO SemesterDuration (StartDate, IsActive) VALUES (GETDATE(), 1)", con);
                insertCmd.ExecuteNonQuery();

                MessageBox.Show("Semester has started manually. Accounts created now will belong to this semester.");
                RefreshSemesterButtons();


            }
        }


        private void btnEndSem_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"))
            {
                con.Open();

                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM SemesterDuration WHERE IsActive = 1", con);
                int activeSemCount = (int)checkCmd.ExecuteScalar();

                if (activeSemCount == 0)
                {
                    MessageBox.Show("There’s no active semester to end.", "No Active Semester");
                    return;
                }

                SqlCommand endCmd = new SqlCommand(@"
            UPDATE AddStudentAcc SET Status = 'Inactive';
            UPDATE SemesterDuration SET IsActive = 0 WHERE IsActive = 1;", con);
                endCmd.ExecuteNonQuery();

                MessageBox.Show("Semester ended manually. All accounts are now inactive.");

                LoadStudentAccounts();

                ActivityLog.RecordActivity(
                    SessionData.CurrentUserName,
                    "End Semester",
                    "Account Management",
                    "Semester was manually ended — all accounts set to inactive."
                );
                RefreshSemesterButtons();

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
    }
}
