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
    public partial class ManageBooks : Form
    {
        private readonly IBookService _bookService;
        private readonly LogService _logService;

        public ManageBooks()
        {
            InitializeComponent();
            _bookService = ServiceProvider.GetService<IBookService>();
            _logService = ServiceProvider.GetService<LogService>();
            LoadBooks();
        }

        private async void LoadBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync();
                // Bind to your DataGridView or ListBox
                // Example: dgvBooks.DataSource = books.ToList();
            }
            catch (Exception ex)
            {
                FormHelper.ShowError($"Error loading books: {ex.Message}");
            }
        }

        private async void btnAddBook_Click(object sender, EventArgs e)
        {
            try
            {
                // Get values from form controls (you'll need to add these)
                var book = new Book
                {
                    ISBN = GetISBNFromForm(),
                    Title = GetTitleFromForm(),
                    Author = GetAuthorFromForm(),
                    Publisher = GetPublisherFromForm(),
                    Category = GetCategoryFromForm(),
                    Description = GetDescriptionFromForm(),
                    TotalCopies = GetTotalCopiesFromForm(),
                    AvailableCopies = GetTotalCopiesFromForm()
                };

                if (await _bookService.CreateBookAsync(book))
                {
                    await _logService.LogActionAsync(LogActions.BookAdded, "Book", book.BookId);
                    FormHelper.ShowSuccess("Book added successfully!");
                    LoadBooks();
                    ClearForm();
                }
                else
                {
                    FormHelper.ShowError("Failed to add book. ISBN might already exist.");
                }
            }
            catch (Exception ex)
            {
                FormHelper.ShowError($"Error adding book: {ex.Message}");
            }
        }

        // Helper methods - you'll need to implement these based on your form controls
        private string GetISBNFromForm() => string.Empty; // Replace with actual control
        private string GetTitleFromForm() => string.Empty; // Replace with actual control
        private string GetAuthorFromForm() => string.Empty; // Replace with actual control
        private string GetPublisherFromForm() => string.Empty; // Replace with actual control
        private string GetCategoryFromForm() => string.Empty; // Replace with actual control
        private string GetDescriptionFromForm() => string.Empty; // Replace with actual control
        private int GetTotalCopiesFromForm() => 1; // Replace with actual control

        private void ClearForm()
        {
            FormHelper.ClearTextBoxes(this);
        }
    }
}
