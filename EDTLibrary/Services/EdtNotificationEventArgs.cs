using System;

namespace EDTLibrary.Services;

public partial class EdtNotificationService
{
    public class EdtNotificationEventArgs : EventArgs
    {
        public string Category { get; set; }
        public string Type { get; set; }
        public string Messsage { get; set; }
        public string Caption { get; set; }
        public Exception Exception { get; set; }
        public EdtNotificationCategory EdtNotificationType { get; set; }
    }

}

