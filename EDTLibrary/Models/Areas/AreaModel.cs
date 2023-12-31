﻿using EDTLibrary.DataAccess;
using EDTLibrary.Models.Equipment;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EDTLibrary.Models.Areas;

[Serializable]
[AddINotifyPropertyChangedInterface]
public class AreaModel : IArea {

    public AreaHeatLossCalculator AreaHeatLossCalculator { get; set; }
    public int Id { get; set; }

    private string _tag;
    public string Tag
    {
        get { return _tag; }
        set
        {
            var oldValue = _tag;
            if (string.IsNullOrWhiteSpace(value) == true) 
                return;


            _tag = value;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(Tag), OldValue = oldValue, NewValue = _tag };
                UndoManager.AddUndoCommand(cmd);

            }
            OnPropertyUpdated();
        }
    }

    private string _displayTag;
    public string DisplayTag
    {
        get { return _displayTag; }
        set
        {
            var oldValue = _displayTag;
            if (string.IsNullOrWhiteSpace(value) == false) {
                _displayTag = value;
                if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                    var cmd = new UndoCommand { Item = this, PropName = nameof(DisplayTag), OldValue = oldValue, NewValue = _displayTag };
                    UndoManager.AddUndoCommand(cmd);
                }
            }
            OnPropertyUpdated();

        }
    }

    private string _name;

    public string Name
    {
        get { return _name; }
        set 
        { 
            var oldValue = Name;
            _name = value;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(Name), OldValue = oldValue, NewValue = _name };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }

    private string _description;

    public string Description
    {
        get { return _description; }
        set 
        {
            var oldValue = _description;
            _description = value;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(Description), OldValue = oldValue, NewValue = _description };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }

    public int ParentAreaId { get; set; }

    private IArea _parentArea;
    public IArea ParentArea
    {
        get { return _parentArea; }
        set 
        {
            var oldValue = _parentArea;
            _parentArea = value;
            if (_parentArea == null) return;
            ParentAreaId = _parentArea.Id;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(ParentArea), OldValue = oldValue, NewValue = _parentArea };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }


    private string _areaCategory;

    public string AreaCategory
    {
        get { return _areaCategory; }
        set 
        { 
            var oldValue = _areaCategory;
            _areaCategory = value;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(AreaCategory), OldValue = oldValue, NewValue = _areaCategory };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }

    private string _areaClassification;

    public string AreaClassification
    {
        get { return _areaClassification; }
        set 
        { 
            var oldValue = _areaClassification;
            _areaClassification = value;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(AreaClassification), OldValue = oldValue, NewValue = _areaClassification };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }

    private double _minTemp;

    public double MinTemp
    {
        get { return _minTemp; }
        set 
        {
            var oldValue = _minTemp;
            _minTemp = value;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(MinTemp), OldValue = oldValue, NewValue = _minTemp };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }

    private double _maxTemp;

    public double MaxTemp
    {
        get { return _maxTemp; }
        set
        {
            var oldValue = _maxTemp;
            _maxTemp = value;
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(MaxTemp), OldValue = oldValue, NewValue = _maxTemp };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }


    private string _nemaRating;

    public string NemaRating
    {
        get {return _nemaRating; }

        set 
        { 
            var oldValue = _nemaRating;
            _nemaRating = value;
            OnAreaPropertiesChanged();
            if (UndoManager.IsUndoing == false && DaManager.GettingRecords == false) {
                var cmd = new UndoCommand { Item = this, PropName = nameof(NemaRating), OldValue = oldValue, NewValue = _nemaRating };
                UndoManager.AddUndoCommand(cmd);
            }
            OnPropertyUpdated();
        }
    }

    private ObservableCollection<IEquipment> _equipmentList = new ObservableCollection<IEquipment>();

    public double HeatLoss { get; set; }
    public ObservableCollection<IEquipment> EquipmentList
    {
        get => _equipmentList;

        set
        {
            _equipmentList = value;
        }
    }


    public override string ToString()
    {
        return Tag;
    }


    public event EventHandler PropertyUpdated;
    public void OnPropertyUpdated()
    {
        if (PropertyUpdated != null) {
            PropertyUpdated(this, EventArgs.Empty);
        }
    }


    public event EventHandler AreaPropertiesChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    //public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnAreaPropertiesChanged()
    {
        if (AreaPropertiesChanged != null) {
            AreaPropertiesChanged(this, EventArgs.Empty);
        }
    }

}
