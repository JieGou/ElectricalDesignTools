using Microsoft.Diagnostics.Tracing.Parsers.IIS_Trace;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Services;
public enum EdtNotificationCategory
{
    InternalError,
    UserError,
    Notification,
}

public enum EdtNotificationType
{
    VoltageChange,
}

public partial class EdtNotificationService
{
    //Error
    public static void SendError(object sender, string message, string caption, Exception ex)
    {
        var args = new EdtNotificationEventArgs();
        args.Messsage = message;
        args.Caption = caption;
        args.Exception = ex;
        args.EdtNotificationType = EdtNotificationCategory.InternalError;
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
        args.EdtNotificationType = EdtNotificationCategory.UserError;

        OnAlertSent(sender, args);
    }
    public static EventHandler<EdtNotificationEventArgs> AlertSent;
    private static void OnAlertSent(object sender, EdtNotificationEventArgs args)
    {

        if (AlertSent != null) {
            AlertSent(sender, args);

        }
    }



    public static void SendPopupNotification(object sender, string message)
    {
        var args = new EdtNotificationEventArgs();
        args.Messsage = message;
        args.EdtNotificationType = EdtNotificationCategory.Notification;
        OnPopupNotificationSent(sender, args);
    }
    public static EventHandler<EdtNotificationEventArgs> NotificationSent;
    private static void OnPopupNotificationSent(object sender, EdtNotificationEventArgs args)
    {

        if (NotificationSent != null) {
            NotificationSent(sender, args);

        }
    }

    public static void CloseoPupNotification(object sender)
    {
        var args = new EdtNotificationEventArgs();
        args.EdtNotificationType = EdtNotificationCategory.Notification;
        OnPopupNotificationClosed(sender, args);
    }
    public static EventHandler<EdtNotificationEventArgs> NotificationClosed;
    private static void OnPopupNotificationClosed(object sender, EdtNotificationEventArgs args)
    {

        if (NotificationClosed != null) {
            NotificationClosed(sender, args);

        }
    }

}

