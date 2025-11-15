using Library_Final;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.IO;


namespace LibraryCGC
{
    public partial class Form1 : Form
    {

        private int currentEmployeeID;

        // ✅ UPDATE your constructor
        // ✅ Single constructor with optional parameter
        public Form1(int? employeeId = null)
        {
            InitializeComponent();

            // ✅ Use provided ID, or fall back to SessionData
            if (employeeId.HasValue)
            {
                currentEmployeeID = employeeId.Value;
                SessionData.CurrentEmployeeID = employeeId.Value;
            }
            else if (SessionData.IsLoggedIn)
            {
                currentEmployeeID = SessionData.CurrentEmployeeID;
            }
            else
            {
                // If no ID provided and no session, use default
                currentEmployeeID = 0;
            }

            // Only initialize if we have a valid employee ID
            if (currentEmployeeID > 0)
            {
                UpdateTotalBooksLabel();
                UpdateTotalBorrowedLabel();
                UpdateTotalArchivedLabel();
                LoadPenaltyCards();
                UpdateTotalOverdueLabel();
                CleanOldOTPRecords();
            }
        }



        // ✅ ADD THIS: Override OnVisibleChanged to refresh profile
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible && SessionData.IsLoggedIn)
            {
                // Refresh profile when form becomes visible
                RefreshEmployeeProfile();
            }
        }


        // ✅ NEW METHOD: Force refresh from database and update cache
        private void RefreshEmployeeProfile()
        {
            if (!SessionData.IsLoggedIn) return;

            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;";

            string query = "SELECT FirstName, LastName, ProfileImage FROM Employees WHERE EmployeeID = @EmployeeID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@EmployeeID", SessionData.CurrentEmployeeID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string firstName = reader["FirstName"].ToString();
                            string lastName = reader["LastName"].ToString();
                            string fullName = $"{firstName} {lastName}";

                            // Update UI
                            labelEmployeeName.Text = fullName;

                            // Load and cache profile image
                            Image profileImage = null;
                            if (reader["ProfileImage"] != DBNull.Value)
                            {
                                byte[] imageData = (byte[])reader["ProfileImage"];
                                using (MemoryStream ms = new MemoryStream(imageData))
                                {
                                    profileImage = Image.FromStream(ms);
                                    circlePictureBox.Image = (Image)profileImage.Clone();
                                }
                            }
                            else
                            {
                                circlePictureBox.Image = null;
                            }

                            // ✅ UPDATE SESSION CACHE
                            SessionData.InitializeSessionComplete(
                                SessionData.CurrentEmployeeID,
                                SessionData.CurrentUserName,
                                firstName,
                                lastName,
                                profileImage
                            );
                        }
                        else
                        {
                            labelEmployeeName.Text = "Employee Not Found";
                            circlePictureBox.Image = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading employee profile: " + ex.Message);
                }
            }
        }


        // ✅ REPLACE your existing LoadEmployeeProfile with this optimized version
        // ✅ UPDATED LoadEmployeeProfile with caching
        private void LoadEmployeeProfile()
        {
            // Try to use cached session data first
            if (SessionData.IsSessionDataComplete())
            {
                // Use cached data - instant load!
                labelEmployeeName.Text = SessionData.CurrentUserFullName;

                if (SessionData.CurrentUserProfileImage != null)
                {
                    circlePictureBox.Image = (Image)SessionData.CurrentUserProfileImage.Clone();
                }

                return; // Exit early, no database call needed
            }

            // If no cache, load from database
            RefreshEmployeeProfile();
        }



        private void label2_Click(object sender, EventArgs e)
        {

        }

        // ✅ UPDATED Form1_Load
        private async void Form1_Load(object sender, EventArgs e)
        {
            timer2.Start();


            if (currentEmployeeID > 0)
            {
                await Task.Delay(100);

                // Load profile (will use cache if available)
                LoadEmployeeProfile();

                UpdateTotalBooksLabel();
                UpdateTotalBorrowedLabel();
                UpdateTotalArchivedLabel();
                LoadPenaltyCards();
                UpdateTotalOverdueLabel();
                CleanOldOTPRecords();

                // ✅ Subscribe to live overdue update
                GlobalEvents.OverdueDataChanged += () => UpdateTotalOverdueLabel();
                GlobalEvents.BooksDataChanged += () => UpdateTotalBooksLabel();
                GlobalEvents.BorrowedDataChanged += () => UpdateTotalBorrowedLabel();
                GlobalEvents.ArchivedDataChanged += () => UpdateTotalArchivedLabel();
                GlobalEvents.PenaltiesDataChanged += () => LoadPenaltyCards();

                // ✅ START A TIMER TO KEEP PENALTIES UPDATED
                System.Windows.Forms.Timer penaltyUpdateTimer = new System.Windows.Forms.Timer();
                penaltyUpdateTimer.Interval = 60000; // Update every minute
                penaltyUpdateTimer.Tick += (s, args) => LoadPenaltyCards();
                penaltyUpdateTimer.Start();
            }
        }

        private void CleanOldOTPRecords()
        {
            string connectionString = @" Data Source=(LocalDB)\MSSQLLocalDB;
Initial Catalog=LibraryDB;
Integrated Security=True;
Encrypt=True;
Trust Server Certificate=True;

";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string deleteQuery = "DELETE FROM OTPRequests WHERE SentTime < DATEADD(DAY, -7, GETDATE())";
                    SqlCommand cmd = new SqlCommand(deleteQuery, conn);
                    int rows = cmd.ExecuteNonQuery();

                    // Optional: You can remove this line if you want it silent
                    Console.WriteLine($"{rows} old OTP records deleted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error cleaning OTP logs: " + ex.Message);
            }
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




        // ✅ Replace your existing UpdateTotalBooksLabel method with this
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

                // ✅ Sum the Quantity column instead of counting rows
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(Quantity), 0) FROM BooksAcq", con);
                int totalBooks = Convert.ToInt32(cmd.ExecuteScalar());

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

        // ✅ Replace your existing UpdateTotalArchivedLabel method with this
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

                // ✅ Sum the Quantity column instead of counting rows
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(SUM(Quantity), 0) FROM BooksArchive", con);
                int totalArchived = Convert.ToInt32(cmd.ExecuteScalar());

                labelTotalArchived.Text = totalArchived.ToString();
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







        // ✅ FIXED LoadPenaltyCards method for Form1.cs
        // Replace your existing LoadPenaltyCards() method with this:

        // ✅ FIXED LoadPenaltyCards method for Form1.cs
        // This version handles missing AddStudentAcc records gracefully

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

            // ✅ FIXED QUERY - Use LEFT JOIN and get data from IssueBooks directly
            string query = @"
        SELECT 
            ib.ClientID,
            ib.StudentName,
            ISNULL(sa.Role, 'Student') AS Role,
            ISNULL(sa.SectionSY, 'N/A') AS SectionSY,
            ISNULL(sa.Email, 'Not Available') AS Email,
            SUM(ib.OverdueDays) AS TotalOverdueDays,
            SUM(ib.Penalty) AS TotalPenalty,
            COUNT(*) AS BookCount
        FROM IssueBooks ib
        LEFT JOIN AddStudentAcc sa ON ib.ClientID = sa.ClientID
        WHERE ib.Status = 'Overdue'
          AND ib.OverdueDays > 0
          AND ib.Penalty > 0
        GROUP BY ib.ClientID, ib.StudentName, sa.Role, sa.SectionSY, sa.Email
        ORDER BY ib.StudentName;
    ";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int recordCount = 0;

                        while (reader.Read())
                        {
                            recordCount++;

                            string studentName = reader["StudentName"]?.ToString() ?? "Unknown";
                            string role = reader["Role"]?.ToString() ?? "Student";
                            string section = reader["SectionSY"]?.ToString() ?? "N/A";
                            string email = reader["Email"]?.ToString() ?? "Not Available";
                            int overdueDays = Convert.ToInt32(reader["TotalOverdueDays"]);
                            decimal penalty = Convert.ToDecimal(reader["TotalPenalty"]);
                            int bookCount = Convert.ToInt32(reader["BookCount"]);

                            Panel card = new Panel
                            {
                                Size = new Size(230, 140),
                                BackColor = Color.White,
                                BorderStyle = BorderStyle.FixedSingle,
                                Margin = new Padding(8)
                            };

                            Label lblInfo = new Label
                            {
                                AutoSize = false,
                                Dock = DockStyle.Fill,
                                Font = new Font("Segoe UI", 8.5F),
                                TextAlign = ContentAlignment.MiddleLeft,
                                Padding = new Padding(8),
                                Text = $"👤 {studentName}\n" +
                                       $"{role} - {section}\n" +
                                       $"📧 {email}\n" +
                                       $"📚 Books: {bookCount}\n" +
                                       $"📅 Overdue: {overdueDays} days\n" +
                                       $"💰 Penalty: ₱{penalty:F2}"
                            };

                            card.Controls.Add(lblInfo);

                            // Distribute to columns
                            FlowLayoutPanel[] columns =
                            {
                        flowPanel1, flowPanel2, flowPanel3, flowPanel4,
                        flowLayoutPanel11111, flowLayoutPanel22222
                    };

                            FlowLayoutPanel target = columns.OrderBy(p => p.Controls.Count).First();
                            target.Controls.Add(card);
                        }

                        if (recordCount == 0)
                        {
                            Label noData = new Label
                            {
                                Text = "No overdue books at this time",
                                Font = new Font("Segoe UI", 10F, FontStyle.Italic),
                                ForeColor = Color.Gray,
                                AutoSize = true,
                                Padding = new Padding(10)
                            };
                            flowPanel1.Controls.Add(noData);
                        }
                        else
                        {
                            Console.WriteLine($"✅ Loaded {recordCount} penalty cards");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading penalty cards:\n{ex.Message}",
                               "Database Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
            }
            finally
            {
                flowPanel1.ResumeLayout();
                flowPanel2.ResumeLayout();
                flowPanel3.ResumeLayout();
                flowPanel4.ResumeLayout();
                flowLayoutPanel11111.ResumeLayout();
                flowLayoutPanel22222.ResumeLayout();
            }
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
            NotificationDashboard returnedHistory = new NotificationDashboard();
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
            Issue issueForm = new Issue(currentEmployeeID);
            issueForm.Show();
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

        private void guna2Button2_Click(object sender, EventArgs e)
        {

            // ✅ Use SessionData instead of local variable
            ManageProfileForm manageProfileForm = new ManageProfileForm(SessionData.CurrentEmployeeID);
            manageProfileForm.Show();
            this.Hide();
        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
          "Are you sure you want to log out?",
          "Confirm Logout",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Warning
      );

            if (result == DialogResult.Yes)
            {
                REGISTER rEGISTER = new REGISTER();
                rEGISTER.Show();
                this.Hide();
            }
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            ActivityLog activityLog = new ActivityLog();
            activityLog.Show();
            this.Hide();
        }

        private void arthanPanel16_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            sign_in sign_In = new sign_in();
            sign_In.Show();
            this.Hide();
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            NotificationDashboard notificationDashboard = new NotificationDashboard();
            notificationDashboard.Show();
            this.Hide();
        }

        private void arthanButton5_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label3.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm:ss tt");
        }
    }
}
