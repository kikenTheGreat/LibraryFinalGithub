using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Library_Final
{
    /// <summary>
    /// Global session data to track the currently logged-in employee
    /// This ensures all forms use the same employee information
    /// </summary>
    public static class SessionData
    {
        /// <summary>
        /// The EmployeeID of the currently logged-in user
        /// </summary>
        public static int CurrentEmployeeID { get; set; } = 0;

        /// <summary>
        /// The Username of the currently logged-in user
        /// </summary>
        public static string CurrentUserName { get; set; } = "";

        /// <summary>
        /// Optional: Store additional user information
        /// </summary>
        public static string CurrentUserFullName { get; set; } = "";
        public static string CurrentUserRole { get; set; } = "";

        /// <summary>
        /// Store the profile image to avoid repeated database calls
        /// </summary>
        public static Image CurrentUserProfileImage { get; set; } = null;

        /// <summary>
        /// Store first and last name separately
        /// </summary>
        public static string CurrentUserFirstName { get; set; } = "";
        public static string CurrentUserLastName { get; set; } = "";

        /// <summary>
        /// Check if a user is currently logged in
        /// </summary>
        public static bool IsLoggedIn => CurrentEmployeeID > 0;

        /// <summary>
        /// Clear all session data (used during logout)
        /// </summary>
        public static void ClearSession()
        {
            CurrentEmployeeID = 0;
            CurrentUserName = "";
            CurrentUserFullName = "";
            CurrentUserRole = "";
            CurrentUserFirstName = "";
            CurrentUserLastName = "";

            // Dispose image if exists
            if (CurrentUserProfileImage != null)
            {
                CurrentUserProfileImage.Dispose();
                CurrentUserProfileImage = null;
            }
        }

        /// <summary>
        /// Initialize session with user data (called during login)
        /// </summary>
        public static void InitializeSession(int employeeId, string username, string fullName = "", string role = "")
        {
            CurrentEmployeeID = employeeId;
            CurrentUserName = username;
            CurrentUserFullName = fullName;
            CurrentUserRole = role;
        }

        /// <summary>
        /// Initialize session with complete user data including profile image
        /// </summary>
        public static void InitializeSessionComplete(int employeeId, string username, string firstName,
            string lastName, Image profileImage = null, string role = "")
        {
            CurrentEmployeeID = employeeId;
            CurrentUserName = username;
            CurrentUserFirstName = firstName;
            CurrentUserLastName = lastName;
            CurrentUserFullName = $"{firstName} {lastName}";
            CurrentUserRole = role;

            // Clone the image to avoid disposal issues
            if (profileImage != null)
            {
                CurrentUserProfileImage = (Image)profileImage.Clone();
            }
        }

        /// <summary>
        /// Get a display name for the current user
        /// </summary>
        public static string GetDisplayName()
        {
            if (!string.IsNullOrEmpty(CurrentUserFullName))
                return CurrentUserFullName;
            if (!string.IsNullOrEmpty(CurrentUserName))
                return CurrentUserName;
            return "Guest";
        }

        /// <summary>
        /// Check if session data is fully loaded
        /// </summary>
        public static bool IsSessionDataComplete()
        {
            return IsLoggedIn &&
                   !string.IsNullOrEmpty(CurrentUserFullName) &&
                   CurrentUserProfileImage != null;
        }
    }
}