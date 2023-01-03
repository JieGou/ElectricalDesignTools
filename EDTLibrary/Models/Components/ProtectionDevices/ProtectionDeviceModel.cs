using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components.ProtectionDevices;

[AddINotifyPropertyChangedInterface]

//  - Category = CctComponent
//	- Sub-Category = ProtectionDevice, Starter, Disconnect
//	- Type = BKR, FDS, UDS, DOL, VSD, 
//	- SubType = DefaultDcn,, Diconnect,
public class ProtectionDeviceModel : ComponentModelBase, IProtectionDevice
{
    private bool _isStandAlone;
    public bool IsStandAlone {
        get => _isStandAlone;
        set 
        { 
            _isStandAlone = value;
            OnPropertyUpdated();
        }
    }

    public string StarterSize { get; set; }
}
