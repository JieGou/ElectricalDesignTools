using EdtLibrary.Commands;
using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Selectors;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Components.ProtectionDevices;

//  - Category = CctComponent
//	- Sub-Category = ProtectionDevice, Starter, Disconnect
//	- Type = Breaker, FDS, UDS, DOL, VSD, 
//	- SubType = DefaultDcn,, Diconnect,

[AddINotifyPropertyChangedInterface]
public class ProtectionDeviceModel : ComponentModelBase, IProtectionDevice
{
    public ProtectionDeviceModel()
    {
    }
    private bool _isStandAlone;
    public bool IsStandAlone {
        get => _isStandAlone;
        set 
        { 
            _isStandAlone = value;
            OnPropertyUpdated();
        }
    }


    public override string Type
    {
        get => _type;
        set
        {
            if (value == null) return;
            if (value == _type) return;
            var oldValue = _type;
            _type = value;

            if (DaManager.Importing) return;
            if (DaManager.GettingRecords) return;

            if (_type == DisconnectTypes.FDS.ToString() || _type == DisconnectTypes.FWDS.ToString()) {
                var owner = (IPowerConsumer)Owner;
                if (owner != null) {
                    TripAmps = TypeManager.BreakerTripSizes.FirstOrDefault(f => f.TripAmps >= owner.Fla).TripAmps;
                }
            }
            PropertyModelManager.DeletePropModel(PropertyModel);
            PropertyModel = PropertyModelManager.CreateNewPropModel(_type, this);
            PropertyModel.Owner = this;
            PropertyModelId = PropertyModel.Id;

            AddOrDeleteLcsAnalogCable();
            FrameAmps = ProtectionDeviceManager.GetPdFrameAmps(this, (IPowerConsumer)Owner);
            TypeList = ComponentTypeSelector.GetComponentTypeList(this);

            UndoManager.AddUndoCommand(this, nameof(Type), oldValue, _type);
            OnPropertyUpdated();
        }
    }

    
    private string _type;

    private void AddOrDeleteLcsAnalogCable()
    {
        if (Owner is ILoad) {
            if ((Owner as ILoad).Lcs != null && (Owner as ILoad).Lcs.AnalogCable != null) {
                CableManager.DeleteLcsAnalogCable((Owner as ILoad).Lcs, ScenarioManager.ListManager);
            }
        }

        if (_type == StarterTypes.VSD.ToString() ||
            _type == StarterTypes.VFD.ToString() ||
            _type == StarterTypes.RVS.ToString()) {

            if (Owner is ILoad) {
                CableManager.CreateLcsAnalogCableForProtectionDevice((ILoad)Owner, ScenarioManager.ListManager);
            }
        }

    }

}
