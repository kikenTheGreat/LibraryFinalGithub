using System;

namespace Library_Final.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        public Book()
        {
            ISBN = "";
            Title = "";
            Author = "";
            Publisher = "";
            Category = "";
            Description = "";
            TotalCopies = 1;
            AvailableCopies = 1;
            CreatedDate = DateTime.Now;
            IsActive = true;
        }
    }
}
