using System;
using System.Configuration;
using System.Net.Mail;
using Microsoft.Data.SqlClient;
using System.Text;

namespace LibraryCGC
{
    public static class EmailNotificationService
    {
        private static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;";

        /// <summary>
        /// Initialize email tracking table
        /// </summary>
        public static void InitializeEmailTracking()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string createTable = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmailNotificationLog' AND xtype='U')
                        CREATE TABLE EmailNotificationLog (
                            LogID INT IDENTITY(1,1) PRIMARY KEY,
                            ClientID INT NOT NULL,
                            RecipientEmail NVARCHAR(255),
                            RecipientName NVARCHAR(255),
                            NotificationType NVARCHAR(100),
                            Subject NVARCHAR(500),
                            MessageBody NVARCHAR(MAX),
                            SentDate DATETIME DEFAULT GETDATE(),
                            Status NVARCHAR(50),
                            ErrorMessage NVARCHAR(MAX) NULL,
                            ISBN NVARCHAR(50) NULL,
                            BookTitle NVARCHAR(500) NULL
                        )";

                    using (SqlCommand cmd = new SqlCommand(createTable, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing email tracking: {ex.Message}");
            }
        }

        /// <summary>
        /// Log email notification to database
        /// </summary>
        private static void LogEmailNotification(
            int clientID,
            string email,
            string name,
            string notificationType,
            string subject,
            string body,
            string status,
            string errorMessage = null,
            string isbn = null,
            string bookTitle = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        INSERT INTO EmailNotificationLog 
                            (ClientID, RecipientEmail, RecipientName, NotificationType, Subject, MessageBody, Status, ErrorMessage, ISBN, BookTitle)
                        VALUES 
                            (@ClientID, @Email, @Name, @Type, @Subject, @Body, @Status, @Error, @ISBN, @BookTitle)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", clientID);
                        cmd.Parameters.AddWithValue("@Email", email ?? "");
                        cmd.Parameters.AddWithValue("@Name", name ?? "");
                        cmd.Parameters.AddWithValue("@Type", notificationType ?? "");
                        cmd.Parameters.AddWithValue("@Subject", subject ?? "");
                        cmd.Parameters.AddWithValue("@Body", body ?? "");
                        cmd.Parameters.AddWithValue("@Status", status ?? "");
                        cmd.Parameters.AddWithValue("@Error", (object)errorMessage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ISBN", (object)isbn ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BookTitle", (object)bookTitle ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging email: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if notification was already sent today
        /// </summary>
        private static bool WasNotificationSentToday(int clientID, string notificationType, string isbn = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT COUNT(*) 
                        FROM EmailNotificationLog
                        WHERE ClientID = @ClientID
                          AND NotificationType = @Type
                          AND CAST(SentDate AS DATE) = CAST(GETDATE() AS DATE)
                          AND Status = 'Success'
                          AND (@ISBN IS NULL OR ISBN = @ISBN)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", clientID);
                        cmd.Parameters.AddWithValue("@Type", notificationType);
                        cmd.Parameters.AddWithValue("@ISBN", (object)isbn ?? DBNull.Value);

                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false; // If error, allow sending
            }
        }

        /// <summary>
        /// Send HTML formatted email
        /// </summary>
        public static bool SendHtmlEmail(string recipientEmail, string subject, string htmlBody)
        {
            try
            {
                string senderEmail = ConfigurationManager.AppSettings["AppEmail"];
                string senderPassword = ConfigurationManager.AppSettings["AppPassword"];

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(senderEmail, "CGC Library System");
                mail.To.Add(recipientEmail);
                mail.Subject = subject;
                mail.Body = htmlBody;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Generate HTML email template
        /// </summary>
        private static string GenerateHtmlEmail(string studentName, string content, string footerNote = "")
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 20px auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: linear-gradient(135deg, #FDB913 0%, #F9A825 100%); padding: 30px; text-align: center; }}
        .header h1 {{ color: white; margin: 0; font-size: 24px; }}
        .content {{ padding: 30px; color: #333; line-height: 1.6; }}
        .book-info {{ background: #FFF9E6; border-left: 4px solid #FDB913; padding: 15px; margin: 20px 0; border-radius: 4px; }}
        .book-info p {{ margin: 8px 0; }}
        .warning {{ background: #FFE5E5; border-left: 4px solid #FF5252; padding: 15px; margin: 20px 0; border-radius: 4px; }}
        .button {{ display: inline-block; padding: 12px 30px; background: #FDB913; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; font-weight: bold; }}
        .footer {{ background: #f9f9f9; padding: 20px; text-align: center; color: #666; font-size: 12px; border-top: 1px solid #eee; }}
        .icon {{ font-size: 18px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📚 CGC Library System</h1>
        </div>
        <div class='content'>
            <p>Dear <strong>{studentName}</strong>,</p>
            {content}
            {(string.IsNullOrEmpty(footerNote) ? "" : $"<div class='warning'><strong>⚠️ Important:</strong> {footerNote}</div>")}
            <p>If you have any questions, please visit the library desk during operating hours.</p>
            <p>Best regards,<br><strong>CGC Library Team</strong></p>
        </div>
        <div class='footer'>
            <p>This is an automated message from CGC Library System.</p>
            <p>Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Send due date reminders with tracking
        /// </summary>
        public static int SendDueDateReminders()
        {
            int successCount = 0;
            int skippedCount = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            i.ClientID,
                            i.StudentName,
                            i.BookTitle,
                            i.ISBN,
                            i.DueDate,
                            a.Email
                        FROM IssueBooks i
                        INNER JOIN AddStudentAcc a ON i.ClientID = a.ClientID
                        WHERE 
                            i.Status = 'Issued'
                            AND CAST(i.DueDate AS DATE) = CAST(DATEADD(DAY, 1, GETDATE()) AS DATE)
                            AND a.Email IS NOT NULL
                            AND a.Email != ''";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int clientID = Convert.ToInt32(reader["ClientID"]);
                            string email = reader["Email"].ToString();
                            string studentName = reader["StudentName"].ToString();
                            string bookTitle = reader["BookTitle"].ToString();
                            string isbn = reader["ISBN"].ToString();
                            DateTime dueDate = Convert.ToDateTime(reader["DueDate"]);

                            // Check if already sent today
                            if (WasNotificationSentToday(clientID, "Due Date Reminder", isbn))
                            {
                                skippedCount++;
                                continue;
                            }

                            string subject = "📚 Library Reminder: Book Due Tomorrow";

                            string content = $@"
                                <p>This is a friendly reminder that the following book is due <strong>TOMORROW</strong>:</p>
                                <div class='book-info'>
                                    <p><span class='icon'>📖</span> <strong>Book Title:</strong> {bookTitle}</p>
                                    <p><span class='icon'>🔢</span> <strong>ISBN:</strong> {isbn}</p>
                                    <p><span class='icon'>📅</span> <strong>Due Date:</strong> {dueDate:MMMM dd, yyyy}</p>
                                </div>";

                            string htmlBody = GenerateHtmlEmail(
                                studentName,
                                content,
                                "Please return the book on time to avoid penalty charges of ₱5 per day."
                            );

                            bool sent = SendHtmlEmail(email, subject, htmlBody);

                            // Log the notification
                            LogEmailNotification(
                                clientID,
                                email,
                                studentName,
                                "Due Date Reminder",
                                subject,
                                htmlBody,
                                sent ? "Success" : "Failed",
                                sent ? null : "Failed to send email",
                                isbn,
                                bookTitle
                            );

                            if (sent)
                            {
                                successCount++;
                                Console.WriteLine($"✅ Due date reminder sent to {studentName} ({email})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending due date reminders: {ex.Message}");
            }

            Console.WriteLine($"Due Date Reminders: {successCount} sent, {skippedCount} skipped (already sent today)");
            return successCount;
        }

        /// <summary>
        /// Send overdue notifications with tracking
        /// </summary>
        public static int SendOverdueNotifications()
        {
            int successCount = 0;
            int skippedCount = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            i.ClientID,
                            i.StudentName,
                            i.BookTitle,
                            i.ISBN,
                            i.DueDate,
                            i.OverdueDays,
                            i.Penalty,
                            a.Email
                        FROM IssueBooks i
                        INNER JOIN AddStudentAcc a ON i.ClientID = a.ClientID
                        WHERE 
                            i.Status = 'Overdue'
                            AND i.OverdueDays = 1
                            AND a.Email IS NOT NULL
                            AND a.Email != ''";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int clientID = Convert.ToInt32(reader["ClientID"]);
                            string email = reader["Email"].ToString();
                            string studentName = reader["StudentName"].ToString();
                            string bookTitle = reader["BookTitle"].ToString();
                            string isbn = reader["ISBN"].ToString();
                            DateTime dueDate = Convert.ToDateTime(reader["DueDate"]);
                            decimal penalty = Convert.ToDecimal(reader["Penalty"]);

                            // Check if already sent today
                            if (WasNotificationSentToday(clientID, "Overdue Notification", isbn))
                            {
                                skippedCount++;
                                continue;
                            }

                            string subject = "⚠️ Library Alert: Book Is Now OVERDUE";

                            string content = $@"
                                <p>Your borrowed book is now <strong style='color: #FF5252;'>OVERDUE</strong>:</p>
                                <div class='book-info'>
                                    <p><span class='icon'>📖</span> <strong>Book Title:</strong> {bookTitle}</p>
                                    <p><span class='icon'>🔢</span> <strong>ISBN:</strong> {isbn}</p>
                                    <p><span class='icon'>📅</span> <strong>Original Due Date:</strong> {dueDate:MMMM dd, yyyy}</p>
                                    <p><span class='icon'>💰</span> <strong>Current Penalty:</strong> <span style='color: #FF5252;'>₱{penalty:N2}</span></p>
                                </div>";

                            string htmlBody = GenerateHtmlEmail(
                                studentName,
                                content,
                                "Penalties increase by ₱5 per day until the book is returned. Please return it as soon as possible to minimize charges."
                            );

                            bool sent = SendHtmlEmail(email, subject, htmlBody);

                            // Log the notification
                            LogEmailNotification(
                                clientID,
                                email,
                                studentName,
                                "Overdue Notification",
                                subject,
                                htmlBody,
                                sent ? "Success" : "Failed",
                                sent ? null : "Failed to send email",
                                isbn,
                                bookTitle
                            );

                            if (sent)
                            {
                                successCount++;
                                Console.WriteLine($"✅ Overdue notification sent to {studentName} ({email})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending overdue notifications: {ex.Message}");
            }

            Console.WriteLine($"Overdue Notifications: {successCount} sent, {skippedCount} skipped (already sent today)");
            return successCount;
        }

        /// <summary>
        /// Send weekly penalty reminders with tracking
        /// </summary>
        public static int SendWeeklyPenaltyReminders()
        {
            int successCount = 0;
            int skippedCount = 0;

            try
            {
                // Only run on Mondays
                if (DateTime.Now.DayOfWeek != DayOfWeek.Monday)
                    return 0;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT 
                            i.ClientID,
                            i.StudentName,
                            a.Email,
                            COUNT(*) as BookCount,
                            SUM(i.Penalty) as TotalPenalty
                        FROM IssueBooks i
                        INNER JOIN AddStudentAcc a ON i.ClientID = a.ClientID
                        WHERE 
                            i.Status = 'Overdue'
                            AND i.Penalty > 0
                            AND a.Email IS NOT NULL
                            AND a.Email != ''
                        GROUP BY i.ClientID, i.StudentName, a.Email";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int clientID = Convert.ToInt32(reader["ClientID"]);
                            string email = reader["Email"].ToString();
                            string studentName = reader["StudentName"].ToString();
                            int bookCount = Convert.ToInt32(reader["BookCount"]);
                            decimal totalPenalty = Convert.ToDecimal(reader["TotalPenalty"]);

                            // Check if already sent today
                            if (WasNotificationSentToday(clientID, "Weekly Penalty Reminder"))
                            {
                                skippedCount++;
                                continue;
                            }

                            string subject = "📋 Weekly Library Penalty Reminder";

                            string content = $@"
                                <p>This is your <strong>WEEKLY</strong> reminder about outstanding library penalties:</p>
                                <div class='book-info'>
                                    <p><span class='icon'>📚</span> <strong>Overdue Books:</strong> {bookCount}</p>
                                    <p><span class='icon'>💰</span> <strong>Total Penalty:</strong> <span style='color: #FF5252;'>₱{totalPenalty:N2}</span></p>
                                </div>
                                <p>Please return these books immediately to stop penalties from increasing.</p>";

                            string htmlBody = GenerateHtmlEmail(
                                studentName,
                                content,
                                "Penalties continue to accumulate at ₱5 per book per day. You can settle your account at the library desk during operating hours."
                            );

                            bool sent = SendHtmlEmail(email, subject, htmlBody);

                            // Log the notification
                            LogEmailNotification(
                                clientID,
                                email,
                                studentName,
                                "Weekly Penalty Reminder",
                                subject,
                                htmlBody,
                                sent ? "Success" : "Failed",
                                sent ? null : "Failed to send email"
                            );

                            if (sent)
                            {
                                successCount++;
                                Console.WriteLine($"✅ Weekly reminder sent to {studentName} ({email})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending weekly reminders: {ex.Message}");
            }

            Console.WriteLine($"Weekly Reminders: {successCount} sent, {skippedCount} skipped (already sent today)");
            return successCount;
        }

        /// <summary>
        /// Main method to check and send all notifications
        /// </summary>
        public static void CheckAndSendAllNotifications()
        {
            Console.WriteLine($"[{DateTime.Now}] Running email notification checks...");

            InitializeEmailTracking();

            int dueCount = SendDueDateReminders();
            int overdueCount = SendOverdueNotifications();
            int weeklyCount = SendWeeklyPenaltyReminders();

            Console.WriteLine($"[{DateTime.Now}] Completed: {dueCount} due reminders, {overdueCount} overdue notices, {weeklyCount} weekly reminders");
        }
    }
}