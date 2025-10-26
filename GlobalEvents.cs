using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Final
{
    public static class GlobalEvents
    {
        private static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=LibraryDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";


        //Activity Log
        public static void LogActivity(string userName, string action, string module, string details = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO ActivityLog (UserName, Action, Module, Details) VALUES (@UserName, @Action, @Module, @Details)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@Module", module);
                        cmd.Parameters.AddWithValue("@Details", details);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Optionally log to file if DB fails
                Console.WriteLine("Error logging activity: " + ex.Message);
            }
        }




        // 🔔 Overdue
        public static event Action? OverdueDataChanged;
        public static void RaiseOverdueDataChanged() => OverdueDataChanged?.Invoke();

        // 📚 BooksAcq (total books)
        public static event Action? BooksDataChanged;
        public static void RaiseBooksDataChanged() => BooksDataChanged?.Invoke();

        // 📖 IssueBooks (borrowed)
        public static event Action? BorrowedDataChanged;
        public static void RaiseBorrowedDataChanged() => BorrowedDataChanged?.Invoke();

        // 🗃️ BooksArchive (archived)
        public static event Action? ArchivedDataChanged;
        public static void RaiseArchivedDataChanged() => ArchivedDataChanged?.Invoke();


        public static event Action? PenaltiesDataChanged;
        public static void RaisePenaltiesDataChanged() => PenaltiesDataChanged?.Invoke();




     

    }
}
