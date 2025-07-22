
using Library_Final.Interfaces;
using Library_Final.Utilities;

namespace Library_Final
{
    public partial class Login : Form
    {
        private readonly IUserService _userService;

        public Login()
        {
            InitializeComponent();
            _userService = ServiceProvider.GetService<IUserService>();
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void kryptonLinkLabel2_LinkClicked(object sender, EventArgs e)
        {
            this.Hide();
            SignUpForLibrarian signUp = new SignUpForLibrarian();
            signUp.ShowDialog();
            this.Close();
        }

        private async void kryptonButton1_Click(object sender, EventArgs e)
        {
            await LoginUserAsync();
        }

        private async Task LoginUserAsync()
        {
            try
            {
                // Get username and password from form controls
                // Note: You'll need to add proper control names in the designer
                string username = GetUsernameFromForm();
                string password = GetPasswordFromForm();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("Please enter both username and password.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Authenticate user
                var user = await _userService.AuthenticateAsync(username, password);
                
                if (user != null)
                {
                    // Set session
                    SessionManager.Instance.Login(user);
                    
                    MessageBox.Show($"Welcome, {user.FullName}!", "Login Successful", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Navigate to dashboard
                    this.Hide();
                    Dashboard dashboard = new Dashboard();
                    dashboard.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Login Failed", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetUsernameFromForm()
        {
            // You'll need to replace this with the actual username textbox control name
            // Example: return txtUsername.Text;
            // For now, returning empty string - you'll need to update this
            return string.Empty;
        }

        private string GetPasswordFromForm()
        {
            // You'll need to replace this with the actual password textbox control name
            // Example: return txtPassword.Text;
            // For now, returning empty string - you'll need to update this
            return string.Empty;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            // Set focus to username field
            // Example: txtUsername.Focus();
        }
    }
}
