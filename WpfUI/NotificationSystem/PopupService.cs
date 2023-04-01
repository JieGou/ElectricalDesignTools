using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        ShowPopupNotificationAsync(args.Messsage);
    }

    public static async Task ShowPopupNotificationAsync(string notificationText)
    {
        await Task.Run(() => {
            App.Current.Dispatcher.Invoke(() => {
                if (NotificationPopup == null) {
                    NotificationPopup = new PopupNotifcationWindow();
                    NotificationPopup.Owner = MainWindow;
                }
                NotificationPopup.DataContext = new Notification(notificationText);
                NotificationPopup.Show();
            });
        });
       
    }

    public static void ClosePopupNotification(object sender, EdtNotificationEventArgs args)
    {
        ClosePopupNotificationAsync();
    }
    public static async Task ClosePopupNotificationAsync(int popupDuration = 0)
    {
        await Task.Delay(popupDuration);

        App.Current.Dispatcher.Invoke(() => 
        {
            if (NotificationPopup != null) {
                NotificationPopup.Close();
                NotificationPopup = null;
            }
        });
        
       
        
    }

   


}
