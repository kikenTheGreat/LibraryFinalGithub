using LibraryCGC;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Library_Final
{
    public partial class REGISTER : Form
    {
        private string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";
        public REGISTER()
        {
            InitializeComponent();
        }

        private void REGISTER_Load(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void arthanButton5_Click(object sender, EventArgs e)
        { // 1️⃣ Basic validation
          // 1️⃣ Basic validation
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Missing Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2️⃣ Case-sensitive query using COLLATE
            string query = @"SELECT EmployeeID, Username, FirstName, LastName, ProfileImage 
                     FROM Employees 
                     WHERE Username COLLATE SQL_Latin1_General_CP1_CS_AS = @Username 
                     AND Password COLLATE SQL_Latin1_General_CP1_CS_AS = @Password";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int employeeId = Convert.ToInt32(reader["EmployeeID"]);
                            string username = reader["Username"].ToString();
                            string firstName = reader["FirstName"].ToString();
                            string lastName = reader["LastName"].ToString();

                            // Load profile image
                            Image profileImage = null;
                            if (reader["ProfileImage"] != DBNull.Value)
                            {
                                byte[] imageData = (byte[])reader["ProfileImage"];
                                using (MemoryStream ms = new MemoryStream(imageData))
                                {
                                    profileImage = Image.FromStream(ms);
                                }
                            }

                            // ✅ Initialize session
                            SessionData.InitializeSessionComplete(
                                employeeId,
                                username,
                                firstName,
                                lastName,
                                profileImage
                            );

                            MessageBox.Show("Login successful!", "Welcome",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            Form1 form = new Form1(employeeId);
                            form.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Login Failed",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        // 🧮 Same password hashing function from registration form
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
            sign_in s = new sign_in();
            s.Show();
            this.Hide();
        }

        private void arthanButton5_Load(object sender, EventArgs e)
        {

        }

        private void arthanPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void linkForgotPassword_Click(object sender, EventArgs e)
        {

        }

        private void linkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgotPasswordForm forgot = new ForgotPasswordForm();
            forgot.Show();
            this.Hide();
        }

        private void arthanButton5_Load_1(object sender, EventArgs e)
        {

        }

        private void arthanPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        bool isPasswordVisible = false;
        private void btnTogglePassword_Click(object sender, EventArgs e)
        {
            if (isPasswordVisible)
            {
                // Show password
                txtPassword.UseSystemPasswordChar = false;
                btnTogglePassword.Text = "🙈"; // eye-open icon
                isPasswordVisible = false;
            }
            else
            {
                // Hide password
                txtPassword.UseSystemPasswordChar = true;
                btnTogglePassword.Text = "👁"; // eye-closed icon
                isPasswordVisible = true;
            }


        }
    }
}
