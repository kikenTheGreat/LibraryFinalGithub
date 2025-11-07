using LibraryCGC;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Library_Final
{
    public partial class ManageProfileForm : Form
    {
        private string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";
        private int currentEmployeeID; // store logged-in user's ID
        public ManageProfileForm(int employeeId)
        {
            InitializeComponent();
            currentEmployeeID = employeeId;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void ManageProfileForm_Load(object sender, EventArgs e)
        {
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            string query = "SELECT * FROM Employees WHERE EmployeeID = @EmployeeID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    txtFirstName.Text = reader["FirstName"].ToString();
                    txtLastName.Text = reader["LastName"].ToString();
                    txtDepartment.Text = reader["Department"].ToString();
                    txtPosition.Text = reader["Position"].ToString();
                    txtPhoneNumber.Text = reader["PhoneNumber"].ToString();
                    txtEmail.Text = reader["EmailAddress"].ToString();
                    txtUsername.Text = reader["Username"].ToString();

                    // Load profile image
                    if (reader["ProfileImage"] != DBNull.Value)
                    {
                        byte[] imgData = (byte[])reader["ProfileImage"];
                        using (MemoryStream ms = new MemoryStream(imgData))
                        {
                            picProfile.Image = Image.FromStream(ms);
                        }
                    }
                }
                reader.Close();
            }
        }

        private void btnChangeImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Dispose previous image if needed
                    if (picProfile.Image != null)
                    {
                        picProfile.Image.Dispose();
                        picProfile.Image = null;
                    }

                    // ✅ Clone the image so it’s not locked by the file system
                    using (Image temp = Image.FromFile(ofd.FileName))
                    {
                        picProfile.Image = new Bitmap(temp);
                    }

                    picProfile.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }


        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] imageBytes = ImageToByteArray(picProfile.Image);

                string query = @"
        UPDATE Employees
        SET FirstName = @FirstName,
            LastName = @LastName,
            Department = @Department,
            Position = @Position,
            PhoneNumber = @PhoneNumber,
            EmailAddress = @EmailAddress,
            Username = @Username,
            Password = @Password,
            ProfileImage = @ProfileImage
        WHERE EmployeeID = @EmployeeID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", currentEmployeeID);
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@Department", txtDepartment.Text);
                    cmd.Parameters.AddWithValue("@Position", txtPosition.Text);
                    cmd.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text);
                    cmd.Parameters.AddWithValue("@EmailAddress", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);

                    if (imageBytes != null)
                        cmd.Parameters.AddWithValue("@ProfileImage", imageBytes);
                    else
                        cmd.Parameters.AddWithValue("@ProfileImage", DBNull.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // 🖼️ Converts a PictureBox image into a byte array (safe for database storage)
        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Use PNG to avoid GDI+ issues with RawFormat
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }



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

        private void guna2Button1_Click(object sender, EventArgs e)
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

        private void arthanPanel3_Paint(object sender, PaintEventArgs e)
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

            // ✅ Use SessionData when creating new Form1
            Form1 form1 = new Form1(SessionData.CurrentEmployeeID);
            form1.Show();
            this.Hide();

        }
    }
}
