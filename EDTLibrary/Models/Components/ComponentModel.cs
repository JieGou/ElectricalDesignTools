using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData.TypeModels;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.DistributionEquipment;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.Models.Components;

[AddINotifyPropertyChangedInterface]

public class ComponentModel : IComponentEdt
{
    public ListManager ListManager { get; set; }

    public ComponentModel()
    {
        //Category = Categories.Component.ToString();
    }
    public int Id { get; set; }

    private string _tag;
    public string Tag
    {
        get { return _tag; }
        set
        {
            var oldValue = _tag;
            _tag = value;
            if (DaManager.GettingRecords == false) {
                if (Owner != null) {
                    if (CableManager.IsUpdatingPowerCables == false) {
                        //CableManager.AddAndUpdateLoadPowerComponentCablesAsync((IPowerConsumer)Owner, ScenarioManager.ListManager);
                    }
                }
            }
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommandDetail { Item = this, PropName = nameof(Tag), OldValue = oldValue, NewValue = _tag };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }

    public string Description { get; set; }
    public string Category { get; set; } //Component
    public string SubCategory { get; set; }

    public string Type { get; set; }
    public string SubType { get; set; }
    public List<string> TypeList
    {
        get
        {
            _typelist.Clear();
            if (Type == ComponentTypes.UDS.ToString() || Type == ComponentTypes.FDS.ToString()) {
                _typelist.Add(ComponentTypes.UDS.ToString());
                _typelist.Add(ComponentTypes.FDS.ToString());
            }
            else if (Type == ComponentTypes.VSD.ToString() || Type == ComponentTypes.VFD.ToString() || Type == ComponentTypes.RVS.ToString()) {
                _typelist.Add(ComponentTypes.VSD.ToString());
                _typelist.Add(ComponentTypes.VFD.ToString());
                _typelist.Add(ComponentTypes.RVS.ToString());
            }
            return _typelist;
        }
        set
        { _typelist = value; }
    }


    public double Voltage { get; set; }
    public double Size { get; set; }

    public int AreaId { get; set; }
    private IArea _area;
    private int _sequenceNumber;
    private List<string> _typelist = new List<string>();

    public IArea Area
    {
        get { return _area; }
        set
        {
            var oldValue = _area;
            _area = value;
            if (Area != null) {
                AreaManager.UpdateArea(this, _area, oldValue);

                if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                    var cmd = new UndoCommandDetail { Item = this, PropName = nameof(Area), OldValue = oldValue, NewValue = _area };
                    UndoManager.AddUndoCommand(cmd);
                }
                OnPropertyUpdated();
            }

        }
    }
    public string NemaRating { get; set; }
    public string AreaClassification { get; set; }
    public int OwnerId { get; set; }
    public string OwnerType { get; set; }
    public IEquipment Owner { get; set; }
    public int SequenceNumber
    {
        get => _sequenceNumber;
        set
        {
            _sequenceNumber = value;
            OnPropertyUpdated();
        }
    }

    public int PowerCableId { get; set; }

    public CableModel PowerCable { get; set; }





    public double HeatLoss { get; set; }
    public string Unit { get; set; }
    public double PdTrip { get; set; }
    public double PdFrame { get; set; }
    public double Fla { get; set; }
    public string FedFromTag { get; set; }
    public int FedFromId { get; set; }
    public string FedFromType { get; set; }
    public IDteq FedFrom { get; set; }
    public double AmpacityFactor { get; set; }
    public string PdType { get; set; }
    public double PdSizeTrip { get; set; }
    public double PdSizeFrame { get; set; }

    public event EventHandler PropertyUpdated;

    public async Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
    {
        await Task.Run(() => {
            if (PropertyUpdated != null) {
                PropertyUpdated(this, EventArgs.Empty);
            }
        });
    }

    public async Task UpdateAreaProperties()
    {
        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            NemaRating = Area.NemaRating;
            AreaClassification = Area.AreaClassification;
        }));

    }

    public event EventHandler AreaChanged;
    public virtual async Task OnAreaChanged()
    {
        await Task.Run(() => {
            if (AreaChanged != null) {
                AreaChanged(this, EventArgs.Empty);
            }
        });
    }

    public void MatchOwnerArea(object source, EventArgs e)
    {
        IEquipment owner = (IEquipment)source;
        AreaManager.UpdateArea(this, owner.Area, Area);
        OnPropertyUpdated();
    }

    public void CreatePowerCable()
    {
        throw new NotImplementedException();
    }

    public void SizePowerCable()
    {
        throw new NotImplementedException();
    }

    public void CalculateCableAmps()
    {
        throw new NotImplementedException();
    }
}
