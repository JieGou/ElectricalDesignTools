using Firebase.Auth;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WpfUI._Authentication
{
    public class EdtUserAccount
    {
        public User? CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
                Username = _currentUser.DisplayName;
                Email = _currentUser.Email;
            }
        }
        private User? _currentUser;

        public string Email { get; set; } = "n/a";
        public string Username { get; set; } = "Anonymous";
        public bool IsSubscribed { get; set; }
        public DateTime Subscription_Start { get; set; } = DateTime.Now;
        public DateTime Subscription_End { get; set; } = DateTime.Now;

    }
}
