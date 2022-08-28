﻿using EDTLibrary.DataAccess;
using EDTLibrary.ErrorManagement;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            if (value == null) return;
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

            var cmd = new UndoCommandDetail { Item = this, PropName = nameof(Tag), OldValue = oldValue, NewValue = _tag };
            UndoManager.AddUndoCommand(cmd);

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
    private string _nemaRating;
    private string _areaClassification;

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
    public string NemaRating
    {
        get => _nemaRating;
        set
        {
            _nemaRating = value;
            OnPropertyUpdated();
        }
    }
    public string AreaClassification
    {
        get => _areaClassification;
        set
        {
            _areaClassification = value;
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
}
