using Library_Final.Models;

namespace Library_Final.Interfaces
{
    public interface IBookService
    {
        Task<Book?> GetBookByIdAsync(int bookId);
        Task<Book?> GetBookByISBNAsync(string isbn);
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(string category);
        Task<IEnumerable<Book>> GetAvailableBooksAsync();
        Task<bool> CreateBookAsync(Book book);
        Task<bool> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int bookId);
        Task<bool> IsISBNAvailableAsync(string isbn);
        Task<bool> UpdateBookAvailabilityAsync(int bookId, int availableCopies);
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}
