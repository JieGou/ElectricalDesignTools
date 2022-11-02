using EDTLibrary.DataAccess;
using EDTLibrary.Managers;
using EDTLibrary.Models.Loads;
using EDTLibrary.UndoSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.Models.Raceways;
public class RacewayModel
{
    public int Id { get; set; }
    public string Tag { get; set; }

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
        set { _width = value; }
    }

    private double _height;

    public double Height
    {
        get { return _height; }
        set { _height = value; }
    }
    public double Diameter
    {
        get { return _diameter; }
        set
        {
            _diameter = value;
        }
    }
    private double _diameter;
   

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
