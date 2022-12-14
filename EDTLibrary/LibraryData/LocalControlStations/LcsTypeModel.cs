using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.LibraryData.LocalControlStations;
public class LcsTypeModel
{
    private string _type;

    public int Id { get; set; }
    public string Type { 
        get => _type; 
        set 
        {
            if (string.IsNullOrEmpty(value.ToString())) return;
            _type = value; 
        } 
    }
    public string Description { get; set; }
    public int DigitalConductorQty { get; set; }
    public int AnalogConductorQty { get; set; }
}
