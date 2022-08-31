﻿using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
using EDTLibrary.LibraryData;
using EDTLibrary.LibraryData.TypeModels;
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace EDTLibrary.Models.Components;

[AddINotifyPropertyChangedInterface]

public class ComponentModel : IComponentEdt
{
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
            if (value == null || value == _tag) return;
            if (TagAndNameValidator.IsTagAvailable(value, ScenarioManager.ListManager) == false) {
                ErrorHelper.NotifyUserError(ErrorMessages.DuplicateTagMessage, "Duplicate Tag Error", image: MessageBoxImage.Exclamation);
                return;
            }
            var oldValue = _tag;
            _tag = value;

            if (DaManager.GettingRecords == true) return;
            if (Owner != null) {
                if (CableManager.IsUpdatingPowerCables == false) {
                    //CableManager.AddAndUpdateLoadPowerComponentCablesAsync((IPowerConsumer)Owner, ScenarioManager.ListManager);
                }
            }

            UndoManager.AddUndoCommand(this, nameof(Tag), oldValue, _tag); 
            OnPropertyUpdated();
        }
    }



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

            UndoManager.AddUndoCommand(this, nameof(Type),oldValue,_type);
            OnPropertyUpdated();
        }
    }
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
    public double Trip { get; set; }

    public int AreaId { get; set; }
    private IArea _area;
    private int _sequenceNumber;
    private List<string> _typelist = new List<string>();
    
    private string _type;

    public IArea Area
    {
        get { return _area; }
        set
        {
            var oldValue = _area;
            _area = value;
            if (Area != null) {

                UndoManager.Lock(this, nameof(Area));
                AreaManager.UpdateArea(this, _area, oldValue);
                UndoManager.AddUndoCommand(this, nameof(Area), oldValue, _area);

                OnPropertyUpdated();
            }

        }
    }
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
            Size = DataTableManager.GetDisconnectSize(load);

        }
        if (Type == ComponentTypes.UDS.ToString()){
            Trip = DataTableManager.GetDisconnectFuse(load);
        }
        OnPropertyUpdated();
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
