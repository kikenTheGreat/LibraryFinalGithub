using Library_Final;
using Microsoft.Data.SqlClient;
using System.Drawing;

namespace LibraryCGC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateTotalBorrowedLabel();
            timer1.Interval = 5000; // update every 5 seconds
            timer1.Tick += (s, args) => UpdateTotalBorrowedLabel();
            timer1.Start();

            UpdateTotalBooksLabel();
            UpdateTotalArchivedLabel();
        }

        private void arthanButton1_Click(object sender, EventArgs e)
        {
            Book_Aquire bookAcq = new Book_Aquire();
            bookAcq.ShowDialog();
            this.Hide();



        }

        private void arthanButton1_Load(object sender, EventArgs e)
        {

        }

        private void arthanPanel12_Paint(object sender, PaintEventArgs e)
        {

        }

        private void arthanButton2_Load(object sender, EventArgs e)
        {

        }

        private void arthanButton2_Click(object sender, EventArgs e)
        {
            Archive a = new Archive();
            a.ShowDialog();


        }

        private void arthanButton4_Click(object sender, EventArgs e)
        {
            Issue i = new Issue();
            i.ShowDialog();


        }

        private void arthanPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void arthanButton3_Load(object sender, EventArgs e)
        {

        }

        private void arthanButton3_Click(object sender, EventArgs e)
        {
            CreateAcc ca = new CreateAcc();
            ca.Show();
            this.Hide();
        }

        private void arthanPanel27_Paint(object sender, PaintEventArgs e)
        {

        }

        private void arthanPanel27_Click(object sender, EventArgs e)
        {
            CreateAcc ca = new CreateAcc();
            ca.Show();
            this.Close();
        }

        private void arthanButton3_Click_1(object sender, EventArgs e)
        {
            Return r = new Return();
            r.Show();
            this.Close();
        }


        public void UpdateTotalBooksLabel()
        {

            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM BooksAcq", con);
                int totalBooks = (int)cmd.ExecuteScalar();
                labelTotalBooks.Text = totalBooks.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }




        public void UpdateTotalBorrowedLabel()
        {
          
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM IssueBooks", con);
                int totalBorrowed = (int)cmd.ExecuteScalar();
                labelTotalBorrowed.Text = totalBorrowed.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public void UpdateTotalArchivedLabel()
        {

            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM BooksArchive", con);
                int totalarchived = (int)cmd.ExecuteScalar();
                labelTotalArchived.Text = totalarchived.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }



    }
}
