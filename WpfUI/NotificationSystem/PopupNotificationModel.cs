using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.PopupWindows;
internal class PopupNotificationModel
{
   
    public PopupNotificationModel(string notificationText)
    {
        NotificationText = notificationText;
    }
    public string NotificationText { get; set; }
}
