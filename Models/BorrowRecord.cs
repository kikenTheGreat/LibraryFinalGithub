using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_Final.Models
{
    public class BorrowRecord
    {
        [Key]
        public int BorrowId { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Book")]
        public int BookId { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required]
        public BorrowStatus Status { get; set; } = BorrowStatus.Borrowed;

        [MaxLength(200)]
        public string? Notes { get; set; }

        public decimal? FineAmount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Book Book { get; set; } = null!;

        // Calculated properties
        public bool IsOverdue => Status == BorrowStatus.Borrowed && DateTime.Now > DueDate;
        public int DaysOverdue => IsOverdue ? (DateTime.Now - DueDate).Days : 0;
    }

    public enum BorrowStatus
    {
        Borrowed = 1,
        Returned = 2,
        Lost = 3,
        Damaged = 4
    }
}
