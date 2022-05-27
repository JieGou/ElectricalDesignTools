using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDTLibrary.LibraryData.TypeModels;
public class LcsTypeModel
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public int RequiredDigitalConductors { get; set; }
    public int RequiredAnalogConductors { get; set; }
}
