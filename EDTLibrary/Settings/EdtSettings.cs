using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Settings;
public class EdtSettings
{
    public bool AutoSize_ProtectionDevice { get; set; }
    public bool AutoSize_PowerCable{ get; set; }
    public bool Notification_VoltageChange { get; set; }
    public bool Notification_CableAlreadyInRaceway { get; set; }
}
