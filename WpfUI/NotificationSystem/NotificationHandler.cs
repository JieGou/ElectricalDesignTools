using System;
using System.Windows;
using System.Windows.Threading;
using WpfUI.NoificationSystem;
using static EDTLibrary.Services.EdtNotificationService;

namespace WpfUI.Helpers
{
    public class NotificationHandler
    {
        static bool isAlertActive;
        public static void AlertReceived(object sender, EdtNotificationEventArgs args)
        {
            ShowAlert(args.Messsage, args.Caption);
        }

        static NotificationWindow notificationWindow = null;
        static string notificationMessage;

        public static void ShowAlert(string message, string caption = "User Error", MessageBoxImage image = MessageBoxImage.Warning)
        {

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                if (notificationWindow == null || notificationWindow.IsLoaded == false) {
                    notificationMessage = message + Environment.NewLine;
                    notificationWindow = new NotificationWindow();
                    notificationWindow.DataContext = new NotificationModel(caption, notificationMessage);
                    notificationWindow.ShowDialog();
                }
                else {
                    var notificationModel = (NotificationModel)notificationWindow.DataContext;
                    notificationModel.NotificationText += (message + Environment.NewLine);
                    notificationWindow.Focus();
                }

            }));
        }



        public static void ErrorReceived(object sender, EdtNotificationEventArgs args)
        {
            ShowErrorMessage(args.Exception);
        }
        public static void ShowErrorMessage(Exception ex, string debugMessage ="")
        {

           
#if DEBUG
            if (ex.Data.Contains("UserMessage")) {
                MessageBox.Show($"UserMessage: \n\n{ex.Data["UserMessage"]}\n\n\n" + 
                    $"Error: \n\n{ex.Message}\n\n\n" +
                                $"Stack Trace: \n\n {ex}", "Error");
            }
            else {
                MessageBox.Show(ex.ToString());
            }
#endif

#if !DEBUG

            if (ex.Data.Contains("UserMessage")) {
                 MessageBox.Show($"Error: \n\n{ex.Message}\n\n\n" +
                                $"UserMessage: \n\n{ex.Data["UserMessage"]}\n\n\n", "Error");
            }
            else {
                MessageBox.Show(ex.ToString());
            }
#endif



        }
    }
}
