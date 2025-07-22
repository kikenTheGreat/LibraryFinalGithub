using Library_Final.Models;

namespace Library_Final.Utilities
{
    public static class FormHelper
    {
        public static void ShowError(string message, string title = "Error")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowSuccess(string message, string title = "Success")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static DialogResult ShowConfirmation(string message, string title = "Confirm")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static bool CheckUserPermission(UserRole requiredRole)
        {
            var currentUser = SessionManager.Instance.CurrentUser;
            if (currentUser == null)
            {
                ShowError("You must be logged in to perform this action.");
                return false;
            }

            if (currentUser.Role < requiredRole)
            {
                ShowError("You don't have permission to perform this action.");
                return false;
            }

            return true;
        }

        public static void ClearTextBoxes(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Clear();
                }
                else if (control.HasChildren)
                {
                    ClearTextBoxes(control);
                }
            }
        }

        public static bool ValidateRequiredFields(params Control[] controls)
        {
            foreach (var control in controls)
            {
                if (control is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
                {
                    ShowWarning("Please fill in all required fields.");
                    textBox.Focus();
                    return false;
                }
            }
            return true;
        }
    }
}
