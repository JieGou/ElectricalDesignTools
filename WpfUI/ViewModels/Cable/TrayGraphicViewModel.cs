using EDTLibrary.Models.Cables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.ViewModels.Cable;
public class TrayGraphicViewModel : IRacewaySizerGraphic
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

    private string _toolTip;
    public string ToolTip
    {
        get { return _toolTip; }
        set { _toolTip = value; }
    }

    public int QtyOfTotal { get; set;}

    public double X { get; set; }
    public double Y { get; set; }


    //Tray
    public double Height { get; set; }
    public double Width { get; set; }


    //Cable
    public double Diameter { get; set; }
    public ICable CableModel { get; set; }

}
