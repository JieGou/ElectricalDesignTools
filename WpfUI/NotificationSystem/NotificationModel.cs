using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.NoificationSystem
{
    [AddINotifyPropertyChangedInterface]

    class NotificationModel
    {
        public NotificationModel(string caption, string notificationText, string notificationName) 
        {
            Caption = caption;
            NotificationText = notificationText;
            NotificationName = notificationName;
        }

        public string Caption { get; }
        public string NotificationText { get; set; }
        public string NotificationName { get; set; }
    }
}
