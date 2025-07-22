using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_Final.Models
{
    public class LogEntry
    {
        [Key]
        public int LogId { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        // Navigation properties
        public virtual User? User { get; set; }
    }

    public static class LogActions
    {
        public const string Login = "LOGIN";
        public const string Logout = "LOGOUT";
        public const string BookAdded = "BOOK_ADDED";
        public const string BookUpdated = "BOOK_UPDATED";
        public const string BookDeleted = "BOOK_DELETED";
        public const string BookBorrowed = "BOOK_BORROWED";
        public const string BookReturned = "BOOK_RETURNED";
        public const string UserCreated = "USER_CREATED";
        public const string UserUpdated = "USER_UPDATED";
        public const string UserDeleted = "USER_DELETED";
    }
}
