using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.PopupWindows;
internal class Notification
{
    public Notification()
    {

    }
    public Notification(string notificationText)
    {
        NotificationText = notificationText;
    }
    public string NotificationText { get; set; }
}
