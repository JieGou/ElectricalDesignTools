using PropertyChanged;
using System;

namespace EDTLibrary.Models.Areas;

[AddINotifyPropertyChangedInterface]
public class AreaModel : IArea
{
    public int Id { get; set; }
    private string _tag;

    public string Tag
    {
        get { return _tag; }
        set
        {
            if (string.IsNullOrWhiteSpace(value) == false) {
                _tag = value;
            }
        }
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public int AreaId { get; set; }
    public IArea Area { get; set; }

    public string AreaCategory { get; set; }
    public string AreaClassification { get; set; }

    public double MinTemp { get; set; }
    public double MaxTemp { get; set; }

    private string _nemaRating;

    public string NemaRating
    {
        get {return _nemaRating; }

        set 
        { 
            _nemaRating = value;
            OnAreaPropertiesChanged();
        }
    }

    public event EventHandler AreaPropertiesChanged;
    public virtual void OnAreaPropertiesChanged()
    {
        if (AreaPropertiesChanged != null) {
            AreaPropertiesChanged(this, EventArgs.Empty);
        }
    }

}
