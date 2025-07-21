
namespace Library_Final
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void kryptonButton4_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void kryptonLinkLabel2_LinkClicked(object sender, EventArgs e)
        {
            this.Hide();
            SignUp Reg = new SignUp();
            Reg.ShowDialog();
            this.Close();
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard dash = new Dashboard();
            dash.ShowDialog();
            this.Close();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
