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
    public partial class Return : Form
    {
        private readonly IBorrowService _borrowService;
        private readonly LogService _logService;

        public Return()
        {
            InitializeComponent();
            _borrowService = ServiceProvider.GetService<IBorrowService>();
            _logService = ServiceProvider.GetService<LogService>();
            LoadBorrowedBooks();
        }

        private async void LoadBorrowedBooks()
        {
            try
            {
                var currentUser = SessionManager.Instance.CurrentUser;
                if (currentUser == null) return;

                var borrowedBooks = await _borrowService.GetBorrowRecordsByUserIdAsync(currentUser.UserId);
                var activeBorrows = borrowedBooks.Where(b => b.Status == BorrowStatus.Borrowed);
                
                // Bind to your control
                // Example: dgvBorrowedBooks.DataSource = activeBorrows.ToList();
            }
            catch (Exception ex)
            {
                FormHelper.ShowError($"Error loading borrowed books: {ex.Message}");
            }
        }

        private async void btnReturnBook_Click(object sender, EventArgs e)
        {
            try
            {
                int borrowId = GetSelectedBorrowId();
                if (borrowId == 0)
                {
                    FormHelper.ShowWarning("Please select a book to return.");
                    return;
                }

                string notes = GetNotesFromForm();

                if (await _borrowService.ReturnBookAsync(borrowId, notes))
                {
                    await _logService.LogActionAsync(LogActions.BookReturned, "BorrowRecord", borrowId);
                    FormHelper.ShowSuccess("Book returned successfully!");
                    LoadBorrowedBooks();
                }
                else
                {
                    FormHelper.ShowError("Failed to return book.");
                }
            }
            catch (Exception ex)
            {
                FormHelper.ShowError($"Error returning book: {ex.Message}");
            }
        }

        private async void btnCalculateFine_Click(object sender, EventArgs e)
        {
            try
            {
                int borrowId = GetSelectedBorrowId();
                if (borrowId == 0) return;

                decimal fine = await _borrowService.CalculateFineAsync(borrowId);
                if (fine > 0)
                {
                    FormHelper.ShowWarning($"Fine amount: ${fine:F2}");
                }
                else
                {
                    FormHelper.ShowSuccess("No fine applicable.");
                }
            }
            catch (Exception ex)
            {
                FormHelper.ShowError($"Error calculating fine: {ex.Message}");
            }
        }

        // Helper methods
        private int GetSelectedBorrowId() => 0; // Replace with actual logic
        private string GetNotesFromForm() => string.Empty; // Replace with actual control
    }
}
