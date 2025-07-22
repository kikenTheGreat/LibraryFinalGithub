using Microsoft.EntityFrameworkCore;
using Library_Final.Data;
using Library_Final.Interfaces;
using Library_Final.Models;

namespace Library_Final.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly LibraryDbContext _context;
        private const int MaxBooksPerUser = 3;
        private const decimal FinePerDay = 1.00m;

        public BorrowService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<BorrowRecord?> GetBorrowRecordByIdAsync(int borrowId)
        {
            return await _context.BorrowRecords
                .Include(br => br.User)
                .Include(br => br.Book)
                .FirstOrDefaultAsync(br => br.BorrowId == borrowId);
        }

        public async Task<IEnumerable<BorrowRecord>> GetBorrowRecordsByUserIdAsync(int userId)
        {
            return await _context.BorrowRecords
                .Include(br => br.Book)
                .Where(br => br.UserId == userId)
                .OrderByDescending(br => br.BorrowDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRecord>> GetBorrowRecordsByBookIdAsync(int bookId)
        {
            return await _context.BorrowRecords
                .Include(br => br.User)
                .Where(br => br.BookId == bookId)
                .OrderByDescending(br => br.BorrowDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRecord>> GetActiveBorrowsAsync()
        {
            return await _context.BorrowRecords
                .Include(br => br.User)
                .Include(br => br.Book)
                .Where(br => br.Status == BorrowStatus.Borrowed)
                .OrderBy(br => br.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRecord>> GetOverdueBorrowsAsync()
        {
            var today = DateTime.Now.Date;
            return await _context.BorrowRecords
                .Include(br => br.User)
                .Include(br => br.Book)
                .Where(br => br.Status == BorrowStatus.Borrowed && br.DueDate.Date < today)
                .OrderBy(br => br.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRecord>> GetBorrowHistoryAsync()
        {
            return await _context.BorrowRecords
                .Include(br => br.User)
                .Include(br => br.Book)
                .OrderByDescending(br => br.BorrowDate)
                .ToListAsync();
        }

        public async Task<bool> BorrowBookAsync(int userId, int bookId, DateTime dueDate)
        {
            try
            {
                // Check if user can borrow more books
                if (!await CanUserBorrowAsync(userId))
                    return false;

                // Check if book is available
                var book = await _context.Books.FindAsync(bookId);
                if (book == null || book.AvailableCopies <= 0)
                    return false;

                // Create borrow record
                var borrowRecord = new BorrowRecord
                {
                    UserId = userId,
                    BookId = bookId,
                    BorrowDate = DateTime.Now,
                    DueDate = dueDate,
                    Status = BorrowStatus.Borrowed
                };

                _context.BorrowRecords.Add(borrowRecord);

                // Update book availability
                book.AvailableCopies--;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ReturnBookAsync(int borrowId, string? notes = null)
        {
            try
            {
                var borrowRecord = await GetBorrowRecordByIdAsync(borrowId);
                if (borrowRecord == null || borrowRecord.Status != BorrowStatus.Borrowed)
                    return false;

                borrowRecord.ReturnDate = DateTime.Now;
                borrowRecord.Status = BorrowStatus.Returned;
                borrowRecord.Notes = notes;

                // Calculate fine if overdue
                if (borrowRecord.IsOverdue)
                {
                    borrowRecord.FineAmount = await CalculateFineAsync(borrowId);
                }

                // Update book availability
                var book = await _context.Books.FindAsync(borrowRecord.BookId);
                if (book != null)
                {
                    book.AvailableCopies++;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RenewBookAsync(int borrowId, DateTime newDueDate)
        {
            try
            {
                var borrowRecord = await GetBorrowRecordByIdAsync(borrowId);
                if (borrowRecord == null || borrowRecord.Status != BorrowStatus.Borrowed)
                    return false;

                borrowRecord.DueDate = newDueDate;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkAsLostAsync(int borrowId, decimal fineAmount)
        {
            try
            {
                var borrowRecord = await GetBorrowRecordByIdAsync(borrowId);
                if (borrowRecord == null)
                    return false;

                borrowRecord.Status = BorrowStatus.Lost;
                borrowRecord.FineAmount = fineAmount;
                borrowRecord.ReturnDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkAsDamagedAsync(int borrowId, decimal fineAmount)
        {
            try
            {
                var borrowRecord = await GetBorrowRecordByIdAsync(borrowId);
                if (borrowRecord == null)
                    return false;

                borrowRecord.Status = BorrowStatus.Damaged;
                borrowRecord.FineAmount = fineAmount;
                borrowRecord.ReturnDate = DateTime.Now;

                // Update book availability
                var book = await _context.Books.FindAsync(borrowRecord.BookId);
                if (book != null)
                {
                    book.AvailableCopies++;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<decimal> CalculateFineAsync(int borrowId)
        {
            var borrowRecord = await GetBorrowRecordByIdAsync(borrowId);
            if (borrowRecord == null || !borrowRecord.IsOverdue)
                return 0;

            return borrowRecord.DaysOverdue * FinePerDay;
        }

        public async Task<bool> CanUserBorrowAsync(int userId)
        {
            var activeBorrowCount = await GetActiveBorrowCountAsync(userId);
            return activeBorrowCount < MaxBooksPerUser;
        }

        public async Task<int> GetActiveBorrowCountAsync(int userId)
        {
            return await _context.BorrowRecords
                .CountAsync(br => br.UserId == userId && br.Status == BorrowStatus.Borrowed);
        }
    }
}
