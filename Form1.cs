using Library_Final;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;

namespace LibraryCGC
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
            UpdateTotalBooksLabel();
            UpdateTotalArchivedLabel();
            LoadPenaltyCards();
            UpdateTotalOverdueLabel();

        }







        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPenaltyCards();
            UpdateTotalBorrowedLabel();
            UpdateTotalBooksLabel();
            UpdateTotalArchivedLabel();
            UpdateTotalOverdueLabel();

            // ✅ Subscribe to live overdue update
            GlobalEvents.OverdueDataChanged += () => UpdateTotalOverdueLabel();
            GlobalEvents.BooksDataChanged += () => UpdateTotalBooksLabel();
            GlobalEvents.BorrowedDataChanged += () => UpdateTotalBorrowedLabel();
            GlobalEvents.ArchivedDataChanged += () => UpdateTotalArchivedLabel();
            GlobalEvents.PenaltiesDataChanged += () => LoadPenaltyCards(); // 👈 new






        }

        private void arthanButton1_Click(object sender, EventArgs e)
        {
            Book_Aquire bookAcq = new Book_Aquire();
            bookAcq.Show();
            this.Hide(); // ✅ Keeps app running



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
            a.Show();
            this.Hide(); // ✅ Keeps app running


        }

        private void arthanButton4_Click(object sender, EventArgs e)
        {
            Issue i = new Issue();
            i.Show();
            this.Hide(); // ✅ Keeps app running


        }

        private void arthanPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void arthanButton3_Load(object sender, EventArgs e)
        {

        }

        private void arthanButton3_Click(object sender, EventArgs e)
        {

        }

        private void arthanPanel27_Paint(object sender, PaintEventArgs e)
        {

        }

        private void arthanPanel27_Click(object sender, EventArgs e)
        {
            CreateAcc ca = new CreateAcc();
            ca.Show();
            this.Hide(); // ✅ Keeps app running
        }

        private void arthanButton3_Click_1(object sender, EventArgs e)
        {
            Return r = new Return();
            r.Show();
            this.Hide(); // ✅ Keeps app running
        }


        public void UpdateTotalBooksLabel()
        {

            SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;

");
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

            SqlConnection con = new SqlConnection(@" Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;

");
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

            SqlConnection con = new SqlConnection(@"  Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;

");
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





        public void LoadPenaltyCards()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(LoadPenaltyCards));
                return;
            }

            // Reduce flicker during reload
            flowPanel1.SuspendLayout();
            flowPanel2.SuspendLayout();
            flowPanel3.SuspendLayout();
            flowPanel4.SuspendLayout();
            flowLayoutPanel11111.SuspendLayout();
            flowLayoutPanel22222.SuspendLayout();

            // Clear existing cards
            flowPanel1.Controls.Clear();
            flowPanel2.Controls.Clear();
            flowPanel3.Controls.Clear();
            flowPanel4.Controls.Clear();
            flowLayoutPanel11111.Controls.Clear();
            flowLayoutPanel22222.Controls.Clear();

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

            string query = @"
        SELECT 
            sa.ClientID,
            ib.StudentName,
            sa.Role,
            sa.SectionSY,
            sa.Email,
            SUM(ib.OverdueDays) AS TotalOverdueDays,
            SUM(ib.Penalty) AS TotalPenalty
        FROM IssueBooks ib
        INNER JOIN AddStudentAcc sa ON ib.ClientID = sa.ClientID
        WHERE ib.OverdueDays > 0 AND ib.Status <> 'Returned'
        GROUP BY sa.ClientID, ib.StudentName, sa.Role, sa.SectionSY, sa.Email
        ORDER BY ib.StudentName;
    ";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string studentName = reader["StudentName"].ToString();
                        string role = reader["Role"].ToString();
                        string section = reader["SectionSY"].ToString();
                        string email = reader["Email"].ToString();
                        int overdueDays = Convert.ToInt32(reader["TotalOverdueDays"]);
                        decimal penalty = Convert.ToDecimal(reader["TotalPenalty"]);

                        Panel card = new Panel
                        {
                            Size = new Size(230, 120),
                            BackColor = Color.White,
                            BorderStyle = BorderStyle.FixedSingle,
                            Margin = new Padding(8)
                        };

                        Label lblInfo = new Label
                        {
                            AutoSize = false,
                            Dock = DockStyle.Fill,
                            Font = new Font("Segoe UI", 9),
                            TextAlign = ContentAlignment.MiddleLeft,
                            Padding = new Padding(10),
                            Text = $"👤 {studentName}\n" +
                                   $"{role} - {section}\n" +
                                   $"{email}\n\n" +
                                   $"📅 Total Overdue: {overdueDays} days\n" +
                                   $"💰 Total Penalty: ₱{penalty:F2}"
                        };

                        card.Controls.Add(lblInfo);

                        // Distribute dynamically to the column with the fewest cards
                        FlowLayoutPanel[] columns =
                        {
                    flowPanel1, flowPanel2, flowPanel3, flowPanel4,
                    flowLayoutPanel11111, flowLayoutPanel22222
                };

                        FlowLayoutPanel target = columns.OrderBy(p => p.Controls.Count).First();
                        target.Controls.Add(card);
                    }
                }
            }

            // Resume layouts
            flowPanel1.ResumeLayout();
            flowPanel2.ResumeLayout();
            flowPanel3.ResumeLayout();
            flowPanel4.ResumeLayout();
            flowLayoutPanel11111.ResumeLayout();
            flowLayoutPanel22222.ResumeLayout();
        }







        public void UpdateTotalOverdueLabel()
        {
            string connectionString = @" Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;

";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // ✅ Only count rows where Status = 'Overdue'
                    // and OverdueDays > 0 or Penalty > 0
                    string query = @"
    SELECT COUNT(*) 
    FROM IssueBooks
    WHERE 
        Status = 'Overdue'

";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        int totalOverdue = (int)cmd.ExecuteScalar();

                        // ✅ If no overdue books, display 0
                        lblOverdueCount.Text = totalOverdue > 0
                            ? $"Overdue Books: {totalOverdue}"
                            : "Overdue Books: 0";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error counting overdue books: " + ex.Message);
                }
            }
        }





        private void arthanPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void arthanButton8_Load(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanelPenalties_Paint(object sender, PaintEventArgs e)
        {

        }


        private void arthanPanel16_Click(object sender, EventArgs e)
        {
            ReturnedHistory returnedHistory = new ReturnedHistory();
            returnedHistory.Show();
            this.Hide();
        }

        private void arthanPanel14_Click(object sender, EventArgs e)
        {

        }



        private void arthanPanel14_Click_1(object sender, EventArgs e)
        {
            Report report = new Report();
            report.Show();
            this.Hide();
            //Return @return = new Return();
            //@return.Show();
            //this.Hide();
        }

        private void arthanPanel15_Click(object sender, EventArgs e)
        {
            Issue i = new Issue();
            i.Show();
            this.Hide();
        }

        private void arthanButton2_Click_1(object sender, EventArgs e)
        {
            Archive a = new Archive();
            a.Show();
            this.Hide(); // ✅ Keeps app running
        }

        private void arthanButton1_Click_1(object sender, EventArgs e)
        {
            Book_Aquire bookAcq = new Book_Aquire();
            bookAcq.Show();
            this.Hide(); // ✅ Keeps app running
        }

        private void arthanPanel13_Click(object sender, EventArgs e)
        {
            Archive a = new Archive();
            a.Show();
            this.Hide(); // ✅ Keeps app running
        }

        private void arthanPanel12_Click(object sender, EventArgs e)
        {
            Book_Aquire bookAcq = new Book_Aquire();
            bookAcq.Show();
            this.Hide(); // ✅ Keeps app running
        }

        private void lblOverdueCount_Click(object sender, EventArgs e)
        {

        }

        private void arthanPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void arthanButton1_Load_1(object sender, EventArgs e)
        {

        }

        private void arthanButton3_Load_1(object sender, EventArgs e)
        {

        }

        private void arthanPanel14_Paint(object sender, PaintEventArgs e)
        {

        }

        private void arthanButton6_Click(object sender, EventArgs e)
        {
            ActivityLog activityLog = new ActivityLog();
            activityLog.Show();
            this.Hide();
        }

        private void arthanButton7_Click(object sender, EventArgs e)
        {
            sign_in si = new sign_in();
            si.Show();
            this.Hide();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            sign_in si = new sign_in();
            si.Show();
            this.Hide();
        }
    }
}
