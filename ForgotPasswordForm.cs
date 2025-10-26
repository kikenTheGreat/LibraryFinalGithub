using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;  // For reading App.config
using System.Net.Mail;       // For email sending
using Microsoft.Data.SqlClient;


namespace Library_Final
{
    public partial class ForgotPasswordForm : Form
    {
        private string connectionString = " Data Source=(LocalDB)\\MSSQLLocalDB;\r\nInitial Catalog=LibraryDB;\r\nIntegrated Security=True;\r\nEncrypt=True;\r\nTrust Server Certificate=True;\r\n";
        private string currentOTP = "";
        public ForgotPasswordForm()
        {
            InitializeComponent();
            txtOTP.Enabled = false;
            txtNewPassword.Enabled = false;
            txtConfirmPassword.Enabled = false;
            btnVerifyOTP.Enabled = false;
            btnResetPassword.Enabled = false;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            REGISTER rEGISTER = new REGISTER();
            rEGISTER.Show();
            this.Hide();
        }

        private void btnSendOTP_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter your email address.");
                return;
            }

            // 🔹 Step 1: Check if email exists in the Employees table
            bool emailExists = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Employees WHERE EmailAddress = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    emailExists = count > 0;
                }
            }

            if (!emailExists)
            {
                MessageBox.Show("No account found with this email address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🔹 Step 2: Generate 6-digit OTP
            Random rand = new Random();
            currentOTP = rand.Next(100000, 999999).ToString();

            // 🔹 Step 3: Save OTP and creation time in database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string update = "UPDATE Employees SET OTPCode = @OTP, OTPCreatedAt = GETDATE() WHERE EmailAddress = @Email";
                using (SqlCommand cmd = new SqlCommand(update, conn))
                {
                    cmd.Parameters.AddWithValue("@OTP", currentOTP);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // 🔹 Step 4: Send the OTP email using the system Gmail from App.config
            SendEmailOTP(txtEmail.Text.Trim(), currentOTP);

            MessageBox.Show("OTP sent successfully! Please check your email.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            txtOTP.Enabled = true;
            btnVerifyOTP.Enabled = true;
        }


        private void btnVerifyOTP_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOTP.Text))
            {
                MessageBox.Show("Please enter the OTP code sent to your email.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT OTPCode, OTPCreatedAt FROM Employees WHERE EmailAddress = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedOTP = reader["OTPCode"].ToString();
                            DateTime createdAt = Convert.ToDateTime(reader["OTPCreatedAt"]);

                            if (storedOTP == txtOTP.Text.Trim())
                            {
                                if (DateTime.Now <= createdAt.AddMinutes(5))
                                {
                                    MessageBox.Show("OTP verified! You can now reset your password.");
                                    txtNewPassword.Enabled = true;
                                    txtConfirmPassword.Enabled = true;
                                    btnResetPassword.Enabled = true;
                                }
                                else
                                {
                                    MessageBox.Show("Your OTP has expired. Please resend a new one.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid OTP code.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Email not found in system.");
                        }
                    }
                }
            }
        }


        // 🔹 Step 3: Reset Password
        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match!");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string updateQuery = "UPDATE Employees SET Password = @Password, OTPCode = NULL, OTPCreatedAt = NULL WHERE EmailAddress = @Email";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Password", txtNewPassword.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Password has been reset successfully!");
                }
            }

            // Redirect to login
            REGISTER login = new REGISTER();
            login.Show();
            this.Close();
        }


        private void SendEmailOTP(string userEmail, string otp)
        {
            try
            {
                // 🔹 Read sender credentials from App.config
                string senderEmail = ConfigurationManager.AppSettings["AppEmail"];
                string senderPassword = ConfigurationManager.AppSettings["AppPassword"];

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(userEmail);
                mail.Subject = "Library System Password Reset OTP";
                mail.Body = $"Hello,\n\nYour password reset code is: {otp}\nThis code will expire in 5 minutes.\n\n- Library System";

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending email: {ex.Message}", "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnResendOTP_Click(object sender, EventArgs e)
        {
            btnSendOTP_Click(sender, e); // simply reuse the same logic
        }
    }
}
