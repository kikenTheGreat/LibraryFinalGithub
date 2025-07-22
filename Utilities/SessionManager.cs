using Library_Final.Models;

namespace Library_Final.Utilities
{
    public class SessionManager
    {
        private static SessionManager? _instance;
        private static readonly object _lock = new object();

        public static SessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new SessionManager();
                    }
                }
                return _instance;
            }
        }

        public User? CurrentUser { get; private set; }
        public DateTime LoginTime { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;

        private SessionManager() { }

        public void Login(User user)
        {
            CurrentUser = user;
            LoginTime = DateTime.Now;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public bool HasRole(UserRole role)
        {
            return CurrentUser?.Role == role;
        }

        public bool HasAnyRole(params UserRole[] roles)
        {
            return CurrentUser != null && roles.Contains(CurrentUser.Role);
        }
    }
}
