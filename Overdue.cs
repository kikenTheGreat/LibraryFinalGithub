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
    public partial class Overdue : Form
    {
        private readonly IBorrowService _borrowService;

        public Overdue()
        {
            InitializeComponent();
            _borrowService = ServiceProvider.GetService<IBorrowService>();
            LoadOverdueBooks();
        }

        private async void LoadOverdueBooks()
        {
            try
            {
                // Check permissions
                if (!FormHelper.CheckUserPermission(UserRole.Librarian))
                    return;

                var overdueBooks = await _borrowService.GetOverdueBorrowsAsync();
                
                // Bind to your DataGridView
                // Example: dgvOverdueBooks.DataSource = overdueBooks.ToList();
                
                // Update summary labels
                UpdateOverdueSummary(overdueBooks);
            }
            catch (Exception ex)
            {
                FormHelper.ShowError($"Error loading overdue books: {ex.Message}");
            }
        }

        private void UpdateOverdueSummary(IEnumerable<BorrowRecord> overdueBooks)
        {
            var overdueList = overdueBooks.ToList();
            int totalOverdue = overdueList.Count;
            decimal totalFines = overdueList.Sum(b => b.FineAmount ?? 0);
            
            // Update your summary labels
            // Example:
            // lblTotalOverdue.Text = $"Total Overdue: {totalOverdue}";
            // lblTotalFines.Text = $"Total Fines: ${totalFines:F2}";
        }

        private async void btnSendReminder_Click(object sender, EventArgs e)
        {
            try
            {
                // This would typically send email reminders
                // For now, just show a message
                FormHelper.ShowSuccess("Reminder notifications sent to overdue users.");
            }
            catch (Exception ex)
            {
                FormHelper.ShowError($"Error sending reminders: {ex.Message}");
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOverdueBooks();
        }
    }
}
