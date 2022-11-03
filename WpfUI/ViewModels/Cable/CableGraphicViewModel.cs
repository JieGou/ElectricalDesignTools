using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.ViewModels.Cable;
public class CableGraphicViewModel : IRacewaySizerGraphic
{
    public CableGraphicViewModel(string tag, double diameter, double x, double y)
    {
        Tag = tag;
        Diameter = diameter;
        X = x;
        Y = y;
    }
    public string Tag { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    //Cable
    public double Diameter { get; set; } = 25;

    //Tray
    public double Width { get; set; }
    public double Height { get; set; }
}
