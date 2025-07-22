using System;

namespace Library_Final.Models
{
    public class BorrowRecord
    {
        public int BorrowId { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public BorrowStatus Status { get; set; }
        public string Notes { get; set; }
        public decimal? FineAmount { get; set; }
        public DateTime CreatedDate { get; set; }

        // Additional properties for display
        public string BookTitle { get; set; }
        public string UserName { get; set; }

        public bool IsOverdue => Status == BorrowStatus.Borrowed && DateTime.Now > DueDate;
        public int DaysOverdue => IsOverdue ? (DateTime.Now - DueDate).Days : 0;

        public BorrowRecord()
        {
            BorrowDate = DateTime.Now;
            Status = BorrowStatus.Borrowed;
            CreatedDate = DateTime.Now;
            Notes = "";
            BookTitle = "";
            UserName = "";
        }
    }

    public enum BorrowStatus
    {
        Borrowed = 1,
        Returned = 2,
        Lost = 3,
        Damaged = 4
    }
}
