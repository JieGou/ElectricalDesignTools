using EDTLibrary.DataAccess;
using EDTLibrary.LibraryData;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.Models.Validators;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EDTLibrary.Models.Components;

[Serializable]
[AddINotifyPropertyChangedInterface]

public abstract class ComponentModelBase : IComponentEdt
{

    public ComponentModelBase()
    {
    }

    public bool IsSelected { get; set; } = false;

    public int Id { get; set; }

    public bool SettingTag { get; set; } = false;
    public string Tag
    {
        get { return _tag; }
        set
        {
            if (value == null || value == _tag) return;
            if (TagAndNameValidator.IsTagAvailable(value, ScenarioManager.ListManager) == false) {
                return;
            }
            var oldValue = _tag;
            _tag = value;

            if (DaManager.GettingRecords == true) return;

            if (Owner != null) {
                
                CableManager.UpdateComponentCableTags((IPowerConsumer)Owner);
                if (Owner is LoadModel) {
                    CableManager.UpdateLcsCableTags((LoadModel)Owner);
                }
            }

            UndoManager.AddUndoCommand(this, nameof(Tag), oldValue, _tag);
            OnPropertyUpdated();
        }
    }
    private string _tag;



    public string Description { get; set; }
    public string Category { get; set; } //Component
    public string SubCategory { get; set; }

    public string Type
    {
        get => _type;
        set
        {
            if (value == _type) return;
            var oldValue = _type;
            _type = value;

            UndoManager.Lock(this, nameof(Type));
                if (_type == DisconnectTypes.FDS.ToString() || _type == DisconnectTypes.FWDS.ToString()) {
                    var owner = (IPowerConsumer)Owner;
                    if (owner!= null) {
                        TripAmps = TypeManager.BreakerSizes.FirstOrDefault(f => f.TripAmps >= owner.Fla).TripAmps;
                    }
                }
            UndoManager.AddUndoCommand(this, nameof(Type), oldValue, _type);
            OnPropertyUpdated();
        }
    }
    private string _type;
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
    public double FrameAmps
    {
        get => _size;
        set 
        {

            var oldValue = _size;
            _size = value;

            UndoManager.AddUndoCommand(this, nameof(FrameAmps), oldValue, _size);
            OnPropertyUpdated();
        }
    }
    public double TripAmps 
    { 
        get => _trip;
        set
        {

            var oldValue = _trip;
            _trip = value;

            if (DaManager.GettingRecords) return;

            UndoManager.AddUndoCommand(this, nameof(TripAmps), oldValue, _trip);
            OnPropertyUpdated();
        }
    }
    public string StarterSize { get; set; }

    public int AreaId { get; set; }
    private int _sequenceNumber;
    private List<string> _typelist = new List<string>();


    public IArea Area
    {
        get { return _area; }
        set
        {
            var oldValue = _area;
            _area = value;
            if (_area != null) {

                UndoManager.Lock(this, nameof(Area));
                AreaManager.UpdateArea(this, _area, oldValue);
                UndoManager.AddUndoCommand(this, nameof(Area), oldValue, _area);

                OnPropertyUpdated();
            }

        }
    }
    private IArea _area;
    public string NemaRating
    {
        get => _nemaRating;
        set
        {
            if (value == null) return;

            var oldValue = _nemaRating;
            _nemaRating = value;

            UndoManager.AddUndoCommand(this, nameof(NemaRating), oldValue, _nemaRating);
            OnPropertyUpdated();
        }
    }
    private string _nemaRating;
    private double _size;
    private double _trip;

    public string AreaClassification
    {
        get => _areaClassification;
        set
        {
            if (value == null) return;

            var oldValue = _areaClassification;
            _areaClassification = value;

            UndoManager.AddUndoCommand(this, nameof(AreaClassification), oldValue, _areaClassification);
            OnPropertyUpdated();
        }
    }
    private string _areaClassification;
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

    public CableModel PowerCable { get; set; }


    public double HeatLoss { get; set; }

    public void CalculateSize(IPowerConsumer load)
    {
        if (Type == ComponentTypes.UDS.ToString() || Type == ComponentTypes.FDS.ToString()) {
            FrameAmps = DataTableSearcher.GetDisconnectSize(load);

        }
        if (Type == ComponentTypes.UDS.ToString()) {
            TripAmps = DataTableSearcher.GetDisconnectFuse(load);
        }
        OnPropertyUpdated();
    }

    public override string ToString()
    {
        return Tag;
    }

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
                UndoManager.CanAdd = false;
                AreaChanged(this, EventArgs.Empty);
                UndoManager.CanAdd = true;
            }
        });
    }

    public void MatchOwnerArea(object source, EventArgs e)
    {
        IEquipment owner = (IEquipment)source;
        UndoManager.CanAdd = false;
        AreaManager.UpdateArea(this, owner.Area, Area);
        UndoManager.CanAdd = true;
        OnPropertyUpdated();
    }
}
