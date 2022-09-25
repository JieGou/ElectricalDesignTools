using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.UI.Notifications;
using WpfUI.PopupWindows;
using static EDTLibrary.Services.EdtNotificationService;
using Notification = WpfUI.PopupWindows.Notification;

namespace WpfUI.Services;
public class PopupService
{
    public static NotificationPopup NotificationPopup { get; set; }

    public static void ShowNotification(object sender, EdtNotificationEventArgs args)
    {
        ShowNotification(args.Messsage);
    }

    public static void ShowNotification(string notification)
    {
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
            if (NotificationPopup == null) {
                NotificationPopup = new NotificationPopup();
            }
            NotificationPopup.DataContext = new Notification(notification);
            NotificationPopup.Show();

        }));
       
    }

    public static void CloseNotification(object sender, EdtNotificationEventArgs args)
    {
        CloseNotification();
    }
    public static void CloseNotification()
    {
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
            if (NotificationPopup != null) {
                NotificationPopup.Close();
                NotificationPopup = null;
            }
        }));
        
    }

   


}
