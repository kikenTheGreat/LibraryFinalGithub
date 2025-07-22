using Library_Final.Models;

namespace Library_Final.Interfaces
{
    public interface IBorrowService
    {
        Task<BorrowRecord?> GetBorrowRecordByIdAsync(int borrowId);
        Task<IEnumerable<BorrowRecord>> GetBorrowRecordsByUserIdAsync(int userId);
        Task<IEnumerable<BorrowRecord>> GetBorrowRecordsByBookIdAsync(int bookId);
        Task<IEnumerable<BorrowRecord>> GetActiveBorrowsAsync();
        Task<IEnumerable<BorrowRecord>> GetOverdueBorrowsAsync();
        Task<IEnumerable<BorrowRecord>> GetBorrowHistoryAsync();
        Task<bool> BorrowBookAsync(int userId, int bookId, DateTime dueDate);
        Task<bool> ReturnBookAsync(int borrowId, string? notes = null);
        Task<bool> RenewBookAsync(int borrowId, DateTime newDueDate);
        Task<bool> MarkAsLostAsync(int borrowId, decimal fineAmount);
        Task<bool> MarkAsDamagedAsync(int borrowId, decimal fineAmount);
        Task<decimal> CalculateFineAsync(int borrowId);
        Task<bool> CanUserBorrowAsync(int userId);
        Task<int> GetActiveBorrowCountAsync(int userId);
    }
}
