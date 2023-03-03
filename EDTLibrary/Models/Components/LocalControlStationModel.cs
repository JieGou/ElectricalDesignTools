using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.LocalControlStations;
using EDTLibrary.Managers;
using EDTLibrary.Models.Areas;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Equipment;
using EDTLibrary.Models.Loads;
using EDTLibrary.UndoSystem;
using EDTLibrary.Validators;
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
[Serializable]
[AddINotifyPropertyChangedInterface]
public class LocalControlStationModel : ILocalControlStation
{

    public bool IsCalculationLocked
    {
        get { return _isCalculationLocked; }
        set
        {
            _isCalculationLocked = value;
            OnPropertyUpdated();
        }
    }
    private bool _isCalculationLocked;
    public LocalControlStationModel()
    {
        Type = "LCS";
    }

    public bool IsValid { get; set; } = true;
    public void Validate() 
    {
        IsValid = true;
        OnPropertyUpdated();

        return;
    }

    public bool IsSelected { get; set; } = false;

    public int Id { get; set; }
    public string Tag
    {
        get => _tag;
        set
        {
            if (value == null) return;
            if (TagAndNameValidator.IsTagAvailable(value, ScenarioManager.ListManager) == false) {
                return;
            }
            var oldValue = _tag;
            _tag = value;

            UndoManager.Lock(this, nameof(Tag));
            if (DaManager.GettingRecords == false) {
                    CableManager.UpdateLcsCableTags((LoadModel)Owner);
                if (ControlCable != null) {
                }
            }
            UndoManager.AddUndoCommand(this, nameof(Tag), oldValue, _tag);

            OnPropertyUpdated($"{nameof(Tag)}: {Tag}");
        }
    }
    public string Category { get; set; }
    public string SubCategory { get; set; }


    public string Type
    {
        get => _type;

        set
        {
            var oldValue = _type;
            if (value == null) return;
            _type = value;
            TypeModel = TypeManager.GetLcsTypeModel(_type);

            UndoManager.Lock(this, nameof(Type));
            UndoManager.AddUndoCommand(this, nameof(Type), oldValue, _type);



            OnPropertyUpdated(nameof(Type));
        }
    }

    private int _typeId;

    public int TypeId
    {
        get { return _typeId; }
        set 
        {
            var oldValue = _typeId;
            _typeId = value; 

            UndoManager.Lock(this, nameof(TypeModel));
            UndoManager.AddUndoCommand(this, nameof(Type), oldValue, _typeId);
        }
    }

    public LcsTypeModel TypeModel 
    {
        get => _typeModel;
        set
        {
            if (value == null) return;
            
            var oldValue = _typeModel;

            _typeModel = value;
            
            TypeId = _typeModel.Id;

            if (!DaManager.Importing) {
                CableManager.UpdateLcsCableTypes(this);
            }
            UndoManager.Lock(this, nameof(TypeModel));
            UndoManager.AddUndoCommand(this, nameof(Type), oldValue, _typeModel);
        }
    }
    private LcsTypeModel _typeModel;

    public string Description { get; set; }


    public string SubType { get; set; }

    public ObservableCollection<LcsTypeModel> TypeList { get; set; } = new ObservableCollection<LcsTypeModel>();
    public void UpdateTypelist(bool driveBool)
    {
        var templist = driveBool ? 
            TypeManager.LcsTypes.Where(lt => lt.AnalogConductorQty > 1).ToList() : 
            TypeManager.LcsTypes.Where(lt => lt.AnalogConductorQty ==0 || lt.AnalogConductorQty == null).ToList();
        TypeList = new ObservableCollection<LcsTypeModel>(templist);

    }
    public IEquipment Owner { get; set; }

    public int OwnerId { get; set; }
    public string OwnerType { get; set; }
    public int SequenceNumber { get; set; }

    public int AreaId { get; set; }


    public IArea _area;
    private string _tag;

    public IArea Area
    {
        get { return _area; }
        set
        {
            if (value == null) return;
            var oldValue = _area;
            _area = value;

            UndoManager.CanAdd = false;
            AreaManager.UpdateArea(this, _area, oldValue);

            UndoManager.CanAdd = true;
            UndoManager.AddUndoCommand(this, nameof(Area), oldValue, _area);

            OnPropertyUpdated();
        }
    }
    public double Voltage { get; set; }
    private string _nemaRating;
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
    private string _areaClassification;
    private string _type;

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
    public ICable ControlCable { get; set; }


    public ICable AnalogCable { get; set; }

    public double HeatLoss { get; set; } //not used







    public event EventHandler PropertyUpdated;

    public virtual async Task OnPropertyUpdated(string property = "default", [CallerMemberName] string callerMethod = "")
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
    public async Task OnAreaChanged()
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

   
}
