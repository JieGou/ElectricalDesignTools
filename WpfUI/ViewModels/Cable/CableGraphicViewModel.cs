using EDTLibrary.Models.Cables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUI.ViewModels.Cable;
public class CableGraphicViewModel : IRacewaySizerGraphic
{
    public CableGraphicViewModel(string tag, double diameter, double x, double y, ICable cableModel)
    {
        Tag = tag;
        Diameter = diameter;
        X = x;
        Y = y;
        CableModel = cableModel;
    }

    public string Tag { get; set; }

    private string _toolTip;
    public string ToolTip
    {
        get { return 
                $"Type: {CableModel.Type}\n" +
                $"Size: {CableModel.Size}\n" +
                $"Runs: {QtyOfTotal}/{CableModel.QtyParallel}"; 
            }
    }
    public int QtyOfTotal { get; set; }

    public double X { get; set; }
    public double Y { get; set; }

    //Cable
    public double Diameter { get; set; } = 25;

    //Tray
    public double Width { get; set; }
    public double Height { get; set; }
    public ICable CableModel { get; set; }

}
