using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.NoificationSystem
{
    [AddINotifyPropertyChangedInterface]

    class NotificationModel
    {
        public NotificationModel(string caption, string notificationText) 
        {
            Caption = caption;
            NotificationText = notificationText;
        }

        public string Caption { get; }
        public string NotificationText { get; set; }
    }
}
