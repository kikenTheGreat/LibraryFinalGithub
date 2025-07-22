using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Library_Final.Data
{
    public class DatabaseHelper
    {
        private static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\LibraryDB.mdf;Integrated Security=True";

        public static string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static void ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    
                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        public static void InitializeDatabase()
        {
            try
            {
                CreateTables();
                InsertDefaultData();
            }
            catch (Exception ex)
            {
                throw new Exception("Database initialization failed: " + ex.Message);
            }
        }

        private static void CreateTables()
        {
            string createUsersTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                CREATE TABLE Users (
                    UserId INT IDENTITY(1,1) PRIMARY KEY,
                    Username NVARCHAR(50) UNIQUE NOT NULL,
                    PasswordHash NVARCHAR(255) NOT NULL,
                    Email NVARCHAR(100) UNIQUE NOT NULL,
                    FullName NVARCHAR(100) NOT NULL,
                    Role INT NOT NULL,
                    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
                    LastLoginDate DATETIME NULL,
                    IsActive BIT NOT NULL DEFAULT 1
                )";

            string createBooksTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Books' AND xtype='U')
                CREATE TABLE Books (
                    BookId INT IDENTITY(1,1) PRIMARY KEY,
                    ISBN NVARCHAR(13) UNIQUE NOT NULL,
                    Title NVARCHAR(200) NOT NULL,
                    Author NVARCHAR(100) NOT NULL,
                    Publisher NVARCHAR(50) NULL,
                    PublishedDate DATETIME NULL,
                    Category NVARCHAR(50) NULL,
                    Description NVARCHAR(500) NULL,
                    TotalCopies INT NOT NULL DEFAULT 1,
                    AvailableCopies INT NOT NULL DEFAULT 1,
                    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
                    UpdatedDate DATETIME NULL,
                    IsActive BIT NOT NULL DEFAULT 1
                )";

            string createBorrowRecordsTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='BorrowRecords' AND xtype='U')
                CREATE TABLE BorrowRecords (
                    BorrowId INT IDENTITY(1,1) PRIMARY KEY,
                    UserId INT NOT NULL,
                    BookId INT NOT NULL,
                    BorrowDate DATETIME NOT NULL DEFAULT GETDATE(),
                    DueDate DATETIME NOT NULL,
                    ReturnDate DATETIME NULL,
                    Status INT NOT NULL DEFAULT 1,
                    Notes NVARCHAR(200) NULL,
                    FineAmount DECIMAL(10,2) NULL,
                    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
                    FOREIGN KEY (UserId) REFERENCES Users(UserId),
                    FOREIGN KEY (BookId) REFERENCES Books(BookId)
                )";

            ExecuteNonQuery(createUsersTable);
            ExecuteNonQuery(createBooksTable);
            ExecuteNonQuery(createBorrowRecordsTable);
        }

        private static void InsertDefaultData()
        {
            // Check if admin user exists
            string checkAdmin = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
            int adminCount = Convert.ToInt32(ExecuteScalar(checkAdmin));

            if (adminCount == 0)
            {
                string adminPassword = BCrypt.Net.BCrypt.HashPassword("admin123");
                string insertAdmin = @"
                    INSERT INTO Users (Username, PasswordHash, Email, FullName, Role)
                    VALUES ('admin', @Password, 'admin@library.com', 'System Administrator', 3)";

                SqlParameter[] parameters = {
                    new SqlParameter("@Password", adminPassword)
                };

                ExecuteNonQuery(insertAdmin, parameters);
            }
        }
    }
}
