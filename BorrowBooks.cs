using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library_Final.Interfaces;
using Library_Final.Models;
using Library_Final.Utilities;

namespace Library_Final
{
    public partial class BorrowBooks : Form
    {
        private readonly IBookService _bookService;
        private readonly IBorrowService _borrowService;
        private readonly IUserService _userService;
        private readonly LogService _logService;

        public BorrowBooks()
        {
            InitializeComponent();
            _bookService = ServiceProvider.GetService<IBookService>();
            _borrowService = ServiceProvider.GetService<IBorrowService>();
            _userService = ServiceProvider.GetService<IUserService>();
            _logService = ServiceProvider.GetService<LogService>();
            LoadAvailableBooks();
        }

        private async void LoadAvailableBooks()
        {
            try
            {
                var books = await _bookService.GetAvailableBooksAsync();
                // Bind to your control
                // Example: cmbBooks.DataSource = books.ToList();
            }
            catch (Exception ex)
            {
                FormHelper.ShowError($"Error loading books: {ex.Message}");
            }
        }

        private async void btnBorrowBook_Click(object sender, EventArgs e)
        {
            try
            {
                var currentUser = SessionManager.Instance.CurrentUser;
                if (currentUser == null)
                {
                    FormHelper.ShowError("You must be logged in to borrow books.");
                    return;
                }

                // Get selected book ID and due date from form
                int bookId = GetSelectedBookId();
                DateTime dueDate = GetDueDateFromForm();

                if (bookId == 0)
                {
                    FormHelper.ShowWarning("Please select a book to borrow.");
                    return;
                }

                // Check if user can borrow more books
                if (!await _borrowService.CanUserBorrowAsync(currentUser.UserId))
                {
                    FormHelper.ShowError("You have reached the maximum number of borrowed books.");
                    return;
                }

                if (await _borrowService.BorrowBookAsync(currentUser.UserId, bookId, dueDate))
                {
                    await _logService.LogActionAsync(LogActions.BookBorrowed, "BorrowRecord", bookId);
                    FormHelper.ShowSuccess("Book borrowed successfully!");
                    LoadAvailableBooks();
                }
                else
                {
                    FormHelper.ShowError("Failed to borrow book. It might not be available.");
                }
            }
            catch (Exception ex)
            {
                FormHelper.ShowError($"Error borrowing book: {ex.Message}");
            }
        }

        // Helper methods - implement based on your form controls
        private int GetSelectedBookId() => 0; // Replace with actual logic
        private DateTime GetDueDateFromForm() => DateTime.Now.AddDays(14); // Default 2 weeks
    }
}
