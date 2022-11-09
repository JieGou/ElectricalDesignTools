using EDTLibrary.DataAccess;
using EDTLibrary.Managers;
using EDTLibrary.Models.Cables;
using EDTLibrary.Models.Loads;
using EDTLibrary.UndoSystem;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Raceways;
[Serializable]
[AddINotifyPropertyChangedInterface]
public class RacewayModel
{
    public int Id { get; set; }

    private string _tag;
    public string Tag 
    { 
        get => _tag; 
        set { _tag = value;
            OnPropertyUpdated();
        }
    }

    public string Category { get; set; }

    public string Type
    {
        get
        {
            return _type;
        }
        set
        {
            _type = value;

        }

    }
    private string _type;


    private string _material;
    public string Material
    {
        get { return _material; }
        set { _material = value; }
    }

    private double _width;
    public double Width
    {
        get { return _width; }
        set
        {
            _width = value;
            OnPropertyUpdated();

        }
    }

    private double _height;

    public double Height
    {
        get { return _height; }
        set
        {
            _height = value;
            OnPropertyUpdated();
        }
    }
    public double Diameter
    {
        get { return _diameter; }
        set
        {
            _diameter = value;
            OnPropertyUpdated();
        }
    }
    private double _diameter;
    

    public ObservableCollection<ICable> CableList { get; set; } = new ObservableCollection<ICable>();

    //Events
    public event EventHandler PropertyUpdated;
    public virtual async Task OnPropertyUpdated()
    {
        await Task.Run(() => {
            if (PropertyUpdated != null) {
                PropertyUpdated(this, EventArgs.Empty);
            }
        });
    }
}
