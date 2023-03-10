using System;
using System.Collections.Generic;
using System.Text;
using static FireSharp.Library.EdtAuthorization;

namespace FireSharp.Library
{
    public class UserAccount
    {
        public string Email { get; set; }
        public bool IsSubscribed { get; set; }
        public DateTime Subscription_Start { get; set; } = DateTime.Now;
        public DateTime Subscription_End { get; set; } = DateTime.Now;

    }
}
