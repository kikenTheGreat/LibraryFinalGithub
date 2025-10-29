using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using Library_Final;

namespace LibraryCGC
{
    public partial class sign_in : Form
    {
        private string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";
        public sign_in()
        {
            InitializeComponent();
        }

        private void btnIssueBooks_Click(object sender, EventArgs e)
        {
            try
            {
                // === VALIDATION ===
                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("Passwords do not match!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (picProfile.Image == null)
                {
                    MessageBox.Show("Please upload a profile picture.", "Missing Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Convert image to byte array
                byte[] imageBytes = ImageToByteArray(picProfile.Image);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // === SPECIFIC DUPLICATE CHECK ===
                    string checkQuery = @"
                SELECT 
                    CASE 
                        WHEN EXISTS (SELECT 1 FROM Employees WHERE EmployeeCode = @EmployeeCode) THEN 'Employee Code'
                        WHEN EXISTS (SELECT 1 FROM Employees WHERE Username = @Username) THEN 'Username'
                        WHEN EXISTS (SELECT 1 FROM Employees WHERE EmailAddress = @EmailAddress) THEN 'Email Address'
                        ELSE NULL
                    END AS DuplicateField";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@EmployeeCode", txtEmployeeID.Text);
                        checkCmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                        checkCmd.Parameters.AddWithValue("@EmailAddress", txtEmail.Text);

                        object duplicateField = checkCmd.ExecuteScalar();

                        if (duplicateField != DBNull.Value && duplicateField != null)
                        {
                            MessageBox.Show($"{duplicateField} already exists!", "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // === INSERT NEW EMPLOYEE ===
                    string query = @"
                INSERT INTO Employees 
                (EmployeeCode, FirstName, LastName, Department, Position, PhoneNumber, EmailAddress, Username, Password, ProfileImage)
                VALUES 
                (@EmployeeCode, @FirstName, @LastName, @Department, @Position, @PhoneNumber, @EmailAddress, @Username, @Password, @ProfileImage)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeCode", txtEmployeeID.Text);
                        cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                        cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                        cmd.Parameters.AddWithValue("@Department", txtDepartment.Text);
                        cmd.Parameters.AddWithValue("@Position", txtPosition.Text);
                        cmd.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text);
                        cmd.Parameters.AddWithValue("@EmailAddress", txtEmail.Text);
                        cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                        cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                        cmd.Parameters.AddWithValue("@ProfileImage", imageBytes);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("Employee registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();

                            REGISTER registerForm = new REGISTER();
                            registerForm.Show();
                            this.Hide();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // 🖼 Convert Image to Byte Array
        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        // 🧹 Clear all fields after saving
        private void ClearForm()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtEmployeeID.Clear();
            txtDepartment.Clear();
            txtPosition.Clear();
            txtPhoneNumber.Clear();
            txtEmail.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            picProfile.Image = null;
        }





        private void guna2CustomGradientPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            REGISTER r = new REGISTER();
            r.Show();
            this.Hide();
        }

        private void guna2CustomGradientPanel2_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void btnUploadImage_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    // Allow only image files
                    ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                    ofd.Title = "Select Profile Image";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        // Display the selected image in the PictureBox
                        picProfile.Image = Image.FromFile(ofd.FileName);

                        // Optional: store file path temporarily if needed
                        picProfile.Tag = ofd.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void sign_in_Load(object sender, EventArgs e)
        {

        }

        private void guna2CustomGradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
