using Microsoft.Diagnostics.Tracing.Parsers.IIS_Trace;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Services;
public class NotificationService
{
    //Error
    public static void SendError(object sender, string message, string caption, Exception ex)
    {
        var args = new EdtNotificationEventArgs();
        args.Messsage = message;
        args.Caption = caption;
        args.Exception = ex;
        args.EdtNotificationType = EdtNotificationType.InternalError;
        OnErrorSent(sender, args);
    }
    public static EventHandler<EdtNotificationEventArgs> ErrorSent;
    private static void OnErrorSent(object sender, EdtNotificationEventArgs args)
    {

        if (ErrorSent != null) {
            ErrorSent(sender, args);
            
        }
    }


    //Notification
    public static void SendAlert(object sender, string message, string caption)
    {
        var args = new EdtNotificationEventArgs();
        args.Messsage = message;
        args.Caption = caption;
        args.Exception = new Exception(message);
        args.EdtNotificationType = EdtNotificationType.UserError;

        OnAlertSent(sender, args);
    }
    public static EventHandler<EdtNotificationEventArgs> AlertSent;
    private static void OnAlertSent(object sender, EdtNotificationEventArgs args)
    {

        if (AlertSent != null) {
            AlertSent(sender, args);

        }
    }
    public class EdtNotificationEventArgs : EventArgs
    {
        public string Messsage { get; set; }
        public string Caption { get; set; }
        public Exception Exception { get; set; }
        public EdtNotificationType EdtNotificationType { get; set; }
    }


    public static void SendNotification(object sender, string message, string caption, Exception ex)
    {
        var args = new EdtNotificationEventArgs();
        args.Messsage = message;
        args.Caption = caption;
        args.Exception = ex;
        args.EdtNotificationType = EdtNotificationType.Notification;
        OnNotificationSent(sender, args);
    }
    public static EventHandler<EdtNotificationEventArgs> NotificationSent;
    private static void OnNotificationSent(object sender, EdtNotificationEventArgs args)
    {

        if (NotificationSent != null) {
            NotificationSent(sender, args);

        }
    }

}
public enum EdtNotificationType
{
    InternalError, 
    UserError,
    Notification,
}
