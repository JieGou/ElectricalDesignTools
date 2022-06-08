using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.ViewModels.Cable;
public class TrayGraphicViewModel : ITraySizerGraphic
{
    public TrayGraphicViewModel(string tag, double width, double height, double x, double y)
    {
        Tag = tag;
        Width = width;
        Height = height;
        X = x;
        Y = y;
    }

    public string Tag { get; set; }
  
    public double X { get; set; }
    public double Y { get; set; }


    //Tray
    public double Height { get; set; }
    public double Width { get; set; }


    //Cable
    public double Diameter { get; set; }

}
