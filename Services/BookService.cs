using Microsoft.EntityFrameworkCore;
using Library_Final.Data;
using Library_Final.Interfaces;
using Library_Final.Models;

namespace Library_Final.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<Book?> GetBookByIdAsync(int bookId)
        {
            return await _context.Books
                .Include(b => b.BorrowRecords)
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.IsActive);
        }

        public async Task<Book?> GetBookByISBNAsync(string isbn)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.ISBN == isbn && b.IsActive);
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .Where(b => b.IsActive)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();
            return await _context.Books
                .Where(b => b.IsActive && (
                    b.Title.ToLower().Contains(lowerSearchTerm) ||
                    b.Author.ToLower().Contains(lowerSearchTerm) ||
                    b.ISBN.Contains(searchTerm) ||
                    (b.Category != null && b.Category.ToLower().Contains(lowerSearchTerm))
                ))
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category)
        {
            return await _context.Books
                .Where(b => b.IsActive && b.Category == category)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
        {
            return await _context.Books
                .Where(b => b.IsActive && b.AvailableCopies > 0)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        public async Task<bool> CreateBookAsync(Book book)
        {
            try
            {
                if (await IsISBNAvailableAsync(book.ISBN) == false)
                    return false;

                book.CreatedDate = DateTime.Now;
                book.AvailableCopies = book.TotalCopies;

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateBookAsync(Book book)
        {
            try
            {
                book.UpdatedDate = DateTime.Now;
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteBookAsync(int bookId)
        {
            try
            {
                var book = await GetBookByIdAsync(bookId);
                if (book == null) return false;

                // Check if book has active borrows
                var hasActiveBorrows = await _context.BorrowRecords
                    .AnyAsync(br => br.BookId == bookId && br.Status == BorrowStatus.Borrowed);

                if (hasActiveBorrows)
                    return false; // Cannot delete book with active borrows

                book.IsActive = false; // Soft delete
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsISBNAvailableAsync(string isbn)
        {
            return !await _context.Books.AnyAsync(b => b.ISBN == isbn && b.IsActive);
        }

        public async Task<bool> UpdateBookAvailabilityAsync(int bookId, int availableCopies)
        {
            try
            {
                var book = await GetBookByIdAsync(bookId);
                if (book == null) return false;

                book.AvailableCopies = availableCopies;
                book.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _context.Books
                .Where(b => b.IsActive && !string.IsNullOrEmpty(b.Category))
                .Select(b => b.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }
    }
}
