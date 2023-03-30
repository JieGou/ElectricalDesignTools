using Firebase.Auth;
using System;
using System.Security.Permissions;
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
                FullName = _currentUser.DisplayName;
                Email = _currentUser.Email;
            }
        }
        private User? _currentUser;

        public string Email { get; set; } = "n/a";
        public string FullName { get; set; } = "Anonymous";
        public string UserId { get; set; } = "n/a";
        public string SubscriptionStatus { get; set; } = "Beta Trial";
        public DateTime Subscription_Start { get; set; } = DateTime.UtcNow;
        public DateTime Subscription_End { get; set; } = new DateTime(2023,09,30);
            
    }
}
