using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Windows.UI.Notifications;
using WpfUI.PopupWindows;
using static EDTLibrary.Services.EdtNotificationService;
using Notification = WpfUI.PopupWindows.PopupNotificationModel;

namespace WpfUI.Services;
public class PopupService
{
    public static PopupNotifcationWindow NotificationPopup { get; set; }
    public static MainWindow MainWindow { get; set; }

    public static void ShowPopupNotification(object sender, EdtNotificationEventArgs args)
    {
        ShowPopupNotification(args.Messsage);
    }

    public static void ShowPopupNotification(string notificationText)
    {
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
            if (NotificationPopup == null) {
                NotificationPopup = new PopupNotifcationWindow();
                NotificationPopup.Owner = MainWindow;
            }
            NotificationPopup.DataContext = new Notification(notificationText);
            NotificationPopup.Show();

        }));
       
    }

    public static void ClosePopupNotification(object sender, EdtNotificationEventArgs args)
    {
        ClosePopupNotification();
    }
    public static void ClosePopupNotification()
    {
        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
            if (NotificationPopup != null) {
                NotificationPopup.Close();
                NotificationPopup = null;
            }
        }));
        
    }

   


}
