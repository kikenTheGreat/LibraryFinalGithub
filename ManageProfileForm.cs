using LibraryCGC;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Library_Final
{
    public partial class ManageProfileForm : Form
    {
        private readonly string connectionString =
            "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;";

        private int currentEmployeeID; // Store logged-in user's ID

        public ManageProfileForm(int employeeId)
        {
            InitializeComponent();
            currentEmployeeID = employeeId;
        }

        private void ManageProfileForm_Load(object sender, EventArgs e)
        {
            LoadEmployeeData();
        }

        // 🧾 Load employee data into form fields
        private void LoadEmployeeData()
        {
            string query = "SELECT * FROM Employees WHERE EmployeeID = @EmployeeID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeID);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtFirstName.Text = reader["FirstName"].ToString();
                        txtLastName.Text = reader["LastName"].ToString();
                        txtDepartment.Text = reader["Department"].ToString();
                        txtPosition.Text = reader["Position"].ToString();
                        txtPhoneNumber.Text = reader["PhoneNumber"].ToString();
                        txtEmail.Text = reader["EmailAddress"].ToString();
                        txtUsername.Text = reader["Username"].ToString();

                        // Don't load password for security reasons
                        txtPassword.Text = "";
                        if (picProfile.Image != null && currentImageBytes == null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                picProfile.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                currentImageBytes = ms.ToArray();
                            }
                        }

                    }
                }
            }
        }
        // Add a field to store image bytes
        private byte[] currentImageBytes = null;

        // 🖼️ Change profile picture
        private void btnChangeImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Dispose of previous image safely
                    if (picProfile.Image != null)
                    {
                        picProfile.Image.Dispose();
                        picProfile.Image = null;
                    }

                    // Read file into byte array first (avoids file lock)
                    currentImageBytes = File.ReadAllBytes(ofd.FileName);

                    // Create image from byte array for display
                    using (MemoryStream ms = new MemoryStream(currentImageBytes))
                    {
                        picProfile.Image = new Bitmap(ms);
                        picProfile.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
        }

        // 💾 Save updated profile data
        // 💾 Save updated profile data
        private void btnSaveChanges_Click(object sender, EventArgs e)
        {


            // Check if email is already used by another employee
            string checkEmailQuery = "SELECT COUNT(*) FROM Employees WHERE EmailAddress = @EmailAddress AND EmployeeID != @EmployeeID";
            using (SqlConnection connCheck = new SqlConnection(connectionString))
            using (SqlCommand cmdCheck = new SqlCommand(checkEmailQuery, connCheck))
            {
                cmdCheck.Parameters.AddWithValue("@EmailAddress", txtEmail.Text.Trim());
                cmdCheck.Parameters.AddWithValue("@EmployeeID", currentEmployeeID);

                connCheck.Open();
                int count = (int)cmdCheck.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("This email address is already in use by another employee. Please use a different email.",
                        "Duplicate Email", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Text="";
                    return;
                }
            }

            try
            {
                // Validate password if user is trying to change it
                bool isChangingPassword = !string.IsNullOrWhiteSpace(txtPassword.Text);

                if (isChangingPassword)
                {
                    if (txtPassword.Text != txtConfirmPassword.Text)
                    {
                        MessageBox.Show("Passwords do not match!", "Validation Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Build dynamic query based on whether password is being changed
                string query;
                if (isChangingPassword)
                {
                    query = @"
                        UPDATE Employees
                        SET FirstName = @FirstName,
                            LastName = @LastName,
                            Department = @Department,
                            Position = @Position,
                            PhoneNumber = @PhoneNumber,
                            EmailAddress = @EmailAddress,
                            Username = @Username,
                            Password = @Password,
                            ProfileImage = @ProfileImage,
                            LastUpdated = GETDATE()
                        WHERE EmployeeID = @EmployeeID";
                }
                else
                {
                    query = @"
                        UPDATE Employees
                        SET FirstName = @FirstName,
                            LastName = @LastName,
                            Department = @Department,
                            Position = @Position,
                            PhoneNumber = @PhoneNumber,
                            EmailAddress = @EmailAddress,
                            Username = @Username,
                            ProfileImage = @ProfileImage,
                            LastUpdated = GETDATE()
                        WHERE EmployeeID = @EmployeeID";
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeID);
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Department", string.IsNullOrWhiteSpace(txtDepartment.Text) ? (object)DBNull.Value : txtDepartment.Text.Trim());
                    cmd.Parameters.AddWithValue("@Position", string.IsNullOrWhiteSpace(txtPosition.Text) ? (object)DBNull.Value : txtPosition.Text.Trim());
                    cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(txtPhoneNumber.Text) ? (object)DBNull.Value : txtPhoneNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmailAddress", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());

                    // Only add password parameter if changing password
                    if (isChangingPassword)
                    {
                        cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                    }

                    if (currentImageBytes != null && currentImageBytes.Length > 0)
                    {
                        cmd.Parameters.Add("@ProfileImage", SqlDbType.VarBinary, -1).Value = currentImageBytes;
                    }
                    else
                    {
                        cmd.Parameters.Add("@ProfileImage", SqlDbType.VarBinary, -1).Value = DBNull.Value;
                    }


                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Profile updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Clear password fields after successful update
                    txtPassword.Text = "";
                    txtConfirmPassword.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 🧠 Convert PictureBox image to byte array
        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        // 🔒 Hash password (if you want to implement this later)
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        // 🔙 Back button handler
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
            Form1 form1 = new Form1(SessionData.CurrentEmployeeID);
            form1.Show();
            this.Hide();
        }
    }
}
