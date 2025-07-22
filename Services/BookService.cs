using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Library_Final.Models;
using Library_Final.Data;

namespace Library_Final.Services
{
    public class BookService
    {
        public List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            string query = "SELECT * FROM Books WHERE IsActive = 1 ORDER BY Title";
            
            DataTable result = DatabaseHelper.ExecuteQuery(query);
            
            foreach (DataRow row in result.Rows)
            {
                books.Add(CreateBookFromDataRow(row));
            }
            
            return books;
        }

        public List<Book> GetAvailableBooks()
        {
            List<Book> books = new List<Book>();
            string query = "SELECT * FROM Books WHERE IsActive = 1 AND AvailableCopies > 0 ORDER BY Title";
            
            DataTable result = DatabaseHelper.ExecuteQuery(query);
            
            foreach (DataRow row in result.Rows)
            {
                books.Add(CreateBookFromDataRow(row));
            }
            
            return books;
        }

        public List<Book> SearchBooks(string searchTerm)
        {
            List<Book> books = new List<Book>();
            string query = @"
                SELECT * FROM Books 
                WHERE IsActive = 1 AND (
                    Title LIKE @SearchTerm OR 
                    Author LIKE @SearchTerm OR 
                    ISBN LIKE @SearchTerm OR 
                    Category LIKE @SearchTerm
                )
                ORDER BY Title";
            
            SqlParameter[] parameters = {
                new SqlParameter("@SearchTerm", "%" + searchTerm + "%")
            };
            
            DataTable result = DatabaseHelper.ExecuteQuery(query, parameters);
            
            foreach (DataRow row in result.Rows)
            {
                books.Add(CreateBookFromDataRow(row));
            }
            
            return books;
        }

        public bool AddBook(Book book)
        {
            try
            {
                string query = @"
                    INSERT INTO Books (ISBN, Title, Author, Publisher, PublishedDate, Category, Description, TotalCopies, AvailableCopies, CreatedDate, IsActive)
                    VALUES (@ISBN, @Title, @Author, @Publisher, @PublishedDate, @Category, @Description, @TotalCopies, @AvailableCopies, @CreatedDate, @IsActive)";

                SqlParameter[] parameters = {
                    new SqlParameter("@ISBN", book.ISBN),
                    new SqlParameter("@Title", book.Title),
                    new SqlParameter("@Author", book.Author),
                    new SqlParameter("@Publisher", book.Publisher ?? (object)DBNull.Value),
                    new SqlParameter("@PublishedDate", book.PublishedDate ?? (object)DBNull.Value),
                    new SqlParameter("@Category", book.Category ?? (object)DBNull.Value),
                    new SqlParameter("@Description", book.Description ?? (object)DBNull.Value),
                    new SqlParameter("@TotalCopies", book.TotalCopies),
                    new SqlParameter("@AvailableCopies", book.TotalCopies),
                    new SqlParameter("@CreatedDate", book.CreatedDate),
                    new SqlParameter("@IsActive", book.IsActive)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateBook(Book book)
        {
            try
            {
                string query = @"
                    UPDATE Books SET 
                        Title = @Title, 
                        Author = @Author, 
                        Publisher = @Publisher, 
                        PublishedDate = @PublishedDate, 
                        Category = @Category, 
                        Description = @Description, 
                        TotalCopies = @TotalCopies,
                        UpdatedDate = @UpdatedDate
                    WHERE BookId = @BookId";

                SqlParameter[] parameters = {
                    new SqlParameter("@BookId", book.BookId),
                    new SqlParameter("@Title", book.Title),
                    new SqlParameter("@Author", book.Author),
                    new SqlParameter("@Publisher", book.Publisher ?? (object)DBNull.Value),
                    new SqlParameter("@PublishedDate", book.PublishedDate ?? (object)DBNull.Value),
                    new SqlParameter("@Category", book.Category ?? (object)DBNull.Value),
                    new SqlParameter("@Description", book.Description ?? (object)DBNull.Value),
                    new SqlParameter("@TotalCopies", book.TotalCopies),
                    new SqlParameter("@UpdatedDate", DateTime.Now)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteBook(int bookId)
        {
            try
            {
                // Check if book has active borrows
                string checkQuery = "SELECT COUNT(*) FROM BorrowRecords WHERE BookId = @BookId AND Status = 1";
                SqlParameter[] checkParams = { new SqlParameter("@BookId", bookId) };
                
                int activeBorrows = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParams));
                if (activeBorrows > 0)
                    return false;

                // Soft delete
                string query = "UPDATE Books SET IsActive = 0 WHERE BookId = @BookId";
                SqlParameter[] parameters = { new SqlParameter("@BookId", bookId) };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsISBNAvailable(string isbn)
        {
            string query = "SELECT COUNT(*) FROM Books WHERE ISBN = @ISBN AND IsActive = 1";
            SqlParameter[] parameters = {
                new SqlParameter("@ISBN", isbn)
            };
            
            int count = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));
            return count == 0;
        }

        public bool UpdateBookAvailability(int bookId, int availableCopies)
        {
            try
            {
                string query = "UPDATE Books SET AvailableCopies = @AvailableCopies, UpdatedDate = @UpdatedDate WHERE BookId = @BookId";
                SqlParameter[] parameters = {
                    new SqlParameter("@BookId", bookId),
                    new SqlParameter("@AvailableCopies", availableCopies),
                    new SqlParameter("@UpdatedDate", DateTime.Now)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Book CreateBookFromDataRow(DataRow row)
        {
            return new Book
            {
                BookId = Convert.ToInt32(row["BookId"]),
                ISBN = row["ISBN"].ToString(),
                Title = row["Title"].ToString(),
                Author = row["Author"].ToString(),
                Publisher = row["Publisher"] == DBNull.Value ? null : row["Publisher"].ToString(),
                PublishedDate = row["PublishedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["PublishedDate"]),
                Category = row["Category"] == DBNull.Value ? null : row["Category"].ToString(),
                Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                TotalCopies = Convert.ToInt32(row["TotalCopies"]),
                AvailableCopies = Convert.ToInt32(row["AvailableCopies"]),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                UpdatedDate = row["UpdatedDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["UpdatedDate"]),
                IsActive = Convert.ToBoolean(row["IsActive"])
            };
        }
    }
}
