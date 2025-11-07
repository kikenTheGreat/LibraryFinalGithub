using Library_Final;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient;
using OnBarcode.Barcode;
using PdfSharp.Drawing;
using PdfSharp.Drawing;
using PdfSharp.Drawing;
using PdfSharp.Drawing;
using ZXing.Rendering;
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
using System.Drawing;          // Bitmap
using System.Drawing.Imaging;  // ImageFormat
using System;
using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Drawing;
using System.Drawing;
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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
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

          

            // --- Role ---
            var colRole = new DataGridViewTextBoxColumn();
            colRole.HeaderText = "Role";
            colRole.DataPropertyName = "Role";
            colRole.Name = "Role";
            colRole.Width = 100;
            AddStudentAccDataGrid.Columns.Add(colRole);

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
                con.Open();

                // Insert new student and get ClientID
                SqlCommand cmd = new SqlCommand(@"
            INSERT INTO AddStudentAcc (Name, SectionSY, Email, StudentNumber, Department, Role)
            OUTPUT INSERTED.ClientID
            VALUES (@Name, @SectionSY, @Email, @StudentNumber, @Department, @Role)", con);

                cmd.Parameters.AddWithValue("@Name", Name.Text);
                cmd.Parameters.AddWithValue("@SectionSY", SectionSY.Text);
                cmd.Parameters.AddWithValue("@Email", Email.Text);
                cmd.Parameters.AddWithValue("@StudentNumber", StudentNumber.Text);
                cmd.Parameters.AddWithValue("@Department", Department.Text);
                cmd.Parameters.AddWithValue("@Role", Role.Text);


                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM AddStudentAcc WHERE StudentNumber = @StudentNumber", con);
                checkCmd.Parameters.AddWithValue("@StudentNumber", StudentNumber.Text);
                int exists = (int)checkCmd.ExecuteScalar();
                // Check if the student number is not blank first


              

           




                int clientId = (int)cmd.ExecuteScalar();
                MessageBox.Show("Student record added successfully!");
                // ✅ Step 2: Log the activity
                ActivityLog.RecordActivity(
                    SessionData.CurrentUserName,
                    "Create Account",
                    "Account Management",
                    $"Created new student account — Name: {Name.Text}, Student No: {StudentNumber.Text}, Department: {Department.Text}"
                );



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

                // --- Create PDF Label (58 mm x 40 mm) ---
                PdfDocument pdf = new PdfDocument();
                PdfPage page = pdf.AddPage();
                page.Width = XUnit.FromMillimeter(58);
                page.Height = XUnit.FromMillimeter(40);
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // --- Fonts ---
                var fontHeader = new XFont("Arial", 10);
                var fontRegular = new XFont("Arial", 8);

                // Margins and layout
                double margin = XUnit.FromMillimeter(3);
                double labelWidth = page.Width - margin * 2;

                // --- Header ---
                gfx.DrawString("CGC Library System", fontHeader, XBrushes.Black,
                    new XRect(margin, margin, labelWidth, 10), XStringFormats.TopCenter);

                // --- Barcode (centered) ---
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

                // --- Text below barcode ---
                double textStart = barcodeY + barcodeHeight + XUnit.FromMillimeter(1.5);

                // Shorten long names if needed
                string shortName = Name.Text.Length > 18 ? Name.Text.Substring(0, 17) + "…" : Name.Text;

                // --- Draw info ---
                gfx.DrawString($"ID: {clientId}", fontRegular, XBrushes.Black,
                    new XRect(margin, textStart, labelWidth, 10), XStringFormats.TopCenter);
                gfx.DrawString(shortName, fontRegular, XBrushes.Black,
                    new XRect(margin, textStart + 7, labelWidth, 10), XStringFormats.TopCenter);
                gfx.DrawString(Role.Text, fontRegular, XBrushes.Black,
                    new XRect(margin, textStart + 14, labelWidth, 10), XStringFormats.TopCenter);

                // --- Save and open ---
                string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Library_Barcodes");
                Directory.CreateDirectory(folderPath);
                string pdfPath = Path.Combine(folderPath, $"Client_{clientId}.pdf");
                pdf.Save(pdfPath);
                pdf.Close();
                barcodeImage.Dispose();

                MessageBox.Show($"Label PDF created:\n{pdfPath}");

                // Automatically open for printing
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
    }
}
