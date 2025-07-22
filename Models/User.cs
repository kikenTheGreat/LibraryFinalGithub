using System;

namespace Library_Final.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }

        public User()
        {
            Username = "";
            PasswordHash = "";
            Email = "";
            FullName = "";
            CreatedDate = DateTime.Now;
            IsActive = true;
        }
    }

    public enum UserRole
    {
        Student = 1,
        Librarian = 2,
        Admin = 3
    }
}
