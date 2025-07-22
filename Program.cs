using Library_Final.Utilities;
using Library_Final.Data;
using Microsoft.EntityFrameworkCore;

namespace Library_Final
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize application configuration
            ApplicationConfiguration.Initialize();
            
            // Initialize dependency injection
            ServiceProvider.Initialize();
            
            // Initialize database
            InitializeDatabase();
            
            // Run the application
            Application.Run(new Login());
            
            // Cleanup
            ServiceProvider.DisposeServices();
        }
        
        private static void InitializeDatabase()
        {
            try
            {
                using var context = ServiceProvider.GetService<LibraryDbContext>();
                context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}