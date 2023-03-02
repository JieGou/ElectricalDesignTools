using EDTLibrary;
using EDTLibrary.Models.Loads;
using EDTLibrary.Settings;
using System;
using System.Data.Entity.Core.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using WpfUI.Converters;
using WpfUI.NoificationSystem;
using static EDTLibrary.Services.EdtNotificationService;

namespace WpfUI.Helpers
{
    public class NotificationHandler
    {
        static bool isAlertActive;
        public static void AlertReceived(object sender, EdtNotificationEventArgs args)
        {
            ShowAlert(args.Messsage, args.Caption, args.NotificationName);
        }

        static NotificationWindow notificationWindow = null;

        public static void ShowAlert(string message, string caption, string notificationName="none")
        {
            var prop = EdtAppSettings.Default.GetType().GetProperty(notificationName);

            if (prop != null) {
                var notificationModel = new NotificationModel(caption, message + Environment.NewLine, prop.Name);
                var binding = new Binding();
                binding.Source = EdtAppSettings.Default;
                binding.Path = new PropertyPath(prop.Name);
                binding.Converter = new BooleanInverter();


                if ((bool)prop.GetValue(EdtAppSettings.Default) || notificationName == "") {
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                        if (notificationWindow == null || notificationWindow.IsLoaded == false) {
                            notificationWindow = new NotificationWindow();
                            if (notificationName=="none") {
                                notificationWindow.chkDisableAlert.Visibility = Visibility.Collapsed;
                            }

                            notificationWindow.chkDisableAlert.SetBinding(CheckBox.IsCheckedProperty, binding);
                            notificationWindow.DataContext = notificationModel;
                            notificationWindow.ShowDialog();
                        }
                        else {
                            var notificationModel = (NotificationModel)notificationWindow.DataContext;
                            notificationModel.NotificationText += (message + Environment.NewLine);
                            notificationWindow.Focus();
                        }
                        EdtAppSettings.Default.Save();
                    }));
                }
            }

            else {

                var notificationModel = new NotificationModel(caption, message + Environment.NewLine, "");
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                        if (notificationWindow == null || notificationWindow.IsLoaded == false) {
                            notificationWindow = new NotificationWindow();

                            notificationWindow.DataContext = notificationModel;
                            notificationWindow.ShowDialog();
                        }
                        else {
                            var notificationModel = (NotificationModel)notificationWindow.DataContext;
                            notificationModel.NotificationText += (message + Environment.NewLine);
                            notificationWindow.Focus();
                        }
                        EdtAppSettings.Default.Save();
                    }));
            }

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
