using PropertyChanged;
using System;
using System.ComponentModel;

namespace EDTLibrary.Models.Areas;

[AddINotifyPropertyChangedInterface]
public class AreaModel : IArea { 
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
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new CommandDetail { Item = this, PropName = nameof(Tag), OldValue = oldValue, NewValue = _tag };
                Undo.UndoList.Add(cmd);

            }

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
                if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                    var cmd = new CommandDetail { Item = this, PropName = nameof(DisplayTag), OldValue = oldValue, NewValue = _displayTag };
                    Undo.UndoList.Add(cmd);
                }
            }

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
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new CommandDetail { Item = this, PropName = nameof(Name), OldValue = oldValue, NewValue = _name };
                Undo.UndoList.Add(cmd);
            }
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
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new CommandDetail { Item = this, PropName = nameof(Description), OldValue = oldValue, NewValue = _description };
                Undo.UndoList.Add(cmd);
            }
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
            ParentAreaId = _parentArea.Id;
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new CommandDetail { Item = this, PropName = nameof(ParentArea), OldValue = oldValue, NewValue = _parentArea };
                Undo.UndoList.Add(cmd);
            }
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
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new CommandDetail { Item = this, PropName = nameof(AreaCategory), OldValue = oldValue, NewValue = _areaCategory };
                Undo.UndoList.Add(cmd);
            }
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
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new CommandDetail { Item = this, PropName = nameof(AreaClassification), OldValue = oldValue, NewValue = _areaClassification };
                Undo.UndoList.Add(cmd);
            }
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
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new CommandDetail { Item = this, PropName = nameof(MinTemp), OldValue = oldValue, NewValue = _minTemp };
                Undo.UndoList.Add(cmd);
            }
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
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new CommandDetail { Item = this, PropName = nameof(MaxTemp), OldValue = oldValue, NewValue = _maxTemp };
                Undo.UndoList.Add(cmd);
            }
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
            if (Undo.Undoing == false && GlobalConfig.GettingRecords == false) {
                var cmd = new CommandDetail { Item = this, PropName = nameof(NemaRating), OldValue = oldValue, NewValue = _nemaRating };
                Undo.UndoList.Add(cmd);
            }
        }
    }

    public event EventHandler AreaPropertiesChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged()
    {
        OnAreaPropertiesChanged();
    }
    public virtual void OnAreaPropertiesChanged()
    {
        if (AreaPropertiesChanged != null) {
            AreaPropertiesChanged(this, EventArgs.Empty);
        }
    }

}
