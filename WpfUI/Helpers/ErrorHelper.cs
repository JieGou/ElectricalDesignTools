using EDTLibrary.DataAccess;
using EDTLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using static EDTLibrary.Services.NotificationService;

namespace WpfUI.Helpers
{
    public class ErrorHelper
    {


        

        public static void ShowAlert(object sender, EdtNotificationEventArgs args)
        {
            NotifyUserError(args.Messsage, args.Caption);
        }

        public static void NotifyUserError(string message, string caption = "User Error", MessageBoxImage image = MessageBoxImage.Warning)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                MessageBox.Show(message, caption, MessageBoxButton.OK, image);
            }));
        }



        public static void ShowError(object sender, EdtNotificationEventArgs args)
        {
            ShowErrorMessage(args.Exception);
        }
        public static void ShowErrorMessage(Exception ex)
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
