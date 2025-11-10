
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Library_Final
{
    /// <summary>
    /// SMS Notification Service using Semaphore API (Philippine SMS provider)
    /// Sign up at: https://semaphore.co/
    /// </summary>
    public static class SmsNotificationService
    {
        private static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;";

        // Add to App.config: <add key="SemaphoreApiKey" value="YOUR_API_KEY_HERE"/>
        private static string apiKey = ConfigurationManager.AppSettings["SemaphoreApiKey"];
        private static string senderName = "CGC Library"; // Max 11 characters

        /// <summary>
        /// Initialize SMS tracking table
        /// </summary>
        public static void InitializeSmsTracking()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string createTable = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SmsNotificationLog' AND xtype='U')
                        CREATE TABLE SmsNotificationLog (
                            LogID INT IDENTITY(1,1) PRIMARY KEY,
                            ClientID INT NOT NULL,
                            PhoneNumber NVARCHAR(20),
                            RecipientName NVARCHAR(255),
                            NotificationType NVARCHAR(100),
                            Message NVARCHAR(500),
                            SentDate DATETIME DEFAULT GETDATE(),
                            Status NVARCHAR(50),
                            ErrorMessage NVARCHAR(MAX) NULL,
                            ApiResponse NVARCHAR(MAX) NULL
                        )";

                    using (SqlCommand cmd = new SqlCommand(createTable, con))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Add phone number column to AddStudentAcc if not exists
                    string addPhoneColumn = @"
                        IF NOT EXISTS (SELECT * FROM sys.columns 
                                      WHERE object_id = OBJECT_ID('AddStudentAcc') 
                                      AND name = 'PhoneNumber')
                        BEGIN
                            ALTER TABLE AddStudentAcc ADD PhoneNumber NVARCHAR(20) NULL
                        END";

                    using (SqlCommand cmd = new SqlCommand(addPhoneColumn, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing SMS tracking: {ex.Message}");
            }
        }

        /// <summary>
        /// Send SMS via Semaphore API
        /// </summary>
        public static async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("⚠️ Semaphore API Key not configured. Add it to App.config");
                return false;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                    {
                        { "apikey", apiKey },
                        { "number", FormatPhoneNumber(phoneNumber) },
                        { "message", message },
                        { "sendername", senderName }
                    };

                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync("https://api.semaphore.co/api/v4/messages", content);

                    string responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"✅ SMS sent to {phoneNumber}");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"❌ SMS failed: {responseString}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SMS error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Format phone number for Philippine format
        /// </summary>
        private static string FormatPhoneNumber(string phone)
        {
            // Remove spaces, dashes, parentheses
            phone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            // Convert to international format
            if (phone.StartsWith("09"))
                phone = "+63" + phone.Substring(1);
            else if (phone.StartsWith("9"))
                phone = "+63" + phone;
            else if (!phone.StartsWith("+63"))
                phone = "+63" + phone;

            return phone;
        }

        /// <summary>
        /// Log SMS notification
        /// </summary>
        private static void LogSmsNotification(
            int clientID,
            string phoneNumber,
            string name,
            string notificationType,
            string message,
            string status,
            string errorMessage = null,
            string apiResponse = null)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        INSERT INTO SmsNotificationLog 
                            (ClientID, PhoneNumber, RecipientName, NotificationType, Message, Status, ErrorMessage, ApiResponse)
                        VALUES 
                            (@ClientID, @Phone, @Name, @Type, @Message, @Status, @Error, @Response)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", clientID);
                        cmd.Parameters.AddWithValue("@Phone", phoneNumber ?? "");
                        cmd.Parameters.AddWithValue("@Name", name ?? "");
                        cmd.Parameters.AddWithValue("@Type", notificationType ?? "");
                        cmd.Parameters.AddWithValue("@Message", message ?? "");
                        cmd.Parameters.AddWithValue("@Status", status ?? "");
                        cmd.Parameters.AddWithValue("@Error", (object)errorMessage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Response", (object)apiResponse ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging SMS: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if SMS was already sent today
        /// </summary>
        private static bool WasSmsSentToday(int clientID, string notificationType)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = @"
                        SELECT COUNT(*) 
                        FROM SmsNotificationLog
                        WHERE ClientID = @ClientID
                          AND NotificationType = @Type
                          AND CAST(SentDate AS DATE) = CAST(GETDATE() AS DATE)
                          AND Status = 'Success'";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ClientID", clientID);
                        cmd.Parameters.AddWithValue("@Type", notificationType);

                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Send SMS for overdue books
        /// </summary>
        public static async Task<int> SendOverdueSmsNotificationsAsync()
        {
            int successCount = 0;

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
                            i.Penalty,
                            a.PhoneNumber
                        FROM IssueBooks i
                        INNER JOIN AddStudentAcc a ON i.ClientID = a.ClientID
                        WHERE 
                            i.Status = 'Overdue'
                            AND i.OverdueDays = 1
                            AND a.PhoneNumber IS NOT NULL
                            AND a.PhoneNumber != ''";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int clientID = Convert.ToInt32(reader["ClientID"]);
                            string phone = reader["PhoneNumber"].ToString();
                            string name = reader["StudentName"].ToString();
                            string bookTitle = reader["BookTitle"].ToString();
                            decimal penalty = Convert.ToDecimal(reader["Penalty"]);

                            // Check if already sent today
                            if (WasSmsSentToday(clientID, "Overdue SMS"))
                                continue;

                            // SMS must be max 160 characters for single message
                            string message = $"CGC Library Alert: '{bookTitle}' is OVERDUE. Penalty: P{penalty:N2}. Return ASAP to avoid more charges.";

                            bool sent = await SendSmsAsync(phone, message);

                            LogSmsNotification(
                                clientID,
                                phone,
                                name,
                                "Overdue SMS",
                                message,
                                sent ? "Success" : "Failed"
                            );

                            if (sent) successCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending overdue SMS: {ex.Message}");
            }

            return successCount;
        }

        /// <summary>
        /// Send all SMS notifications
        /// </summary>
        public static async Task CheckAndSendAllSmsAsync()
        {
            Console.WriteLine($"[{DateTime.Now}] Running SMS notification checks...");

            InitializeSmsTracking();

            int overdueCount = await SendOverdueSmsNotificationsAsync();

            Console.WriteLine($"[{DateTime.Now}] Completed: {overdueCount} overdue SMS sent");
        }
    }
}
