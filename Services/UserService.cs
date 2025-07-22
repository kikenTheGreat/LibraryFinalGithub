using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Library_Final.Models;
using Library_Final.Data;

namespace Library_Final.Services
{
    public class UserService
    {
        public User AuthenticateUser(string username, string password)
        {
            string query = "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1";
            SqlParameter[] parameters = {
                new SqlParameter("@Username", username)
            };

            DataTable result = DatabaseHelper.ExecuteQuery(query, parameters);
            
            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                string storedHash = row["PasswordHash"].ToString();
                
                if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                {
                    // Update last login date
                    UpdateLastLogin(Convert.ToInt32(row["UserId"]));
                    
                    return new User
                    {
                        UserId = Convert.ToInt32(row["UserId"]),
                        Username = row["Username"].ToString(),
                        Email = row["Email"].ToString(),
                        FullName = row["FullName"].ToString(),
                        Role = (UserRole)Convert.ToInt32(row["Role"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        LastLoginDate = row["LastLoginDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["LastLoginDate"]),
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    };
                }
            }
            
            return null;
        }

        public bool CreateUser(User user, string password)
        {
            try
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                
                string query = @"
                    INSERT INTO Users (Username, PasswordHash, Email, FullName, Role, CreatedDate, IsActive)
                    VALUES (@Username, @PasswordHash, @Email, @FullName, @Role, @CreatedDate, @IsActive)";

                SqlParameter[] parameters = {
                    new SqlParameter("@Username", user.Username),
                    new SqlParameter("@PasswordHash", hashedPassword),
                    new SqlParameter("@Email", user.Email),
                    new SqlParameter("@FullName", user.FullName),
                    new SqlParameter("@Role", (int)user.Role),
                    new SqlParameter("@CreatedDate", user.CreatedDate),
                    new SqlParameter("@IsActive", user.IsActive)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            string query = "SELECT * FROM Users WHERE IsActive = 1 ORDER BY FullName";
            
            DataTable result = DatabaseHelper.ExecuteQuery(query);
            
            foreach (DataRow row in result.Rows)
            {
                users.Add(new User
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    Username = row["Username"].ToString(),
                    Email = row["Email"].ToString(),
                    FullName = row["FullName"].ToString(),
                    Role = (UserRole)Convert.ToInt32(row["Role"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    LastLoginDate = row["LastLoginDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["LastLoginDate"]),
                    IsActive = Convert.ToBoolean(row["IsActive"])
                });
            }
            
            return users;
        }

        public bool IsUsernameAvailable(string username)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            SqlParameter[] parameters = {
                new SqlParameter("@Username", username)
            };
            
            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            return count == 0;
        }

        public bool IsEmailAvailable(string email)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            SqlParameter[] parameters = {
                new SqlParameter("@Email", email)
            };
            
            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            return count == 0;
        }

        private void UpdateLastLogin(int userId)
        {
            string query = "UPDATE Users SET LastLoginDate = @LoginDate WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@LoginDate", DateTime.Now),
                new SqlParameter("@UserId", userId)
            };
            
            DatabaseHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
